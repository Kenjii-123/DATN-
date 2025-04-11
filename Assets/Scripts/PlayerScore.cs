using UnityEngine;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    public int gemCount = 0;
    public TextMeshProUGUI gemCountTextDisplay;

    void Start()
    {
        UpdateGemCountUI();
    }

    public void AddGem(int value)
    {
        gemCount += value;
        UpdateGemCountUI();
    }

    public void UpdateGemCountUI()
    {
        if (gemCountTextDisplay != null)
        {
            gemCountTextDisplay.text = gemCount.ToString();
        }
        else
        {
            Debug.LogError("Chưa gán TextMeshPro UGUI cho Gem Count!");
        }
    }
}