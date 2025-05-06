using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using TMPro;

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
    public GameObject interactionUIPrefab;
    public Transform notificationParent;
    public GameObject incorrectAnswerPanelPrefab;
    public Transform incorrectAnswerPanelParent;

    private bool playerInRange = false;
    private Animator animator;
    public int correctAnswers = 0;
    private bool chestOpened = false;
    private GameObject currentQuestionPanelInstance;
    private List<Question> currentQuestionsToAsk;
    private Question currentQuestion;
    public int currentQuestionIndex = 0;
    private GameManager gameManager;
    private GameObject currentInteractionUIInstance;
    private List<Question> incorrectlyAnsweredQuestions = new List<Question>();
    private GameObject currentIncorrectAnswerPanelInstance;
    private int incorrectQuestionIndex = 0;
    private bool showingIncorrectAnswers = false;
    private bool finishedShowingIncorrectAnswers = false;

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

        FindNotificationParent();
        if (incorrectAnswerPanelParent == null)
        {
            incorrectAnswerPanelParent = notificationParent;
            Debug.LogWarning("Incorrect Answer Panel Parent không được gán, sử dụng Notification Parent làm mặc định.");
        }
    }

    void FindNotificationParent()
    {
        if (notificationParent == null)
        {
            Canvas mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas != null)
            {
                notificationParent = mainCanvas.transform;
            }
            else
            {
                Debug.LogError("Không tìm thấy Canvas chính trong Scene!");
            }
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
        if (collision.CompareTag("Player") && !chestOpened)
        {
            playerInRange = true;
            ShowInteractionUI();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            HideInteractionUI();
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
            HideInteractionUI();
        }
        else
        {
            ShowInsufficientGemsUI();
        }
    }

    void OpenChest()
    {
        correctAnswers = 0;
        currentQuestionIndex = 0;
        incorrectlyAnsweredQuestions.Clear();
        chestOpened = true;
        animator.SetTrigger("IsOpen");
        Invoke("ShowQuestionPanel", questionPanelShowDelay);
    }

    void ShowQuestionPanel()
    {
        if (questionPanelPrefab != null && questionPanelParent != null && currentQuestionsToAsk.Count > 0 && currentQuestionIndex < currentQuestionsToAsk.Count && !showingIncorrectAnswers)
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
        else if (!showingIncorrectAnswers)
        {
            Debug.Log("Đã trả lời hết số câu hỏi yêu cầu từ chest này. Hiển thị các câu trả lời sai.");
            ShowIncorrectAnswerSequence();
        }
    }

    public void CheckAnswer(bool isCorrect)
    {
        if (currentQuestionPanelInstance != null)
        {
            Destroy(currentQuestionPanelInstance);
            currentQuestionPanelInstance = null;
        }

        if (!showingIncorrectAnswers)
        {
            if (isCorrect)
            {
                correctAnswers++;
                SaveProgress();
                Debug.Log("Correct answers: " + correctAnswers + "/" + currentQuestionsToAsk.Count);
            }
            else
            {
                Debug.Log("Incorrect answer. Correct answers: " + correctAnswers + "/" + currentQuestionsToAsk.Count);
                incorrectlyAnsweredQuestions.Add(currentQuestion);
            }

            currentQuestionIndex++;
            if (currentQuestionIndex < currentQuestionsToAsk.Count)
            {
                Invoke("ShowQuestionPanel", questionPanelShowDelay);
            }
            else
            {
                Debug.Log("Đã trả lời hết số câu hỏi yêu cầu từ chest này. Hiển thị các câu trả lời sai.");
                ShowIncorrectAnswerSequence();
            }
        }
    }

    void ShowIncorrectAnswerSequence()
    {
        showingIncorrectAnswers = true;
        incorrectQuestionIndex = 0;
        if (incorrectlyAnsweredQuestions.Count > 0)
        {
            ShowIncorrectAnswerPanel(incorrectlyAnsweredQuestions[incorrectQuestionIndex]);
        }
        else
        {
            Debug.Log("Không có câu trả lời sai nào.");
            finishedShowingIncorrectAnswers = true;
            HandleChestCompletion();
        }
    }

    void ShowIncorrectAnswerPanel(Question incorrectQuestion)
    {
        if (incorrectAnswerPanelPrefab != null && incorrectAnswerPanelParent != null)
        {
            currentIncorrectAnswerPanelInstance = Instantiate(incorrectAnswerPanelPrefab, incorrectAnswerPanelParent);
            IncorrectAnswerPanelUI incorrectPanelUI = currentIncorrectAnswerPanelInstance.GetComponent<IncorrectAnswerPanelUI>();
            if (incorrectPanelUI != null)
            {
                incorrectPanelUI.SetIncorrectAnswer(incorrectQuestion, incorrectQuestion.answers[incorrectQuestion.correctAnswerIndex], this);
            }
            else
            {
                Debug.LogError("IncorrectAnswerPanelUI script không được tìm thấy trên prefab bảng câu trả lời sai.");
            }
        }
        else
        {
            Debug.LogError("Incorrect Answer Panel Prefab hoặc Parent chưa được gán.");
            ContinueAfterIncorrectAnswer();
        }
    }

    public void ContinueAfterIncorrectAnswer()
    {
        if (currentIncorrectAnswerPanelInstance != null)
        {
            Destroy(currentIncorrectAnswerPanelInstance);
            currentIncorrectAnswerPanelInstance = null;
        }

        incorrectQuestionIndex++;
        if (incorrectQuestionIndex < incorrectlyAnsweredQuestions.Count)
        {
            ShowIncorrectAnswerPanel(incorrectlyAnsweredQuestions[incorrectQuestionIndex]);
        }
        else
        {
            Debug.Log("Đã xem hết các câu trả lời sai.");
            finishedShowingIncorrectAnswers = true;
            HandleChestCompletion();
        }
    }

    void HandleChestCompletion()
    {
        if (finishedShowingIncorrectAnswers)
        {
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
        else
        {
            Debug.Log($"Chưa xem hết câu sai: finishedShowingIncorrectAnswers = {finishedShowingIncorrectAnswers}");
        }
    }

    void ShowInteractionUI()
    {
        if (interactionUIPrefab != null && notificationParent != null && currentInteractionUIInstance == null)
        {
            currentInteractionUIInstance = Instantiate(interactionUIPrefab, notificationParent);
            TextMeshProUGUI notificationText = currentInteractionUIInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (notificationText != null)
            {
                notificationText.text = $"Bấm {KeyCode.O.ToString().ToUpper()} để mở rương. (Cần {gemsNeededToOpen} gem)";
            }
            else
            {
                Debug.LogError("Không tìm thấy TextMeshProUGUI trong Interaction UI Prefab!");
            }
        }
    }

    void HideInteractionUI()
    {
        if (currentInteractionUIInstance != null)
        {
            Destroy(currentInteractionUIInstance);
            currentInteractionUIInstance = null;
        }
    }

    void ShowInsufficientGemsUI()
    {
        if (interactionUIPrefab != null && notificationParent != null)
        {
            GameObject insufficientGemsUI = Instantiate(interactionUIPrefab, notificationParent);
            TextMeshProUGUI notificationText = insufficientGemsUI.GetComponentInChildren<TextMeshProUGUI>();
            if (notificationText != null)
            {
                notificationText.text = "Không đủ Gem, hãy thu thập thêm!";
            }
            else
            {
                Debug.LogError("Không tìm thấy TextMeshProUGUI trong Interaction UI Prefab!");
            }
            Destroy(insufficientGemsUI, 2f);
        }
    }
}