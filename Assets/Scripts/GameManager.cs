using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Biến static để tham chiếu đến instance duy nhất của GameManager

    public GameObject mapLv1;
    public GameObject mapLv2;
    public GameObject mapLv3;

    private static int selectedLevel = 1;

    void Awake()
    {
        // Đảm bảo chỉ có một instance của GameManager tồn tại
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // Nếu bạn muốn GameManager tồn tại khi chuyển scene (tùy chọn)
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
    }

    private void ActivateLevel(int level)
    {
        mapLv1.SetActive(level == 1);
        mapLv2.SetActive(level == 2);
        mapLv3.SetActive(level == 3);
    }

    public static void SetSelectedLevel(int level)
    {
        selectedLevel = level;
    }

    public void LoadPauseMenu()
    {
        SceneManager.LoadScene(3);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay"); // Tải lại Gameplay
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}