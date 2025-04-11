using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Vector2 spawnPoint;

    void Start()
    {
        spawnPoint = transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.SetSpawnPoint(spawnPoint);
            }
        }
    }
}