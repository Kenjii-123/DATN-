using UnityEngine;

public class BossController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackRange = 5f;
    public float shootRange = 8f;
    public float attackCooldown = 2f;
    public float detectionRange = 10f;
    public GameObject laserPrefab;
    public Transform laserSpawnPoint;
    public int attackDamage = 20;
    public int shootDamage = 15;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private BossHealth bossHealth;

    private bool isAttacking = false;
    private bool hasDealtDamage = false;
    private float attackTimer = 0f;
    private float lastAttackTime = -Mathf.Infinity;
    private bool isMoving = false;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bossHealth = GetComponent<BossHealth>();
        if (bossHealth == null)
        {
            Debug.LogError("BossHealth component not found on Boss!");
        }
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
        }
        if (!isAttacking)
        {
            if (isMoving)
            {
                MoveAndAttack();
            }
            else
            {
                rb.velocity = Vector2.zero;
                animator.SetBool("IsRun", false);
            }
        }
    }

    void MoveAndAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange && isGrounded && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
        else if (distanceToPlayer <= shootRange && isGrounded && Time.time >= lastAttackTime + attackCooldown)
        {
            Shoot();
        }
        else if (isGrounded)
        {
            MoveTowardsPlayer();
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetBool("IsRun", false);
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        animator.SetBool("IsRun", true);
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-12, 12, 9);
        }
        else
        {
            transform.localScale = new Vector3(12, 12, 9);
        }
    }

    void Attack()
    {
        isAttacking = true;
        hasDealtDamage = false;
        attackTimer = 0f;
        lastAttackTime = Time.time;
        rb.velocity = Vector2.zero;
        animator.SetBool("IsRun", false);
        animator.SetTrigger("IsAttack1");
        Invoke("DealDamage", 0.5f);
    }

    void Shoot()
    {
        isAttacking = true;
        hasDealtDamage = false;
        attackTimer = 0f;
        lastAttackTime = Time.time;
        rb.velocity = Vector2.zero;
        animator.SetBool("IsRun", false);
        animator.SetTrigger("IsShoot");
        Instantiate(laserPrefab, laserSpawnPoint.position, laserSpawnPoint.rotation);
        Invoke("DealShootDamage", 0.5f);
    }

    void DealDamage()
    {
        if (!hasDealtDamage)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
            hasDealtDamage = true;
        }
        isAttacking = false;
    }

    void DealShootDamage()
    {
        if (!hasDealtDamage)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(shootDamage);
            hasDealtDamage = true;
        }
        isAttacking = false;
    }

    public void Die()
    {
        animator.SetTrigger("IsDeath");
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 1f);
    }

    public void TakeDamage(int damage)
    {
        if (bossHealth != null)
        {
            bossHealth.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}