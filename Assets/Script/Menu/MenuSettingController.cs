using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MenuSetting : MonoBehaviour, IMenuService
{
    [Header("Slider UI")]
    [SerializeField] private GameObject slider;
    [SerializeField] private CanvasGroup sliderCanvasGroup;

    [Header("Loading UI")]
    [SerializeField] private CanvasGroup loadingCanvasGroup;
    [SerializeField] private float loadingDuration = 2f;
    [SerializeField] private Image fillImage;

    [Header("Fill Settings")]
    [SerializeField] private float maxFillWidth = 500f;
    [SerializeField] private float targetPercent = 0.7f;
    [SerializeField] private TextMeshProUGUI loadingText;
    [Header("Screen Fade")]
    [SerializeField] private CanvasGroup screenFaderCanvasGroup;
    private void Start()
    {
        if (slider != null)
            slider.SetActive(false);

        if (loadingCanvasGroup != null)
            loadingCanvasGroup.gameObject.SetActive(false);

        if (fillImage != null)
        {
            RectTransform fillRect = fillImage.rectTransform;
            fillRect.sizeDelta = new Vector2(0, fillRect.sizeDelta.y);
        }
    }

    public void OnClickMenu(BaseEventData eventData)
    {
        if (sliderCanvasGroup == null) return;

        GameManager.Instance.FadeInUI(sliderCanvasGroup, 1f, () =>
        {
            ShowLoadingBar(targetPercent);
        });
    }

    public void ShowLoadingBar(float targetPercent)
    {
        if (loadingCanvasGroup != null)
        {
            loadingCanvasGroup.gameObject.SetActive(true);
            loadingCanvasGroup.alpha = 1;
        }

        if (fillImage != null)
            StartCoroutine(AnimateFillBar(targetPercent));
    }

    private IEnumerator AnimateFillBar(float targetPercent)
    {
        RectTransform fillRect = fillImage.rectTransform;
        float targetWidth = maxFillWidth * targetPercent;
        float elapsed = 0f;

        fillRect.sizeDelta = new Vector2(0, fillRect.sizeDelta.y);

        while (elapsed < loadingDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / loadingDuration);
            float currentWidth = Mathf.Lerp(0f, targetWidth, progress);

            float percent = progress * targetPercent * 100f;
            if (loadingText != null)
            {
                loadingText.text = $"Loading... {percent:0}%";
            }


            fillRect.sizeDelta = new Vector2(currentWidth, fillRect.sizeDelta.y);

            Color orange = new Color(1f, 0.5f, 0f);
            fillImage.color = orange;

            yield return null;
        }



        fillRect.sizeDelta = new Vector2(targetWidth, fillRect.sizeDelta.y);
        if (loadingText != null)
            loadingText.text = $"Loading... {targetPercent * 100f:0}%";
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.FadeOutUI(loadingCanvasGroup, 0.5f, () =>
 {
     GameManager.Instance.FadeInUI(screenFaderCanvasGroup, 1f, () =>
     {
         ScreenFader.Instance.FadeToScene("Village");
     });
 });
    }
}