using UnityEngine;

public class Enemy2Controller : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackRange = 1f;
    public int attackDamage = 20;
    public float attackCooldown = 1f;
    public float detectionRange = 5f; 

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;

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
        transform.localScale = new Vector3(9f, 9f, 9f);
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
                animator.SetBool("isRunning2", false);
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
        else if (isGrounded)
        {
            MoveTowardsPlayer();
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetBool("isRunning2", false);
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        animator.SetBool("isRunning2", true);
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(9, 9, 9);
        }
        else
        {
            transform.localScale = new Vector3(-9, 9, 9);
        }
    }

    void Attack()
    {
        isAttacking = true;
        hasDealtDamage = false;
        attackTimer = 0f;
        lastAttackTime = Time.time;
        rb.velocity = Vector2.zero;
        animator.SetBool("isRunning2", false);
        animator.SetTrigger("IsAttack2");
        Invoke("DealDamage", 0.5f);
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

    public void Hurt()
    {
        animator.SetTrigger("IsHurt2");
    }

    public void Die()
    {
        animator.SetTrigger("IsDie2");
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 1f);
    }

 
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}