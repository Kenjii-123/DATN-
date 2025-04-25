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

    private PlayerMovement playerMovement;

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

        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("Không tìm thấy script PlayerMovement trên cùng GameObject!");
        }
    }

    void Update()
    {
    }

    public void TakeDamage(int damage)
    {
        if (playerMovement != null && playerMovement.isGiantArmorActive)
        {
            playerMovement.playerAudioSource.PlayOneShot(playerMovement.takeDamageSound);
            return;
        }
        else
        {
            currentHealth -= damage;
            UpdateHealthUI();
            if (playerMovement != null && playerMovement.takeDamageSound != null)
            {
                playerMovement.playerAudioSource.PlayOneShot(playerMovement.takeDamageSound);
            }

            if (animator != null)
            {
                animator.SetTrigger("isHurt");
            }

            if (currentHealth <= 0)
            {
                Die();
            }
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
        if (playerMovement != null && playerMovement.dieSound != null)
        {
            playerMovement.playerAudioSource.PlayOneShot(playerMovement.dieSound);
        }

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
        if (playerMovement != null && playerMovement.reviveSound != null)
        {
            playerMovement.playerAudioSource.PlayOneShot(playerMovement.reviveSound);
        }
    }

    public void SetSpawnPoint(Vector2 newSpawnPoint)
    {
        currentSpawnPoint = newSpawnPoint;
    }
}