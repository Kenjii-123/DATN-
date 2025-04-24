using UnityEngine;

public class GolemController : MonoBehaviour
{
    [Header("Thông số di chuyển")]
    public float moveSpeed = 3f;
    public float detectionRange = 10f;

    [Header("Thông số tấn công")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 20;
    private float lastAttackTime = -Mathf.Infinity;

    [Header("Thông số phân thân")]
    public bool canSplit = true;
    public float healthThresholdForSplit = 0.5f;
    public GameObject golemClonePrefab;
    public Transform[] cloneSpawnPoints;
    public float splitDelay = 0.8f; // Thời gian chờ trước khi thực sự phân thân
    private bool isSplitting = false;

    [Header("Kiểm tra mặt đất")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private GolemHealth golemHealth;
    private bool isAlive = true;
    private bool isChasing = false;
    private bool isCollidingWithPlayer = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        golemHealth = GetComponent<GolemHealth>();

        if (golemHealth == null)
        {
            Debug.LogError("GolemHealth component not found on Golem!");
        }

        if (player == null)
        {
            Debug.LogError("Không tìm thấy Player với tag 'Player'!");
        }
    }

    void Update()
    {
        if (!isAlive || player == null) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("IsGrounded", isGrounded);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
            animator.SetBool("IsGrun", false);
            rb.velocity = Vector2.zero;
        }

        if (isChasing && isGrounded && !isCollidingWithPlayer)
        {
            MoveTowardsPlayer();
        }
        else if (isCollidingWithPlayer && isGrounded && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
        else
        {
            animator.SetBool("IsGrun", false);
            rb.velocity = Vector2.zero;
        }

        // Kiểm tra điều kiện phân thân
        if (canSplit && !isSplitting && (float)golemHealth.currentHealth / golemHealth.maxHealth <= healthThresholdForSplit)
        {
            Split();
        }
    }

    void MoveTowardsPlayer()
    {
        animator.SetBool("IsGrun", true);
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        UpdateFacingDirection(direction.x);
    }

    void StandIdle()
    {
        animator.SetBool("IsGrun", false);
        rb.velocity = Vector2.zero;
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        animator.SetTrigger("IsGattack");
        rb.velocity = Vector2.zero;
        // Gây sát thương thông qua Animation Event hoặc Invoke
        Invoke("DealDamage", 0.5f); // Ví dụ gọi sau 0.5s animation
    }

    void DealDamage()
    {
        if (isAlive && player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                animator.SetTrigger("IsGhurt");
            }
        }
    }

    void Split()
    {
        if (golemClonePrefab != null && cloneSpawnPoints.Length >= 2)
        {
            isSplitting = true;
            animator.SetTrigger("IsSplit");
            rb.velocity = Vector2.zero;
            Invoke("ActuallySplit", splitDelay);
        }
        else
        {
            Debug.LogWarning("Golem Clone Prefab chưa được gán hoặc không đủ điểm sinh bản sao!");
        }
    }

    void ActuallySplit()
    {
        if (isAlive && golemClonePrefab != null && cloneSpawnPoints.Length >= 2)
        {
            Instantiate(golemClonePrefab, cloneSpawnPoints[0].position, transform.rotation);
            Instantiate(golemClonePrefab, cloneSpawnPoints[1].position, transform.rotation);
            golemHealth.TakeDamage(golemHealth.maxHealth / 2); // Giảm máu golem gốc
            canSplit = false; // Chỉ phân thân một lần
            isSplitting = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive) return;
        golemHealth.TakeDamage(damage);
        animator.SetTrigger("IsGhurt");
        if (golemHealth.currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isAlive = false;
        animator.SetTrigger("IsGdie");
        rb.velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 1f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
        }
    }

    void UpdateFacingDirection(float directionX)
    {
        if (directionX > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (directionX < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}