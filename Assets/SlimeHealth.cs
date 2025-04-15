using UnityEngine;

public class SlimeHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public bool isAlive = true;

    private int currentHealth;
    private SlimeController slimeController;

    void Start()
    {
        currentHealth = maxHealth;
        slimeController = GetComponent<SlimeController>();
        if (slimeController == null)
        {
            Debug.LogError("Không tìm thấy SlimeController!");
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;
        if (slimeController != null)
        {
            slimeController.Hurt();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isAlive = false;
        if (slimeController != null)
        {
            slimeController.Die();
        }
    }
}