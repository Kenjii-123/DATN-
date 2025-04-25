using UnityEngine;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    public int gemCount = 0;
    public TextMeshProUGUI gemCountTextDisplay;
    public AudioClip coinCollectSound;
    private AudioSource audioSource;

    void Start()
    {
        UpdateGemCountUI();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void AddGem(int value)
    {
        gemCount += value;
        UpdateGemCountUI();
        PlayCoinCollectSound();
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

    private void PlayCoinCollectSound()
    {
        if (coinCollectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(coinCollectSound);
        }
        else if (coinCollectSound == null)
        {
            Debug.LogWarning("Chưa gán AudioClip cho hiệu ứng thu thập coin!");
        }
        else if (audioSource == null)
        {
            Debug.LogWarning("AudioSource không tồn tại trên GameObject PlayerScore!");
        }
    }
}