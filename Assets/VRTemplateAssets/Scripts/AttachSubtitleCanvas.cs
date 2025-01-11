using UnityEngine;

public class AttachSubtitleCanvas : MonoBehaviour
{
    public Canvas subtitleCanvas; // Assign the Canvas in the Inspector or dynamically

    void Start()
    {
        // Find the Main Camera in the scene
        GameObject mainCamera = GameObject.FindWithTag("MainCamera");

        if (mainCamera != null && subtitleCanvas != null)
        {
            // Attach the canvas to the camera
            subtitleCanvas.transform.SetParent(mainCamera.transform);

            // Adjust position relative to the camera
            subtitleCanvas.transform.localPosition = new Vector3(0, -0.5f, 2f);
            subtitleCanvas.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogError("Main Camera or Subtitle Canvas not found!");
        }
    }
}

