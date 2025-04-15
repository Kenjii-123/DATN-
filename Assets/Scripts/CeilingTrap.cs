using UnityEngine;

public class CeilingTrap : MonoBehaviour
{
    public int damageAmount = 50;
    public LayerMask playerLayer;
    public string isActiveParameterName = "IsActive";
    public float activeDuration = 1.5f;
    public float inactiveDuration = 2.5f;
    public Transform damagePoint;
    public float damageCheckRadius = 0.3f;
    private Animator animator;
    private Collider2D trapCollider;
    private float timer = 0f;
    private bool isActive = false;
    private bool hasDealtDamage = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("CeilingTrap cần có một Animator component!");
            enabled = false;
            return;
        }

        trapCollider = GetComponent<Collider2D>();
        if (trapCollider != null)
        {
            trapCollider.enabled = false;
        }

        isActive = false;
        animator.SetBool(isActiveParameterName, false);
        timer = 0f;
        hasDealtDamage = false;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (isActive)
        {
            if (timer >= activeDuration)
            {
                isActive = false;
                animator.SetBool(isActiveParameterName, false);
                if (trapCollider != null)
                {
                    trapCollider.enabled = false;
                }
                timer = 0f;
                hasDealtDamage = false;
            }
            else
            {
                CheckAndDealDamage();
            }
        }
        else
        {
            if (timer >= inactiveDuration)
            {
                isActive = true;
                animator.SetBool(isActiveParameterName, true);
                if (trapCollider != null)
                {
                    trapCollider.enabled = true;
                }
                timer = 0f;
                hasDealtDamage = false;
            }
        }
    }

    void CheckAndDealDamage()
    {
        if (damagePoint != null && !hasDealtDamage)
        {
            Collider2D hitPlayer = Physics2D.OverlapCircle(damagePoint.position, damageCheckRadius, playerLayer);
            if (hitPlayer != null)
            {
                PlayerHealth playerHealth = hitPlayer.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageAmount);
                    hasDealtDamage = true;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (damagePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(damagePoint.position, damageCheckRadius);
        }
    }

    public void SetTrapActive(bool active)
    {
        isActive = active;
        animator.SetBool(isActiveParameterName, active);
        if (trapCollider != null)
        {
            trapCollider.enabled = active;
        }
        timer = 0f;
        hasDealtDamage = false;
    }
}