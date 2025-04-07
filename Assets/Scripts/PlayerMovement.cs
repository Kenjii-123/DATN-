using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 20;
    public LayerMask enemyLayer;

    public GameObject doubleJumpSmokePrefab;
    public Transform smokeSpawnPoint;

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool canDoubleJump;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        rb.velocity = movement;

        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(12, 12, 1);
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-12, 12, 1);
        }

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool("isJumping", true);
            canDoubleJump = true;
        }
        else if (!isGrounded && canDoubleJump && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("doubleJumpTrigger");
            canDoubleJump = false;

            if (doubleJumpSmokePrefab != null && smokeSpawnPoint != null)
            {
                Instantiate(doubleJumpSmokePrefab, smokeSpawnPoint.position, Quaternion.identity);
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Attack();
        }

        UpdateAnimation();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("djump"))
        {
            canDoubleJump = true;
        }
    }

    void Attack()
    {
        animator.SetTrigger("attackTrigger");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            Enemy2Health enemyHealth = enemy.GetComponent<Enemy2Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        if (attackPoint == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(rb.velocity.x) > 0.1f;
        bool isJumping = !isGrounded;
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
    }
}