using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IncorrectAnswerPanelUI : MonoBehaviour
{
    public TMP_Text incorrectQuestionText;
    public TMP_Text correctAnswerText;
    public Button nextButton;
    private Chest chestScript;

    public void SetIncorrectAnswer(Question question, string correctAnswer, Chest chest)
    {
        if (incorrectQuestionText != null)
        {
            incorrectQuestionText.text = question.questionText;
        }
        else
        {
            Debug.LogError("TMP_Text cho câu hỏi sai chưa được gán!");
        }

        if (correctAnswerText != null)
        {
            correctAnswerText.text =  correctAnswer;
        }
        else
        {
            Debug.LogError("TMP_Text cho câu trả lời đúng chưa được gán!");
        }

        chestScript = chest;
        if (nextButton != null && chestScript != null)
        {
            nextButton.onClick.AddListener(chestScript.ContinueAfterIncorrectAnswer);
        }
        else
        {
            Debug.LogError("Button Next hoặc Chest script chưa được gán!");
        }
    }

    private void OnDestroy()
    {
        if (nextButton != null && chestScript != null)
        {
            nextButton.onClick.RemoveListener(chestScript.ContinueAfterIncorrectAnswer);
        }
    }
}