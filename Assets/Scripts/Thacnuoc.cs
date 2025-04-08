using UnityEngine;

public class Thacnuoc : MonoBehaviour
{
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag(playerTag))
        {

            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Die();
            }
            else
            {
                Debug.LogError("PlayerHealth component not found on player!");
            }
        }
    }
}