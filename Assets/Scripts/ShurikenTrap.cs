using UnityEngine;

public class ShurikenTrap : MonoBehaviour
{
    public Transform pointA; 
    public Transform pointB; 
    public float speed = 5f;
    public float tocDoXoay = 360f; 

    private Vector3 target;

    void Start()
    {
        target = pointB.position;
    }

    void Update()
    {
     
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        transform.Rotate(0, 0, tocDoXoay * Time.deltaTime);

        if (transform.position == pointB.position)
        {
            target = pointA.position;
        }
        if (transform.position == pointA.position)
        {
            target = pointB.position;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Die(); 
            }
        }
    }
}