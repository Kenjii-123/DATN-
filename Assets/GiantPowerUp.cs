using UnityEngine;

public class GiantPowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSkill playerSkill = other.GetComponent<PlayerSkill>();
            if (playerSkill != null)
            {
                playerSkill.hasGiantItem = true;
                Destroy(gameObject);
                Debug.Log("Đã thu thập Giant Item!");
            }
        }
    }
}