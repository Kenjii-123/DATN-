using UnityEngine;

public class Enemy2Health : MonoBehaviour
{
    public int maxHealth = 3;

    private int currentHealth;
    private Enemy2Controller enemyController;

    void Start()
    {
        currentHealth = maxHealth;
        enemyController = GetComponent<Enemy2Controller>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        enemyController.Hurt();

        if (currentHealth <= 0)
        {
            enemyController.Die();
        }
    }
}