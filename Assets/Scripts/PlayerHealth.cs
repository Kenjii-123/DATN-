using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Animator animator;
    public Collider2D playerCollider;
    public int maxHealth = 100;
    public int currentHealth;
    [HideInInspector] public Vector2 currentSpawnPoint;
    public GameObject startPointObject;
    public GameObject explosionPrefab;
    public float respawnDelay = 1f;
    public Image healthBarFill;

    void Start()
    {
        currentHealth = maxHealth;
        if (startPointObject != null)
        {
            currentSpawnPoint = startPointObject.transform.position;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy GameObject Start Point! Điểm hồi sinh sẽ là vị trí ban đầu của Player.");
            currentSpawnPoint = transform.position;
        }
        UpdateHealthUI();
    }

    void Update()
    {
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthUI();

        if (animator != null)
        {
            animator.SetTrigger("isHurt");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            float fillAmount = (float)currentHealth / maxHealth;
            healthBarFill.fillAmount = fillAmount;
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
        UpdateHealthUI();
    }

    public void SetSpawnPoint(Vector2 newSpawnPoint)
    {
        currentSpawnPoint = newSpawnPoint;
    }
}