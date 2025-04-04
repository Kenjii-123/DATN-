using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public Animator animator;
    public Collider2D playerCollider;
    public int maxHealth = 100;
    public int currentHealth;
    public Vector2 currentSpawnPoint;
    public GameObject explosionPrefab;
    public float respawnDelay = 1f;

    void Start()
    {
        currentHealth = maxHealth;
        currentSpawnPoint = transform.position;
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player nhận sát thương: " + damage + ", máu còn lại: " + currentHealth);

        if (animator != null)
        {
            animator.SetTrigger("isHurt"); 
        }
    }

    public void Die()
    {
        gameObject.SetActive(false);

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Invoke("Respawn", respawnDelay);
    }

    void Respawn()
    {
        transform.position = currentSpawnPoint;
        currentHealth = maxHealth;
        gameObject.SetActive(true);
    }

    public void SetSpawnPoint(Vector2 newSpawnPoint)
    {
        currentSpawnPoint = newSpawnPoint;
    }
}