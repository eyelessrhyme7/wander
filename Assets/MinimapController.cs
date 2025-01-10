using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [Header("Settings")]
    public Transform playerCamera; // Reference to the player's camera
    public Vector3 offset = new Vector3(0, -1, 2); // Offset position relative to the camera
    public float distanceFromCamera = 2.0f; // Distance from the camera along its forward direction
    public float minLookDownAngle = 20.0f; // Angle threshold for looking down
  //  public RectTransform minimapRectTransform; // Reference to the minimap UI

    private Transform canvasTransform; 
    private bool isLookingDown = false;

    void Start()
    {
        // Get the canvas's transform
        canvasTransform = transform;

        // Ensure the playerCamera is set
        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }

        // Ensure minimapRectTransform is assigned in the inspector
        // if (minimapRectTransform == null)
        // {
        //     Debug.LogError("Minimap RectTransform not assigned!");
        // }
    }

    void Update()
    {
        isLookingDown = false;
        // Check if the camera is looking down
        CheckCameraLookingDown();

        // Update the minimap's position and orientation
        UpdateMinimapPosition();
    }

    void CheckCameraLookingDown()
    {
        // Get the vertical angle of the camera
        Vector3 forward = playerCamera.forward;

        // Calculate the angle between the forward direction and Vector3.down
        float angle = Vector3.Angle(forward, Vector3.down);

        // Print the angle (0° when looking directly down, 90° horizontal, 180° when looking directly up)
        Debug.Log("Vertical Angle: " + angle);

        // Normalize the angle (because it can be between 0 and 360 degrees)
       // if (verticalAngle > 180)
         //   verticalAngle -= 360;

        // Determine if the camera is looking down
        
        if(angle < 60.0f || angle > 100.0f){
         //   Debug.Log("Looking Down");
            isLookingDown = true;
        }
        Debug.Log($"Vertical Angle: {angle}, Looking Down: {isLookingDown}");
    }

    void UpdateMinimapPosition()
    {
        if (isLookingDown)
        {
            // If the camera is looking down, fix the minimap position (bottom right)
        //    minimapRectTransform.anchorMin = new Vector2(1, 0);
          //  minimapRectTransform.anchorMax = new Vector2(1, 0);
            //minimapRectTransform.anchoredPosition = new Vector2(-10, 10); // Customize this as needed
        }
        else
        {
            // If the camera is looking forward, follow the camera movement
            Vector3 cameraForward = playerCamera.forward;
            Vector3 cameraPosition = playerCamera.position;

            // Calculate the position offset from the camera
            Vector3 targetPosition = cameraPosition + cameraForward * distanceFromCamera + offset;

            // Set the canvas position
            canvasTransform.position = targetPosition;

            // Ensure the canvas faces the camera
            canvasTransform.rotation = Quaternion.LookRotation(canvasTransform.position - playerCamera.position);
        }
    }
}
