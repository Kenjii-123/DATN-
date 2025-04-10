using UnityEngine;
using System.Collections;

public class FallTrap : MonoBehaviour
{
    public float fallDelay = 1f;
    public float respawnDelay = 5f;

    private Rigidbody2D rb;
    private Vector2 originalPosition;
    private bool isFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalPosition = transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Fall());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isFalling)
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Die();
            }
        }
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);
        rb.isKinematic = false;
        isFalling = true;
        yield return new WaitForSeconds(respawnDelay);
        ResetTrap();
    }

    void ResetTrap()
    {
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        transform.position = originalPosition;
        isFalling = false;
    }
}