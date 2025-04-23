using UnityEngine;

public class GolemHealth: MonoBehaviour
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
        Debug.Log("Golem nhận sát thương: " + damage + ", máu còn lại: " + currentHealth);

        if (currentHealth <= 0)
        {
            GetComponent<GolemController>().Die();
        }
    }
}