using UnityEngine;

public class HighlightController : MonoBehaviour
{
    [Header("Highlight Settings")]
    public Material highlightMaterial;
    private Material[] originalMaterials;

    private MeshRenderer[] meshRenderers;
    private bool isHighlighted = false;

    void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        originalMaterials = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
            originalMaterials[i] = meshRenderers[i].material;
    }

    public void EnableHighlight()
    {
        if (isHighlighted) return;

        foreach (MeshRenderer mr in meshRenderers)
            mr.material = highlightMaterial;

        isHighlighted = true;
    }

    public void DisableHighlight()
    {
        if (!isHighlighted) return;

        for (int i = 0; i < meshRenderers.Length; i++)
            meshRenderers[i].material = originalMaterials[i];

        isHighlighted = false;
    }
}
