using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Vector3 originalScale;
    private float giantTimer = 0f;
    private bool isGiant = false;
    private float giantDuration;
    private float giantScaleMultiplier;
    public bool hasGiantItem = false;
    private bool giantActivated = false;
    public float giantPowerUpDuration = 5f;
    public float giantPowerUpScaleMultiplier = 2f;

    [Header("Hiệu ứng biến hình")]
    public GameObject transformationEffectPrefab;
    private GameObject currentTransformationEffect;
    public float transformationDuration = 0.5f;
    private float transformationTimer = 0f;
    private bool isTransforming = false;

    private Transform playerTransform;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerTransform = transform;

        if (playerMovement != null)
        {
            originalScale = playerTransform.localScale;
        }
        else
        {
            Debug.LogError("PlayerSkill cần được gắn trên cùng GameObject với PlayerMovement hoặc có tham chiếu đến nó.");
            enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && hasGiantItem && !giantActivated && !isTransforming && !isGiant)
        {
            StartGiantTransformation();
            hasGiantItem = false;
            giantActivated = true;
            Debug.Log("Bắt đầu biến hình khổng lồ!");
        }

        if (isTransforming)
        {
            transformationTimer += Time.deltaTime;
            if (transformationTimer >= transformationDuration)
            {
                FinishGiantTransformation();
            }
        }

        if (isGiant)
        {
            giantTimer += Time.deltaTime;
            if (giantTimer >= giantDuration)
            {
                StartRevertTransformation();
            }
        }
    }

    void StartGiantTransformation()
    {
        isTransforming = true;
        transformationTimer = 0f;

        if (transformationEffectPrefab != null)
        {
            currentTransformationEffect = Instantiate(transformationEffectPrefab, playerTransform.position, playerTransform.rotation, playerTransform);
        }

        playerTransform.localScale = originalScale * giantPowerUpScaleMultiplier;

        Invoke(nameof(FinishGiantTransformation), transformationDuration);
    }

    void FinishGiantTransformation()
    {
        isTransforming = false;
        isGiant = true;
        Debug.Log("Biến hình khổng lồ hoàn tất!");
    }

    void StartRevertTransformation()
    {
        isTransforming = true;
        transformationTimer = 0f;

        Invoke(nameof(RevertToNormal), transformationDuration);
    }

    void RevertToNormal()
    {
        isGiant = false;
        playerTransform.localScale = originalScale;
        isTransforming = false;
        Debug.Log("Trở lại kích thước bình thường.");

        if (currentTransformationEffect != null)
        {
            Destroy(currentTransformationEffect);
            currentTransformationEffect = null;
        }
    }
}