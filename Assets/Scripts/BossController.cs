using UnityEngine;

public class BossController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackRange = 5f;
    public float attack1Cooldown = 2f;
    public float attack2Cooldown = 3f;
    public float detectionRange = 10f;
    public int attack1Damage = 20;
    public int attack2Damage = 25;

    [Header("Summoning")]
    public GameObject minionPrefab;
    public Transform[] summonPoints;
    public float summonCooldown = 5f;
    public int maxMinions = 2; 
    private float nextSummonTime = 0f;
    private int currentMinionCount = 0;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private BossHealth bossHealth;

    private bool isAttacking = false;
    private bool hasDealtDamage = false;
    private float attackTimer = 0f;
    private float lastAttack1Time = -Mathf.Infinity;
    private float lastAttack2Time = -Mathf.Infinity;
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
        float currentHealthPercentage = (float)bossHealth.currentHealth / bossHealth.maxHealth;

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

        if (!isAttacking && isMoving && isGrounded)
        {
            if (currentHealthPercentage >= 1f)
            {
                if (distanceToPlayer <= attackRange && Time.time >= lastAttack1Time + attack1Cooldown)
                {
                    Attack1();
                }
                else
                {
                    MoveTowardsPlayer();
                }
            }
            else if (currentHealthPercentage < 0.5f) 
            {
                if (distanceToPlayer <= attackRange && Time.time >= lastAttack2Time + attack2Cooldown)
                {
                    Attack2();
                }
                else if (Time.time >= nextSummonTime && currentMinionCount < maxMinions && minionPrefab != null && summonPoints.Length > 0)
                {
                    SummonMinion();
                    nextSummonTime = Time.time + summonCooldown;
                }
                else
                {
                    MoveTowardsPlayer();
                }
            }
            else 
            {
                if (distanceToPlayer <= attackRange && Time.time >= lastAttack1Time + attack1Cooldown)
                {
                    Attack1();
                }
                else if (distanceToPlayer <= attackRange && Time.time >= lastAttack2Time + attack2Cooldown)
                {
                    Attack2();
                }
                else
                {
                    MoveTowardsPlayer();
                }
            }
        }
        else if (!isMoving)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("IsRun", false);
        }
        else if (!isGrounded)
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
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void Attack1()
    {
        isAttacking = true;
        hasDealtDamage = false;
        attackTimer = 0f;
        lastAttack1Time = Time.time;
        rb.velocity = Vector2.zero;
        animator.SetBool("IsRun", false);
        animator.SetTrigger("IsAttack1");
        Invoke("DealAttack1Damage", 0.5f);
    }

    void Attack2()
    {
        isAttacking = true;
        hasDealtDamage = false;
        attackTimer = 0f;
        lastAttack2Time = Time.time;
        rb.velocity = Vector2.zero;
        animator.SetBool("IsRun", false);
        animator.SetTrigger("IsAttack2");
        Invoke("DealAttack2Damage", 0.6f); 
    }

    void DealAttack1Damage()
    {
        if (!hasDealtDamage && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attack1Damage);
            }
            hasDealtDamage = true;
        }
        isAttacking = false;
    }

    void DealAttack2Damage()
    {
        if (!hasDealtDamage && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attack2Damage);
            }
            hasDealtDamage = true;
        }
        isAttacking = false;
    }

    void SummonMinion()
    {
        if (minionPrefab != null && summonPoints.Length > 0)
        {
            animator.SetTrigger("IsSummon"); 
            rb.velocity = Vector2.zero;
            animator.SetBool("IsRun", false);
            isAttacking = true; 

            Invoke("ActuallySummonMinion", 0.8f); 
            nextSummonTime = Time.time + summonCooldown;
        }
        else
        {
            Debug.LogWarning("Minion Prefab không được gán hoặc không có điểm triệu hồi!");
        }
    }

    void ActuallySummonMinion()
    {
        if (minionPrefab != null && summonPoints.Length > 0 && currentMinionCount < maxMinions)
        {
            int randomSummonPointIndex = Random.Range(0, summonPoints.Length);
            Transform summonPoint = summonPoints[randomSummonPointIndex];
            Instantiate(minionPrefab, summonPoint.position, Quaternion.identity);
            currentMinionCount++;
        }
        isAttacking = false;
    }

    public void MinionDied()
    {
        currentMinionCount--;
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}