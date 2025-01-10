using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WorldSpaceOverlay : MonoBehaviour
{
    private const string shaderTestMode = "unity_GUIZTestMode";
    [SerializeField] private UnityEngine.Rendering.CompareFunction desiredUIComparison = UnityEngine.Rendering.CompareFunction.Always;
    [SerializeField] private Graphic[] graphics;

    private Dictionary<Material, Material> materialsMappings = new Dictionary<Material, Material>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (graphics == null || graphics.Length == 0)
        {
            Debug.LogError("No Graphics assigned. Please add at least one Graphic component.");
            return;
        }

        foreach (Graphic graphic in graphics)
        {
            if (graphic == null)
            {
                Debug.LogWarning("A Graphic in the array is null. Skipping this element.");
                continue;
            }

            Material material = graphic.materialForRendering;
            if (material == null)
            {
                Debug.LogWarning($"Graphic {graphic.name} has no material for rendering. Skipping this element.");
                continue;
            }

            if (!materialsMappings.TryGetValue(material, out Material materialCopy))
            {
                materialCopy = new Material(material);
                materialsMappings.Add(material, materialCopy);
            }

            materialCopy.SetInt(shaderTestMode, (int)desiredUIComparison);
            graphic.material = materialCopy;
        }
    }
}
