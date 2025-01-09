using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SphericalButtonBehavior : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Color originalColor;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
    }

    // Hover Enter (First hover)
    public void OnFirstHoverEntered(HoverEnterEventArgs args)
    {
        meshRenderer.material.color = Color.red; // Change color to red when hovering
    }

    // Hover Exit (Last hover)
    public void OnLastHoverExited(HoverExitEventArgs args)
    {
        meshRenderer.material.color = originalColor; // Reset color when hover ends
    }

    // Select Enter (First selection)
    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        meshRenderer.material.color = Color.yellow; // Change color to yellow on click
    }

    // Select Exit (Last selection)
    // public void OnSelectExited(SelectExitEventArgs args)
    // {
    //     Destroy(gameObject, 0.5f); // Destroy the object after 0.5 seconds once it's deselected
    //     Debug.Log("Object deselected and will be destroyed.");
    // }
}
