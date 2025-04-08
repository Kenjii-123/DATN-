using UnityEngine;

public class LightningPowerUp : MonoBehaviour
{
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
           
            PlayerSkills playerSkills = other.GetComponent<PlayerSkills>();
            if (playerSkills != null)
            {
                playerSkills.EnableLightningSkill();
            }

            Destroy(gameObject);
        }
    }
}