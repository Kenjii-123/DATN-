using UnityEngine;

public class EnemyFlyingAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform firePoint;
    public GameObject bulletPrefab;
    private Animator animator;
    private Rigidbody2D rb2D;

    [Header("Detection")]
    public float detectionRange = 5f;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stoppingDistance = 1.5f;

    [Header("Attack")]
    public float attackRange = 3f;
    public float attackRate = 1f;
    private float nextAttackTime = 0f;
    private bool canAttack = true;

    [Header("Health")]
    public int health = 3;
    private bool isDead = false;

    [Header("Rotation")]
    public float rotationSpeed = 5f;

    [Header("Altitude Control")]
    public float targetAltitude = 2f;

    [Header("Facing Player")]
    public bool useRotationToFace = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            if (player == null)
            {
                Debug.LogError("Không tìm thấy Player với tag 'Player'.");
                enabled = false;
            }
        }

        animator.SetBool("isIdle1", true);
        animator.SetBool("isFly1", false);

        Vector3 currentPosition = transform.position;
        currentPosition.y = targetAltitude;
        transform.position = currentPosition;
    }

    void Update()
    {
        if (player == null || isDead) return;

        Vector3 targetPositionXZ = new Vector3(player.position.x, targetAltitude, player.position.z);
        float distanceToPlayerXZ = Vector3.Distance(new Vector3(transform.position.x, targetAltitude, transform.position.z), targetPositionXZ);

        Vector3 directionToPlayerXZ = (targetPositionXZ - new Vector3(transform.position.x, targetAltitude, transform.position.z)).normalized;
        Vector2 directionToPlayerXZ2D = new Vector2(directionToPlayerXZ.x, directionToPlayerXZ.z);

        Vector3 fullDirectionToPlayer = (player.position - transform.position).normalized;

        if (useRotationToFace)
        {
            RotateTowards(directionToPlayerXZ2D);
        }
        else
        {
            FacePlayerUsingScaleX();
        }

        if (distanceToPlayerXZ <= detectionRange)
        {
            animator.SetBool("isIdle1", false);

            if (distanceToPlayerXZ > stoppingDistance)
            {
                animator.SetBool("isFly1", true);
                rb2D.velocity = new Vector2(directionToPlayerXZ.x, directionToPlayerXZ.z) * moveSpeed;
            }
            else
            {
                animator.SetBool("isFly1", false);
                rb2D.velocity = Vector2.zero;

                if (distanceToPlayerXZ <= attackRange && Time.time >= nextAttackTime && canAttack)
                {
                    animator.SetTrigger("isAttack1");
                    Attack(fullDirectionToPlayer);
                    nextAttackTime = Time.time + 1f / attackRate;
                    canAttack = false;
                    Invoke("ResetCanAttack", 0.1f);
                }
            }
        }
        else
        {
            animator.SetBool("isIdle1", true);
            animator.SetBool("isFly1", false);
            rb2D.velocity = Vector2.zero;
        }

        if (health < 3 && !isDead)
        {
            animator.SetTrigger("isHurt1");
            health = 3;
        }

        if (health <= 0 && !isDead)
        {
            isDead = true;
            animator.SetTrigger("isDeath1");
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, 2f);
        }

        Vector3 currentPosition = transform.position;
        currentPosition.y = targetAltitude;
        transform.position = currentPosition;
    }

    void RotateTowards(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            // Thử đảo dấu góc
            float targetAngle = -Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void FacePlayerUsingScaleX()
    {
        if (player != null)
        {
            float relativePositionX = player.position.x - transform.position.x;

            // Đảo ngược điều kiện lật
            if (relativePositionX > 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (relativePositionX < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    void Attack(Vector3 fireDirection)
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                float bulletSpeed = 5f;
                bulletRb.velocity = fireDirection * bulletSpeed;
            }
            Destroy(bullet, 5f);
        }
        else
        {
            Debug.LogError("Chưa gán Bullet Prefab hoặc Fire Point cho Enemy.");
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (!isDead)
        {
            health -= damageAmount;
        }
    }

    void ResetCanAttack()
    {
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}