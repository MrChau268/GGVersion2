using UnityEngine;
using DG.Tweening;

public class LightTriggerDOTween : MonoBehaviour
{
    [Header("Popup References")]
    public GameObject popupUI;
    public CanvasGroup popupCanvas;
    public FairyMessageUI fairyMessageUI; // 👈 link to FairyMessageUI

    [Header("Animation Settings")]
    public float fadeDuration = 0.5f;
    public float scaleDuration = 0.5f;

    private void Start()
    {
        if (popupUI != null)
        {
            popupUI.SetActive(false);
            popupUI.transform.localScale = Vector3.zero;
            if (popupCanvas != null) popupCanvas.alpha = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("✅ Player entered light area — showing popup.");
            ShowPopup();
            fairyMessageUI.ActivateFairyDialogue();
            fairyMessageUI.ShowNextMessage(); // 👈 start conversation
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🚪 Player left light area — hiding popup.");
            HidePopup();
        }
    }

    private void ShowPopup()
    {
        popupUI.transform.DOKill();
        if (popupCanvas != null) popupCanvas.DOKill();

        popupUI.SetActive(true);
        popupUI.transform.localScale = Vector3.zero;
        if (popupCanvas != null) popupCanvas.alpha = 0;

        popupUI.transform.DOScale(1f, scaleDuration).SetEase(Ease.OutBack);
        if (popupCanvas != null)
            popupCanvas.DOFade(1f, fadeDuration);
    }

    private void HidePopup()
    {
        popupUI.transform.DOKill();
        if (popupCanvas != null) popupCanvas.DOKill();

        popupUI.transform
            .DOScale(0f, scaleDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => popupUI.SetActive(false));

        if (popupCanvas != null)
            popupCanvas.DOFade(0f, fadeDuration);
    }
}
