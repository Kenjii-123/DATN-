using UnityEngine;

public class GemPickup : MonoBehaviour
{
    public int gemValue = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CollectGem(other.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectGem(other.gameObject);
        }
    }

    void CollectGem(GameObject player)
    {
        PlayerScore playerScore = player.GetComponent<PlayerScore>();
        if (playerScore != null)
        {
            playerScore.AddGem(gemValue);
        }
        else
        {
            Debug.LogError("Không tìm thấy script PlayerScore trên đối tượng Player!");
        }

        Destroy(gameObject);
    }
}