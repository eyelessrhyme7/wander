using UnityEngine;
using UnityEngine.XR;

public class VisibilityController : MonoBehaviour
{
    public Canvas canvas; // Assign your Canvas in the Inspector
    private bool isVisible = true;
    private bool triggerPressed = false;

    void Update()
    {
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Check Trigger input for Right Controller
        if (rightController.TryGetFeatureValue(CommonUsages.trigger, out float rightTriggerValue))
        {
            if (rightTriggerValue > 0.5f) // Trigger is pressed
            {
                if (!triggerPressed) // Ensure it  only toggles on the first press
                {
                    triggerPressed = true; // Mark as pressed
                    isVisible = !isVisible;
                    canvas.gameObject.SetActive(isVisible);
                    Debug.Log($"Canvas visibility toggled: {isVisible}");
                }
            }
            else // Reset when trigger is released
            {
                triggerPressed = false;
            }
            Debug.Log($"Right Trigger Value: {rightTriggerValue}");
        
        }
    }  
}
