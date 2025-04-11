using UnityEngine;
using System.Collections;

public class VerticalLazeTrap : MonoBehaviour
{
    public float cycleTime = 2f; 

    private Animator animator;
    private BoxCollider2D lazeCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        lazeCollider = GetComponent<BoxCollider2D>();
        StartCoroutine(LazeCycle());
    }

    IEnumerator LazeCycle()
    {
        while (true)
        {
            animator.SetBool("IsActive", true); 
            lazeCollider.enabled = true; 
            yield return new WaitForSeconds(cycleTime);

            animator.SetBool("IsActive", false); 
            lazeCollider.enabled = false; 
            yield return new WaitForSeconds(cycleTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && animator.GetBool("IsActive"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Die(); 
            }
        }
    }
}