using UnityEngine;
using UnityEngine.XR;

public class VisibilityController : MonoBehaviour
{
    public Canvas canvas; // Assign your Canvas in the Inspector
    public GameObject gameObject1;
    public GameObject gameObject2;
    public GameObject gameObject3;
    public GameObject gameObject4;
    public GameObject gameObject5;
    private bool isVisible = true;

    bool isVisible1 = true;
    bool isVisible2 = true;
    bool isVisible3 = true;
    bool isVisible4 = true;
    bool isVisible5 = true;
    private bool triggerPressed = false;

    private bool triggerPressed1 = false;
    private bool triggerPressed2 = false;
    private bool triggerPressed3 = false;
    private bool triggerPressed4 = false;
    private bool triggerPressed5 = false;

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
            // Debug.Log($"Right Trigger Value: {rightTriggerValue}");
        
        }

        InputDevice leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        // Check Trigger input for Left Controller
        if (leftController.TryGetFeatureValue(CommonUsages.trigger, out float leftTriggerValue))
        {
            if (leftTriggerValue > 0.5f) // Trigger is pressed
            {
                Debug.Log("Left Trigger Pressed");
                if(!triggerPressed1)
                {
                    triggerPressed1 = true;
                    isVisible1 = !isVisible1;
                    gameObject1.SetActive(isVisible1);

                    isVisible2 = false;
                    isVisible3 = false;
                    isVisible4 = false;
                    isVisible5 = false;
                    gameObject2.SetActive(isVisible2);
                    gameObject3.SetActive(isVisible3);
                    gameObject4.SetActive(isVisible4);
                    gameObject5.SetActive(isVisible5);
                }
            } 
            else
            {
                triggerPressed1 = false;
            }
            // Debug.Log($"Left Trigger Value: {leftTriggerValue}");
        }

        // Check Grip input for Left Controller
        if (leftController.TryGetFeatureValue(CommonUsages.grip, out float leftGripValue))
        {
            if(leftGripValue > 0.5f) {
                if(!triggerPressed2)
                {
                    triggerPressed2 = true;
                    isVisible2 = !isVisible2;
                    gameObject2.SetActive(isVisible2);

                    isVisible1 = false;
                    isVisible3 = false;
                    isVisible4 = false;
                    isVisible5 = false;
                    gameObject1.SetActive(isVisible1);
                    gameObject3.SetActive(isVisible3);
                    gameObject4.SetActive(isVisible4);
                    gameObject5.SetActive(isVisible5);
                }
            }
            else
            {
                triggerPressed2 = false;
            }
            // Debug.Log($"Left Grip Value: {leftGripValue}");
        }

        // Check X Button input for Left Controller
        if (leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool xButtonPressed))
        {
            if (xButtonPressed)
            {
                if(!triggerPressed3)
                {
                    triggerPressed3 = true;
                    isVisible3 = !isVisible3;
                    gameObject3.SetActive(isVisible3);

                    isVisible1 = false;
                    isVisible2 = false;
                    isVisible4 = false;
                    isVisible5 = false;
                    gameObject1.SetActive(isVisible1);
                    gameObject2.SetActive(isVisible2);
                    gameObject4.SetActive(isVisible4);
                    gameObject5.SetActive(isVisible5);
                }
                Debug.Log("Left X Button Pressed");
            } 
            else
            {
                triggerPressed3 = false;
            }
        }

        // Check Y Button input for Left Controller
        if (leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool yButtonPressed))
        {
            if (yButtonPressed)
            {
                if(!triggerPressed4)
                {
                    triggerPressed4 = true;
                    isVisible4 = !isVisible4;
                    gameObject4.SetActive(isVisible4);

                    isVisible1 = false;
                    isVisible2 = false;
                    isVisible3 = false;
                    isVisible5 = false;
                    gameObject1.SetActive(isVisible1);
                    gameObject2.SetActive(isVisible2);
                    gameObject3.SetActive(isVisible3);
                    gameObject5.SetActive(isVisible5);
                }
                Debug.Log("Left Y Button Pressed");
            } 
            else
            {
                triggerPressed4 = false;
            }
        }

        // Check Joystick Press for Left Controller
        if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool leftJoystickPressed))
        {
            if (leftJoystickPressed)
            {
                if(!triggerPressed5)
                {
                    triggerPressed5 = true;
                    isVisible5 = !isVisible5;
                    gameObject5.SetActive(isVisible5);

                    isVisible1 = false;
                    isVisible2 = false;
                    isVisible3 = false;
                    isVisible4 = false;
                    gameObject1.SetActive(isVisible1);
                    gameObject2.SetActive(isVisible2);
                    gameObject3.SetActive(isVisible3);
                    gameObject4.SetActive(isVisible4);
                }
                Debug.Log("Left Joystick Pressed");
            }
            else
            {
                triggerPressed5 = false;
            }
        }
    }  
}
