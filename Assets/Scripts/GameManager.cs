using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject mapLv1;
    public GameObject mapLv2;
    public GameObject mapLv3;
    public Transform spawnPointMap1;
    public Transform spawnPointMap2;
    public Transform spawnPointMap3;
    public GameObject pauseMenuUI;
    public GameObject levelCompleteUI;
    public GameObject player; 
    private static int currentLevel = 1;
    public int nextLevelToLoad;
    private static int selectedLevel = 1;
    public PlayerScore playerScore; 
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

        
        if (playerScore == null && player != null)
        {
            playerScore = player.GetComponent<PlayerScore>();
            if (playerScore == null)
            {
                Debug.LogError("Không tìm thấy component PlayerScore trên Player!");
            }
        }
        else if (playerScore == null)
        {
            Debug.LogError("Chưa gán PlayerScore vào GameManager!");
        }
    }

    void Start()
    {
        ActivateLevel(selectedLevel);
        SetPlayerSpawnPosition(selectedLevel);
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Level Complete UI không được gán trong GameManager!");
        }

        
        if (playerScore != null)
        {
            playerScore.gemCount = 0;
            playerScore.UpdateGemCountUI();
        }
    }

    private void ActivateLevel(int level)
    {
        if (mapLv1 != null) mapLv1.SetActive(level == 1);
        if (mapLv2 != null) mapLv2.SetActive(level == 2);
        if (mapLv3 != null) mapLv3.SetActive(level == 3);
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
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ShowLevelCompleteUI(int nextLevel)
    {
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(true);
            Time.timeScale = 0f;
            nextLevelToLoad = nextLevel;
        }
        else
        {
            Debug.LogError("Level Complete UI không được gán trong GameManager!");
        }
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        if (nextLevelToLoad > currentLevel)
        {
            ActivateLevel(currentLevel);
            ActivateLevel(nextLevelToLoad);
            SetPlayerSpawnPosition(nextLevelToLoad);
            currentLevel = nextLevelToLoad;
        }
        else
        {
            Debug.LogWarning("Không có level tiếp theo hợp lệ để tải.");
        }
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(false);
        }
    }
}