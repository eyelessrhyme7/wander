using UnityEngine;

public class FaceUserUI : MonoBehaviour
{
    private Transform _cameraTransform;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Get the direction to the camera, ignoring the Y-axis
        Vector3 direction = _cameraTransform.position - transform.position;
        direction.y = 0; // Lock rotation to the Y-axis

        // If the direction is zero, skip rotation to avoid errors
        if (direction.sqrMagnitude > 0.001f)
        {
            // Apply an additional 180-degree rotation to correct inversion
            Quaternion lookRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0);
            transform.rotation = lookRotation;
        }
    }
}
