using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 30;
    public float lifeTime = 0.1f;
    public LayerMask enemyLayer;
    [HideInInspector] public Vector3 direction = Vector3.right;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((enemyLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            Enemy2Health enemyHealth = other.GetComponent<Enemy2Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}