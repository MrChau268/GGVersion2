using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SocialPlatforms;

public class GPopupUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    protected TMP_Text messageText;
    [SerializeField]
    protected Button cancelBtn;
    [SerializeField]
    protected Button confirmBtn;

    [Header("Animation")]
    [SerializeField]
    protected float animationDuration = 0.25f;

    CanvasGroup canvasGroup;
    GPopupData popupData;


    protected void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        gameObject.SetActive(false);
    }

    public void ShowPopup(GPopupData data)
    {
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
        gameObject.SetActive(true);
        if (data.autoCloseTime > 0)
        {
            Invoke(nameof(ClosePopup), data.autoCloseTime);
        }
    }

    public void ClosePopup()
    {
        CancelInvoke();
        StartCoroutine(AnimateOut());
    }


    IEnumerator AnimateIn()
    {
        float time = 0;
        canvasGroup.alpha = 0;
        transform.localScale = Vector3.one * 0.8f;

        while (time < animationDuration)
        {
            time += Time.unscaledDeltaTime;
            float alpha = time / animationDuration;
            canvasGroup.alpha = alpha;
            transform.localScale = Vector3.one * Mathf.Lerp(0.8f, 1f, alpha);
            yield return null;
        }

        canvasGroup.alpha = 1;
        transform.localScale = Vector3.one;
    }

    IEnumerator AnimateOut()
    {
        float time = 0;
        while (time < animationDuration)
        {
            time += Time.unscaledDeltaTime;
            float alpha = time / animationDuration;
            canvasGroup.alpha = 1 - alpha;
            transform.localScale = Vector3.one * Mathf.Lerp(1f, 0.8f, alpha);
            yield return null;
        }

        gameObject.SetActive(false);    
        GPopupManager.Instance.NotifyPopupClosed();
    }
}
