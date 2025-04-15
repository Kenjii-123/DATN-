using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject mapLv1;
    public GameObject mapLv2;
    public GameObject mapLv3;
    public Transform spawnPointMap1; // Điểm spawn cho Level 1
    public Transform spawnPointMap2; // Điểm spawn cho Level 2
    public Transform spawnPointMap3; // Điểm spawn cho Level 3
    public GameObject pauseMenuUI; // Tham chiếu đến Canvas Pause Menu trong Gameplay
    public GameObject player; // Tham chiếu đến GameObject nhân vật (cần gán trong Inspector)
    private static int selectedLevel = 1;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        ActivateLevel(selectedLevel);
        SetPlayerSpawnPosition(selectedLevel); // Đặt vị trí ban đầu
    }

    private void ActivateLevel(int level)
    {
        mapLv1.SetActive(level == 1);
        mapLv2.SetActive(level == 2);
        mapLv3.SetActive(level == 3);
    }

    private void SetPlayerSpawnPosition(int level)
    {
        if (player != null)
        {
            switch (level)
            {
                case 1:
                    player.transform.position = spawnPointMap1.position;
                    break;
                case 2:
                    player.transform.position = spawnPointMap2.position;
                    break;
                case 3:
                    player.transform.position = spawnPointMap3.position;
                    break;
                default:
                    Debug.LogError("Invalid level selected: " + level);
                    break;
            }
        }
        else
        {
            Debug.LogError("Player GameObject not assigned in GameManager!");
        }
    }

    public static void SetSelectedLevel(int level) => selectedLevel = level;

    public void LoadPauseMenu()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogError("Pause Menu UI không được gán trong GameManager!");
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}