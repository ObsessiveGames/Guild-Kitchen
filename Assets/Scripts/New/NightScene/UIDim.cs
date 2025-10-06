using UnityEngine;

public class UIDim : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // The object with the SpriteRenderer
    [SerializeField] private GameObject player;       // The player GameObject
    [SerializeField] private float maxDistance = 8f;  // Distance at which V = 0

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        if (targetObject != null)
            spriteRenderer = targetObject.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            Debug.LogError("SpriteRenderer not found on targetObject!");
    }

    void Update()
    {
        if (spriteRenderer == null || player == null || targetObject == null)
            return;

        float distance = Vector3.Distance(player.transform.position, targetObject.transform.position);

        // Calculate brightness (V in HSV), 1 when close, 0 when far
        float v = Mathf.Clamp01(1f - (distance / maxDistance));

        // Get current color and convert to HSV
        Color originalColor = spriteRenderer.color;
        Color.RGBToHSV(originalColor, out float h, out float s, out float _);

        // Apply new brightness
        Color newColor = Color.HSVToRGB(h, s, v);
        spriteRenderer.color = newColor;
    }
}
