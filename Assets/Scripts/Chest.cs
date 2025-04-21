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

public class Chest : MonoBehaviour
{
    public GameObject questionPanelPrefab;
    public Transform questionPanelParent;
    public List<Question> questions;
    public int questionsToAnswer = 5;
    public float questionPanelShowDelay = 1f;
    public int gemsNeededToOpen = 5;
    public string nextSceneName;

    private bool playerInRange = false;
    private Animator animator;
    private int correctAnswers = 0;
    private bool chestOpened = false;
    private GameObject currentQuestionPanelInstance;
    private List<Question> currentQuestionsToAsk;
    private Question currentQuestion;
    private int currentQuestionIndex = 0;
    private GameManager gameManager;

    private string levelName;

    void Start()
    {
        animator = GetComponent<Animator>();
        gameManager = GameManager.instance;
        if (gameManager == null)
        {
            Debug.LogError("Không tìm thấy GameManager instance!");
        }

        levelName = SceneManager.GetActiveScene().name;
        LoadProgress();

        currentQuestionsToAsk = questions.OrderBy(x => Random.value).Take(questionsToAnswer).ToList();

        if (currentQuestionsToAsk.Count == 0 && questions.Count > 0)
        {
            Debug.LogWarning($"Chest trên level {levelName} có {questions.Count} câu hỏi nhưng chỉ hỏi {questionsToAnswer}. Đảm bảo {questionsToAnswer} nhỏ hơn hoặc bằng số lượng câu hỏi.");
        }
        else if (questions.Count == 0)
        {
            Debug.LogWarning($"Chest trên level {levelName} không có câu hỏi nào.");
        }
    }

    void LoadProgress()
    {
        correctAnswers = PlayerPrefs.GetInt(ProgressKeyPrefix() + levelName + "_" + GetInstanceID(), 0);
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt(ProgressKeyPrefix() + levelName + "_" + GetInstanceID(), correctAnswers);
        PlayerPrefs.Save();
    }

    private string ProgressKeyPrefix()
    {
        return "ChestProgress_";
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
            TryOpenChest();
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
        if (questionPanelPrefab != null && questionPanelParent != null && currentQuestionsToAsk.Count > 0 && currentQuestionIndex < currentQuestionsToAsk.Count)
        {
            currentQuestion = currentQuestionsToAsk[currentQuestionIndex];
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
            Debug.LogError("Question Panel Prefab hoặc Parent chưa được gán, hoặc không có câu hỏi nào để hiển thị.");
            if (currentQuestionsToAsk.Count > 0 && currentQuestionIndex >= currentQuestionsToAsk.Count)
            {
                Debug.Log("Đã trả lời hết số câu hỏi yêu cầu từ chest này.");
                if (!string.IsNullOrEmpty(nextSceneName) && gameManager != null)
                {
                    if (int.TryParse(nextSceneName, out int nextLevel))
                    {
                        gameManager.ShowLevelCompleteUI(nextLevel);
                    }
                    else
                    {
                        Debug.LogError("nextSceneName không phải là số hợp lệ.");
                        gameManager.ShowLevelCompleteUI(GameManager.instance.nextLevelToLoad + 1);
                    }
                }
                else if (gameManager != null)
                {
                    gameManager.ShowLevelCompleteUI(GameManager.instance.nextLevelToLoad + 1);
                }
            }
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
            Debug.Log("Correct answers: " + correctAnswers + "/" + currentQuestionsToAsk.Count);
        }
        else
        {
            Debug.Log("Incorrect answer. Correct answers: " + correctAnswers + "/" + currentQuestionsToAsk.Count);
        }

        currentQuestionIndex++;
        if (currentQuestionIndex < currentQuestionsToAsk.Count)
        {
            Invoke("ShowQuestionPanel", questionPanelShowDelay);
        }
        else
        {
            Debug.Log("Đã trả lời hết số câu hỏi yêu cầu từ chest này.");
            if (!string.IsNullOrEmpty(nextSceneName) && gameManager != null)
            {
                if (int.TryParse(nextSceneName, out int nextLevel))
                {
                    gameManager.ShowLevelCompleteUI(nextLevel);
                }
                else
                {
                    Debug.LogError("nextSceneName không phải là số hợp lệ.");
                    gameManager.ShowLevelCompleteUI(GameManager.instance.nextLevelToLoad + 1);
                }
            }
            else if (gameManager != null)
            {
                gameManager.ShowLevelCompleteUI(GameManager.instance.nextLevelToLoad + 1);
            }
        }
    }
}