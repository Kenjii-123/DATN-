using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Vector2 spawnPoint;
    public AudioClip checkpointSound;
    private AudioSource audioSource;
    private bool checkpointReached = false;

    void Start()
    {
        spawnPoint = transform.position;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !checkpointReached)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.SetSpawnPoint(spawnPoint);
                PlayCheckpointSound();
                checkpointReached = true;
            }
        }
    }

    void PlayCheckpointSound()
    {
        if (checkpointSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(checkpointSound);
        }
        else if (checkpointSound == null)
        {
            Debug.LogWarning("Chưa gán AudioClip cho hiệu ứng checkpoint!");
        }
        else if (audioSource == null)
        {
            Debug.LogWarning("AudioSource không tồn tại trên GameObject Checkpoint!");
        }
    }
}