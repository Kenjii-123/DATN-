using UnityEngine;
using TMPro;

public class ItemPickupNotification : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public float displayDuration = 5f;
    private float timer;

    void OnEnable()
    {
        timer = displayDuration;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void Show(string message) // Chỉ nhận một chuỗi thông báo
    {
        if (itemNameText != null)
        {
            itemNameText.text = message; // Gán trực tiếp thông báo vào text UI
        }
        gameObject.SetActive(true);
    }
}