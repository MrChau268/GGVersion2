using UnityEngine;
using DG.Tweening;

public class BackpackInventoryUI : MonoBehaviour
{
    [Header("References")]
    public GameObject inventoryUI;      // assign your Inventory Panel in Inspector
    private CanvasGroup canvasGroup;    // for fade control
    private bool isOpen = false;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;   // fade time (seconds)
    public Ease fadeEase = Ease.OutQuad;

    void Start()
    {
        // Get CanvasGroup on inventoryUI
        canvasGroup = inventoryUI.GetComponent<CanvasGroup>();

        // Ensure inventory starts hidden
        canvasGroup.alpha = 0f;
        inventoryUI.SetActive(false);
    }

    void Update()
    {
        // Press J to toggle
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        if (isOpen)
        {
            // Fade out + disable when done
            canvasGroup.DOFade(0f, fadeDuration)
                .SetEase(fadeEase)
                .OnComplete(() => inventoryUI.SetActive(false));
        }
        else
        {
            // Fade in + enable
            inventoryUI.SetActive(true);
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, fadeDuration).SetEase(fadeEase);
        }

        isOpen = !isOpen;
    }
}
