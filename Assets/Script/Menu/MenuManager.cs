using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeInUI(CanvasGroup canvasGroup, float duration = 0.5f, Action onComplete = null)
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, 0, 1, duration, onComplete));
    }

    public bool FadeOutUI(CanvasGroup canvasGroup, float duration = 0.5f, Action onComplete = null)
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, 1, 0, duration, onComplete));
        return true;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration, Action onComplete)
    {
        group.gameObject.SetActive(true);
        group.alpha = from;
        group.interactable = false;
        group.blocksRaycasts = false;

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        group.alpha = to;
        group.interactable = to == 1;
        group.blocksRaycasts = to == 1;

        if (to == 0)
            group.gameObject.SetActive(false);

        onComplete?.Invoke();
    }

    public void LoadSceneWithFade(string sceneName, CanvasGroup fadeCanvas, float fadeDuration = 0.5f)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, fadeCanvas, fadeDuration));
    }

    private IEnumerator LoadSceneRoutine(string sceneName, CanvasGroup fadeCanvas, float fadeDuration)
    {
        yield return StartCoroutine(FadeCanvasGroup(fadeCanvas, 0, 1, fadeDuration, null));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(FadeCanvasGroup(fadeCanvas, 1, 0, fadeDuration, null));
    }

}
