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

    public void Show(string itemName)
    {
        if (itemNameText != null)
        {
            itemNameText.text = itemName;
        }
        gameObject.SetActive(true); 
    }
}