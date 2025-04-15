using UnityEngine;

public class SandTrap : MonoBehaviour
{
    public int damageAmount = 20;
    public LayerMask playerLayer;
    public string isActiveParameterName = "isActive";
    public float activeDuration = 2f;
    public float inactiveDuration = 3f;
    private Animator animator;
    private Collider2D trapCollider;
    private bool isCurrentlyActive = false;
    private float timer = 0f;
    private bool trapOn = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Trap cần có một Animator component!");
            enabled = false;
            return;
        }

        trapCollider = GetComponent<Collider2D>();
        if (trapCollider == null)
        {
            Debug.LogError("Trap cần có một Collider2D!");
            enabled = false;
            return;
        }

        trapOn = false;
        animator.SetBool(isActiveParameterName, false);
        trapCollider.enabled = false;
        isCurrentlyActive = false;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (trapOn)
        {
            if (timer >= activeDuration)
            {
                trapOn = false;
                animator.SetBool(isActiveParameterName, false);
                trapCollider.enabled = false;
                timer = 0f;
                isCurrentlyActive = false;
            }
        }
        else
        {
            if (timer >= inactiveDuration)
            {
                trapOn = true;
                animator.SetBool(isActiveParameterName, true);
                trapCollider.enabled = true;
                timer = 0f;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (trapOn && !isCurrentlyActive && (playerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                isCurrentlyActive = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if ((playerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            isCurrentlyActive = false;
        }
    }

    public void SetTrapActive(bool active)
    {
        trapOn = active;
        animator.SetBool(isActiveParameterName, active);
        trapCollider.enabled = active;
        timer = 0f;
        isCurrentlyActive = false;
    }
}