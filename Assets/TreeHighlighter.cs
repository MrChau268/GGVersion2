using UnityEngine;

public class TreeHighlighter : MonoBehaviour
{
    [Header("Highlight Settings")]
    public Material highlightMaterial;
    public float highlightDistance = 6f;

    private Transform player;
    private Renderer[] allTreeRenderers;
    private Material[] originalMaterials;
    private Renderer currentHighlighted;

    private bool isHighlightingEnabled = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        allTreeRenderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[allTreeRenderers.Length];

        for (int i = 0; i < allTreeRenderers.Length; i++)
            originalMaterials[i] = allTreeRenderers[i].sharedMaterial;
    }

    void Update()
    {
        if (!isHighlightingEnabled || player == null || allTreeRenderers.Length == 0)
        {
            ClearHighlight();
            return;
        }

        // 🧠 Check if currentHighlighted is null (maybe destroyed)
        if (currentHighlighted != null && currentHighlighted.gameObject == null)
        {
            currentHighlighted = null;
        }

        Renderer nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var rend in allTreeRenderers)
        {
            // Skip null/destroyed
            if (rend == null) continue;

            float dist = Vector3.Distance(player.position, rend.transform.position);
            if (dist < minDist && dist < highlightDistance)
            {
                minDist = dist;
                nearest = rend;
            }
        }

        if (nearest != currentHighlighted)
        {
            if (currentHighlighted != null)
                ResetMaterial(currentHighlighted);

            if (nearest != null)
                ApplyHighlight(nearest);

            currentHighlighted = nearest;
        }
    }

    void ClearHighlight()
    {
        if (currentHighlighted != null)
        {
            ResetMaterial(currentHighlighted);
            currentHighlighted = null;
        }
    }

    void ApplyHighlight(Renderer rend)
    {
        rend.material = highlightMaterial;
    }

    void ResetMaterial(Renderer rend)
    {
        for (int i = 0; i < allTreeRenderers.Length; i++)
        {
            if (allTreeRenderers[i] == rend)
            {
                rend.material = originalMaterials[i];
                break;
            }
        }
    }

    public void SetHighlighting(bool enabled)
    {
        isHighlightingEnabled = enabled;

        // If disabling, clear any current highlight immediately
        if (!enabled && currentHighlighted != null)
        {
            ResetMaterial(currentHighlighted);
            currentHighlighted = null;
        }
    }
    public GameObject GetCurrentHighlightedTree()
    {
        return currentHighlighted != null ? currentHighlighted.gameObject : null;
    }

}
