using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public struct Question
{
    public string questionText;
    public List<string> answers;
    public int correctAnswerIndex;
}

[System.Serializable]
public class LevelQuestions
{
    public string levelName;
    public List<Question> questions;
    public int questionsToAnswer = 5;
    public string nextSceneName;
}

public class Chest : MonoBehaviour
{
    public GameObject questionPanelPrefab;
    public Transform questionPanelParent;
    public string currentLevelName;
    public List<LevelQuestions> allLevelQuestions;
    public float questionPanelShowDelay = 1f;
    public int gemsNeededToOpen = 5; // Số gem cần để mở rương

    private bool playerInRange = false;
    private Animator animator;
    private int correctAnswers = 0;
    private bool chestOpened = false;
    private GameObject currentQuestionPanelInstance;
    private LevelQuestions currentLevelData;
    private List<Question> currentQuestions;
    private Question currentQuestion;
    private int currentQuestionIndex = 0;
    private GameManager gameManager;

    private const string ProgressKeyPrefix = "LevelProgress_";

    void Start()
    {
        animator = GetComponent<Animator>();
        LoadProgress();
        LoadLevelQuestions();
        gameManager = GameManager.instance;
        if (gameManager == null)
        {
            Debug.LogError("Không tìm thấy GameManager instance!");
        }
    }

    void LoadProgress()
    {
        correctAnswers = PlayerPrefs.GetInt(ProgressKeyPrefix + currentLevelName, 0);
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt(ProgressKeyPrefix + currentLevelName, correctAnswers);
        PlayerPrefs.Save();
    }

    void LoadLevelQuestions()
    {
        currentLevelData = allLevelQuestions.FirstOrDefault(levelData => levelData.levelName == currentLevelName);
        if (currentLevelData == null)
        {
            Debug.LogError("Không tìm thấy dữ liệu câu hỏi cho level: " + currentLevelName);
            enabled = false;
            return;
        }
        currentQuestions = currentLevelData.questions.OrderBy(x => Random.value).Take(currentLevelData.questionsToAnswer).ToList();
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
        if (playerInRange && Input.GetKeyDown(KeyCode.O) && !chestOpened)
        {
            TryOpenChest(); // Gọi hàm thử mở rương
        }
    }

    void TryOpenChest()
    {
        if (gameManager != null && gameManager.playerScore != null && gameManager.playerScore.gemCount >= gemsNeededToOpen)
        {
            OpenChest();
            gameManager.playerScore.gemCount -= gemsNeededToOpen;
            gameManager.playerScore.UpdateGemCountUI();
            Debug.Log("Đã mở rương. Số gem còn lại: " + gameManager.playerScore.gemCount);
        }
        else
        {
            int currentGems = (gameManager != null && gameManager.playerScore != null) ? gameManager.playerScore.gemCount : 0;
            Debug.Log("Không đủ gem để mở rương. Cần " + gemsNeededToOpen + " gem, bạn đang có " + currentGems + " gem.");
            // Hiển thị thông báo cho người chơi trên UI (nếu cần)
        }
    }

    void OpenChest()
    {
        chestOpened = true;
        animator.SetTrigger("IsOpen");
        Invoke("ShowQuestionPanel", questionPanelShowDelay);
    }

    void ShowQuestionPanel()
    {
        if (questionPanelPrefab != null && questionPanelParent != null && currentQuestions.Count > 0)
        {
            currentQuestion = currentQuestions[currentQuestionIndex];
            currentQuestionPanelInstance = Instantiate(questionPanelPrefab, questionPanelParent);
            QuestionPanelUI questionPanelUI = currentQuestionPanelInstance.GetComponent<QuestionPanelUI>();
            if (questionPanelUI != null)
            {
                questionPanelUI.SetQuestion(currentQuestion, this);
            }
            else
            {
                Debug.LogError("QuestionPanelUI script không được tìm thấy trên prefab bảng câu hỏi.");
            }
        }
        else
        {
            Debug.LogError("Question Panel Prefab hoặc Parent chưa được gán, hoặc không có câu hỏi nào.");
        }
    }

    public void CheckAnswer(bool isCorrect)
    {
        if (currentQuestionPanelInstance != null)
        {
            Destroy(currentQuestionPanelInstance);
            currentQuestionPanelInstance = null;
        }

        if (isCorrect)
        {
            correctAnswers++;
            SaveProgress();
            Debug.Log("Correct answers: " + correctAnswers + "/" + currentLevelData.questionsToAnswer);
        }
        else
        {
            Debug.Log("Incorrect answer. Correct answers: " + correctAnswers + "/" + currentLevelData.questionsToAnswer);
        }

        currentQuestionIndex++;
        if (currentQuestionIndex < currentLevelData.questionsToAnswer)
        {
            Invoke("ShowQuestionPanel", questionPanelShowDelay);
        }
        else
        {
            Debug.Log("Đã trả lời hết số câu hỏi yêu cầu.");
            if (gameManager != null && currentLevelData != null)
            {
                if (int.TryParse(currentLevelData.nextSceneName, out int nextLevel))
                {
                    gameManager.ShowLevelCompleteUI(nextLevel);
                }
                else
                {
                    Debug.LogError("nextSceneName trong LevelQuestions không phải là số hợp lệ cho level tiếp theo.");
                    gameManager.ShowLevelCompleteUI(GameManager.instance.nextLevelToLoad + 1);
                }
            }
        }
    }
}