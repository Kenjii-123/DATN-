using UnityEngine;

public class SlimeController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float attackRange = 0.8f;
    public int attackDamage = 15;
    public float attackCooldown = 1.2f;
    public float detectionRange = 4f;
    public float moveAwayDistance = 1.5f;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private SlimeHealth slimeHealth;

    private bool isAttacking = false;
    private float attackTimer = 0f;
    private float lastAttackTime = -Mathf.Infinity;
    private bool canMove = true;

    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;
    private bool isGrounded;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        slimeHealth = GetComponent<SlimeHealth>();

        if (player == null) enabled = false;
        if (animator == null) enabled = false;
        if (rb == null) enabled = false;
        if (slimeHealth == null) enabled = false;

        animator.SetBool("isMove3", false);
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (slimeHealth != null && slimeHealth.isAlive)
        {
            if (distanceToPlayer <= detectionRange)
            {
                if (!isAttacking && canMove && isGrounded)
                {
                    MoveAndAttack();
                }
            }
            else
            {
                animator.SetBool("isMove3", false);
                rb.velocity = new Vector2(0, rb.velocity.y);
            }

            if (isAttacking)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackCooldown)
                {
                    isAttacking = false;
                    attackTimer = 0f;
                }
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isMove3", false);
        }
    }

    void MoveAndAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown && isGrounded)
        {
            Attack();
        }
        else if (distanceToPlayer > attackRange && isGrounded && canMove)
        {
            MoveTowardsPlayer();
        }
        else
        {
            animator.SetBool("isMove3", false);
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        animator.SetBool("isMove3", true);
        FlipSprite(direction.x);
    }

    void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        rb.velocity = Vector2.zero;
        animator.SetBool("isMove3", false);
        animator.SetTrigger("isAttack3");
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    public void Hurt()
    {
        animator.SetTrigger("isHurt3");
        if (player != null)
        {
            Vector2 directionAway = (transform.position - player.position).normalized;
            rb.velocity = new Vector2(directionAway.x * moveSpeed * 1.5f, rb.velocity.y);
            canMove = false;
            Invoke("EnableMovement", 0.3f);
        }
    }

    void EnableMovement()
    {
        canMove = true;
    }

    public void Die()
    {
        animator.SetTrigger("isDie3");
        rb.velocity = Vector2.zero;
        if (slimeHealth != null)
        {
            slimeHealth.isAlive = false;
        }
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 1f);
    }

    void FlipSprite(float directionX)
    {
        if (directionX > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (directionX < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
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