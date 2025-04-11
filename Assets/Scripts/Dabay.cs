using UnityEngine;

public class Dabay : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 5f;

    private Vector3 targetPosition;
    private GameObject standingPlayer;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("Point A or Point B is not assigned!");
            enabled = false;
            return;
        }

        targetPosition = pointB.position;
    }

    void Update()
    {
        Vector3 previousPosition = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        Vector3 deltaMovement = transform.position - previousPosition;

        if (standingPlayer != null)
        {
            standingPlayer.transform.Translate(deltaMovement, Space.World);
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            if (targetPosition == pointA.position)
            {
                targetPosition = pointB.position;
            }
            else
            {
                targetPosition = pointA.position;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            standingPlayer = other.gameObject;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            standingPlayer = null;
        }
    }

    void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawSphere(pointA.position, 0.2f);
            Gizmos.DrawSphere(pointB.position, 0.2f);
        }
    }
}