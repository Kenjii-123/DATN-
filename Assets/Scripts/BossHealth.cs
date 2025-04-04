using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Boss nhận sát thương: " + damage + ", máu còn lại: " + currentHealth);

        if (currentHealth <= 0)
        {
            GetComponent<BossController>().Die();
        }
    }
}