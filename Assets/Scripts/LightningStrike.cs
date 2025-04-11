using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    public float damage = 20f;
    public string enemyTag = "Enemy";
    public float attackRange = 1f; // Bán kính vùng ảnh hưởng của tia sét
    public float destroyDelay = 0.2f; // Thời gian tồn tại của hiệu ứng tia sét
    public Transform attackPoint; // Điểm trung tâm của vùng ảnh hưởng

    void Start()
    {
        // Nếu không có attackPoint được gán, sử dụng vị trí của chính GameObject
        if (attackPoint == null)
        {
            attackPoint = transform;
        }

        // Gây sát thương lên kẻ địch khi tia sét được tạo
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, LayerMask.GetMask(enemyTag));

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag(enemyTag))
            {
                Enemy2Health enemyHealth = enemyCollider.GetComponent<Enemy2Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage((int)damage);
                }
            }
        }

        // Tự hủy sau một khoảng thời gian ngắn (thời gian hiển thị hiệu ứng)
        Destroy(gameObject, destroyDelay);
    }

    // (Tùy chọn) Vẽ Gizmos để dễ nhìn thấy vùng ảnh hưởng trong Editor
    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}