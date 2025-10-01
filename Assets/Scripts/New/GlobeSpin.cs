using UnityEngine;

public class GlobeSpinRepeatable : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpin;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private float rotationSpeed = 180f; // max degrees per second
    [SerializeField] private Vector3 rotationAxis = Vector3.up; // local axis
    [SerializeField] private float triggerDistance = 3f;

    private bool isSpinning = false;
    private float rotatedDegrees = 0f;

    void Start()
    {
        if (objectToSpin == null && transform.childCount > 0)
            objectToSpin = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if (playerObject != null && Input.GetKeyDown(KeyCode.E))
        {
            float distance = Vector3.Distance(playerObject.transform.position, objectToSpin.transform.position);
            if (distance <= triggerDistance)
            {
                // Start or restart a spin on every E press
                isSpinning = true;
                rotatedDegrees = 0f;
            }
        }

        if (isSpinning && objectToSpin != null)
        {
            float delta = rotationSpeed * Time.deltaTime;

            // Start easing out after 1/5 of rotation (72°)
            if (rotatedDegrees >= 360f * 0.2f)
            {
                float t = (rotatedDegrees - 360f * 0.2f) / (360f * 0.8f); // 0 -> 1 for remaining 4/5
                float easedSpeed = rotationSpeed * (1 - t * t * t); // cubic ease-out
                delta = easedSpeed * Time.deltaTime;
            }

            objectToSpin.transform.Rotate(rotationAxis * delta, Space.Self);
            rotatedDegrees += delta;

            if (rotatedDegrees >= 360f)
            {
                float overshoot = rotatedDegrees - 360f;
                objectToSpin.transform.Rotate(rotationAxis * -overshoot, Space.Self);
                isSpinning = false;
            }
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    if (objectToSpin != null)
    //    {
    //        Gizmos.color = Color.cyan;
    //        Gizmos.DrawWireSphere(objectToSpin.transform.position, triggerDistance);
    //    }
    //}
}
