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
    public GameObject speedBoostEffectPrefab; // Prefab hiệu ứng tăng tốc
    public Transform speedBoostEffectSpawnPoint; // Điểm tạo hiệu ứng tăng tốc

    private bool canDash = false;
    private bool isDashing = false;
    public float dashSpeedMultiplier = 2f;
    public float dashDuration = 3f;
    private float dashTimer;
    private float normalMoveSpeed;

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool canDoubleJump = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalMoveSpeed = moveSpeed;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        if (isDashing)
        {
            movement.x *= dashSpeedMultiplier;
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
                moveSpeed = normalMoveSpeed;
                // Hủy hiệu ứng tăng tốc khi hết thời gian
                if (speedBoostEffectPrefab != null && speedBoostEffectSpawnPoint != null)
                {
                    foreach (Transform child in speedBoostEffectSpawnPoint)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }
        else
        {
            moveSpeed = normalMoveSpeed;
        }

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
            canDoubleJump = false;
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool("isJumping", true);
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

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
        {
            isDashing = true;
            dashTimer = dashDuration;
            // Tạo hiệu ứng tăng tốc khi bắt đầu dash
            if (speedBoostEffectPrefab != null && speedBoostEffectSpawnPoint != null)
            {
                Instantiate(speedBoostEffectPrefab, speedBoostEffectSpawnPoint.position, speedBoostEffectPrefab.transform.rotation, speedBoostEffectSpawnPoint);
            }
        }

        UpdateAnimation();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("djump"))
        {
            canDoubleJump = true;
        }
        if (other.CompareTag("DashItem") && !isDashing)
        {
            canDash = true;
            isDashing = true;
            dashTimer = dashDuration;
            normalMoveSpeed = moveSpeed;
            // Tạo hiệu ứng tăng tốc khi thu thập vật phẩm và bắt đầu dash
            if (speedBoostEffectPrefab != null && speedBoostEffectSpawnPoint != null)
            {
                Instantiate(speedBoostEffectPrefab, speedBoostEffectSpawnPoint.position, speedBoostEffectPrefab.transform.rotation, speedBoostEffectSpawnPoint);
            }
            Destroy(other.gameObject);
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