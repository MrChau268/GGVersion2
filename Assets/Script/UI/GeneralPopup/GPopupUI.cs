using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GPopupUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] protected TMP_Text messageText;
    [SerializeField] protected Button cancelBtn;
    [SerializeField] protected Button confirmBtn;

    [Header("Animation")]
    [SerializeField] protected float animationDuration = 0.25f;

    private CanvasGroup canvasGroup;
    private GPopupData popupData;
    private Coroutine animationRoutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Ensure clean initial state
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * 0.8f;

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Debug.Log("GPopupUI ENABLED (popup shown)");

        // Reset visual state every time (important for pooling)
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * 0.8f;

        StartAnimateIn();
    }

    private void OnDisable()
    {
        Debug.Log("GPopupUI DISABLED (popup hidden)");
        CancelInvoke();
    }

    public void ShowPopup(GPopupData data)
    {
        gameObject.SetActive(true);
        popupData = data;

        messageText.text = data.message;

        confirmBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();

        confirmBtn.onClick.AddListener(() =>
        {
            data.onConfirm?.Invoke();
            ClosePopup();
        });

        cancelBtn.onClick.AddListener(() =>
        {
            data.onCancel?.Invoke();
            ClosePopup();
        });

        cancelBtn.gameObject.SetActive(data.onCancel != null);

        

        if (data.autoCloseTime > 0f)
        {
            Invoke(nameof(ClosePopup), data.autoCloseTime);
        }
    }

    public void ClosePopup()
    {
        CancelInvoke();
        StartAnimateOut();
    }

    // ------------------ Animation ------------------

    private void StartAnimateIn()
    {
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        animationRoutine = StartCoroutine(AnimateIn());
    }

    private void StartAnimateOut()
    {
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        animationRoutine = StartCoroutine(AnimateOut());
    }

    private IEnumerator AnimateIn()
    {
        float time = 0f;

        while (time < animationDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / animationDuration;

            canvasGroup.alpha = t;
            transform.localScale = Vector3.one * Mathf.Lerp(0.8f, 1f, t);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;
    }

    private IEnumerator AnimateOut()
    {
        float time = 0f;

        while (time < animationDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / animationDuration;

            canvasGroup.alpha = 1f - t;
            transform.localScale = Vector3.one * Mathf.Lerp(1f, 0.8f, t);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);

        GPopupManager.Instance.NotifyPopupClosed();
    }
}
