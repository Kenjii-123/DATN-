using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorButton : MonoBehaviour
{
    public int levelToLoad;

    public void LoadLevel()
    {
        GameManager.SetSelectedLevel(levelToLoad);
        SceneManager.LoadScene("Gameplay");
    }
}