using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class QuestionPanelUI : MonoBehaviour
{
    public TMP_Text questionTextUI;
    public List<Button> answerButtons;

    private Question currentQuestion;
    private Chest chestScript;

    public void SetQuestion(Question question, Chest chest)
    {
        currentQuestion = question;
        chestScript = chest;

        if (questionTextUI != null)
        {
            questionTextUI.text = currentQuestion.questionText;
        }

        if (answerButtons.Count == currentQuestion.answers.Count)
        {
            for (int i = 0; i < answerButtons.Count; i++)
            {
                int answerIndex = i; 
                TMP_Text buttonText = answerButtons[i].GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = currentQuestion.answers[i];
                }
                answerButtons[i].onClick.RemoveAllListeners(); // Đảm bảo không có listeners cũ
                answerButtons[i].onClick.AddListener(() => chestScript.CheckAnswer(answerIndex == currentQuestion.correctAnswerIndex));
            }
        }
        else
        {
            Debug.LogError("Số lượng nút trả lời không khớp với số lượng đáp án trong QuestionPanelUI.");
        }
    }
}