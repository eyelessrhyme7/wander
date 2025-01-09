using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CharacterController))]
public class XRGravityController : MonoBehaviour
{
    public float gravity = -9.81f;      // Gravity force
    public float fallSpeed = 0f;       // Falling speed
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if grounded
        if (controller.isGrounded && fallSpeed < 0)
        {
            fallSpeed = -2f; // Small push to stay grounded
        }
        else
        {
            fallSpeed += gravity * Time.deltaTime; // Apply gravity
        }

        // Apply vertical movement
        Vector3 move = new Vector3(0, fallSpeed, 0);
        controller.Move(move * Time.deltaTime); // Move the player
    }
}
