using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AxeEquipController : MonoBehaviour
{
    [Header("References")]
    public Button axeButton;          // UI button (icon in inventory)
    public GameObject axeObject;      // 3D axe in player's hand

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;
    public Ease fadeEase = Ease.OutQuad;
    public TreeHighlighter treeHighlighter;


    private Renderer axeRenderer;
    private bool isEquipped = false;

    void Start()
    {
        if (axeObject != null)
        {
            axeRenderer = axeObject.GetComponentInChildren<Renderer>();
            axeObject.SetActive(false);

            // Make sure material supports transparency
            if (axeRenderer != null)
            {
                var mat = axeRenderer.material;
                mat.SetFloat("_Mode", 3); // Transparent mode
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                Color c = mat.color;
                c.a = 0;
                mat.color = c;
            }
        }

        if (axeButton != null)
            axeButton.onClick.AddListener(ToggleAxe);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
            ToggleAxe();

        if (isEquipped && Input.GetKeyDown(KeyCode.E))
        {
            TryCutTree();
        }
    }


    void TryCutTree()
    {
        if (treeHighlighter == null)
        {
            Debug.LogWarning("TreeHighlighter is not assigned.");
            return;
        }

        GameObject tree = treeHighlighter.GetCurrentHighlightedTree();
        if (tree == null)
        {
            Debug.Log("No tree is currently highlighted.");
            return;
        }

        Debug.Log("Tree cut: " + tree.name);
        Destroy(tree);
    }



    void ToggleAxe()
    {
        if (axeObject == null || axeRenderer == null) return;

        if (!isEquipped)
        {
            // Equip (fade in)
            axeObject.SetActive(true);
            Color c = axeRenderer.material.color;
            c.a = 0;
            axeRenderer.material.color = c;

            axeRenderer.material
                .DOFade(1f, fadeDuration)
                .SetEase(fadeEase);

            // Enable tree highlighting
            if (treeHighlighter != null)
                treeHighlighter.SetHighlighting(true);
        }
        else
        {
            // Unequip (fade out)
            axeRenderer.material
                .DOFade(0f, fadeDuration)
                .SetEase(fadeEase)
                .OnComplete(() =>
                {
                    axeObject.SetActive(false);

                    // Disable tree highlighting
                    if (treeHighlighter != null)
                        treeHighlighter.SetHighlighting(false);
                });
        }

        isEquipped = !isEquipped;
    }

}
