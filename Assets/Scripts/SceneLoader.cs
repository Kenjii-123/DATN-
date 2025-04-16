using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject tutorialCanvas; 

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ShowTutorial()
    {
        if (tutorialCanvas != null)
        {
            tutorialCanvas.SetActive(true);
        }
        else
        {
            Debug.LogError("Tutorial Canvas chưa được gán trong SceneLoader!");
        }
    }

    public void HideTutorial()
    {
        if (tutorialCanvas != null)
        {
            tutorialCanvas.SetActive(false);
        }
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