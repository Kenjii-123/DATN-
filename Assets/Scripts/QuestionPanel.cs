using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionPanel : MonoBehaviour
{
    public TextMeshProUGUI questionText; 
    public Button[] answerButtons; 
    public GameObject keyImage; 

    
    private string[] questions = { "Câu hỏi 1", "Câu hỏi 2", "Câu hỏi 3" };
    private string[][] answers = {
        new string[] { "Đáp án 1.1", "Đáp án 1.2", "Đáp án 1.3", "Đáp án 1.4" },
        new string[] { "Đáp án 2.1", "Đáp án 2.2", "Đáp án 2.3", "Đáp án 2.4" },
        new string[] { "Đáp án 3.1", "Đáp án 3.2", "Đáp án 3.3", "Đáp án 3.4" }
    };
    private int[] correctAnswers = { 0, 1, 2 }; 

   
    private int currentQuestionIndex = 0; 
    private int correctAnswersCount = 0; 

    void Start()
    {
    
        ShowQuestion();
        keyImage.SetActive(false);
    }

    void ShowQuestion()
    {
   
        if (currentQuestionIndex < questions.Length)
        {
            questionText.text = questions[currentQuestionIndex];
            for (int i = 0; i < answerButtons.Length; i++)
            {
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = answers[currentQuestionIndex][i];
                int answerIndex = i;

               
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => CheckAnswer(answerIndex));
            }
        }
        else
        {
            
            Debug.Log("Kết thúc chuỗi câu hỏi!");
            if (correctAnswersCount == questions.Length)
            {
                keyImage.SetActive(true);
                Debug.Log("Bạn đã nhận được chìa khóa!");
            }
            else
            {
                Debug.Log("Bạn cần trả lời đúng tất cả các câu hỏi!");
            }
        }
    }

   
    public void CheckAnswer(int answerIndex)
    {
     
        if (answerIndex == correctAnswers[currentQuestionIndex])
        {
            Debug.Log("Đáp án đúng!");
            correctAnswersCount++;
        }
        else
        {
            Debug.Log("Đáp án sai!");
        }

        currentQuestionIndex++;
        ShowQuestion();
    }
}