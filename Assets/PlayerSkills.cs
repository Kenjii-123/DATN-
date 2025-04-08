using UnityEngine;
public class PlayerSkills : MonoBehaviour
{
    public GameObject lightningStrikePrefab;
    public Transform lightningSpawnPoint;
    public KeyCode lightningKey = KeyCode.Z;
    public float lightningCooldown = 1f;

    private bool canUseLightning = false;
    private float lastLightningTime;

    public void EnableLightningSkill()
    {
        canUseLightning = true;
        Debug.Log("Lightning skill enabled!");
    }

    void Update()
    {
        if (canUseLightning && Input.GetKeyDown(lightningKey) && Time.time > lastLightningTime + lightningCooldown)
        {
            SummonLightning();
        }
    }

    void SummonLightning()
    {
        if (lightningStrikePrefab != null && lightningSpawnPoint != null)
        {
            Instantiate(lightningStrikePrefab, lightningSpawnPoint.position, Quaternion.identity);
            lastLightningTime = Time.time;
        }
        else
        {
            Debug.LogError("Lightning Strike Prefab or Spawn Point is not assigned!");
        }
    }
}