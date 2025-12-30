using UnityEngine;

public class BackgroundTogglerWithFade : MonoBehaviour
{
    [Header("Backgrounds")]
    public CanvasGroup menuBackground;
    public CanvasGroup loadingBackground;

    private bool isMenuVisible = true;
    [SerializeField] private MenuSetting menuSetting;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    private void Start()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void ToggleBackground()
    {
        if (isMenuVisible)
        {
            GameManager.Instance.FadeOutUI(menuBackground);
            GameManager.Instance.FadeInUI(loadingBackground, 0.5f, () =>
            {
                if (menuSetting != null)
                {
                    menuSetting.ShowLoadingBar(0.7f);
                }
            });
        }
        else
        {
            GameManager.Instance.FadeOutUI(loadingBackground);
            GameManager.Instance.FadeInUI(menuBackground);
        }

        isMenuVisible = !isMenuVisible;
    }


    public void ShowMenuBackground()
    {
        if (!isMenuVisible)
        {
            ToggleBackground();
        }
    }

    public void ShowLoadingBackground()
    {
        if (isMenuVisible)
        {
            ToggleBackground();
        }
    }
}
