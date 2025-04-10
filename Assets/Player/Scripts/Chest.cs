using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Chest : MonoBehaviour
{
    public GameObject questionPanel;
    public GameObject keyImage;
    public string nextSceneName;

    private bool playerInRange = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.O))
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        
        animator.SetTrigger("IsOpen");

        Invoke("ShowQuestionPanel", 1f);
    }

    void ShowQuestionPanel()
    {
        questionPanel.SetActive(true);
    }

    public void CheckAnswer(bool isCorrect)
    {
        if (isCorrect)
        {
            keyImage.SetActive(true);
        }
        else
        {
       
        }
    }

   
}