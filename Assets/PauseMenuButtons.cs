using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButtons : MonoBehaviour
{
    public void GoHome()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.LoadMainMenu();
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
            SceneManager.LoadScene(0); // Fallback nếu GameManager không tồn tại
            Time.timeScale = 1f;
        }
    }

    public void Resume()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ResumeGame();
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
            SceneManager.LoadScene("Gameplay"); // Fallback
            Time.timeScale = 1f;
        }
    }

    public void Quit()
    {
        SceneManager.LoadScene(0); // Về Main Menu trước khi Quit (tùy chọn)
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
        SceneLoader sceneLoader = GetComponent<SceneLoader>();
        if (sceneLoader != null)
        {
            sceneLoader.QuitGame();
        }
        else
        {
            Debug.LogError("SceneLoader not found on this GameObject!");
            Application.Quit();
        }
    }
}