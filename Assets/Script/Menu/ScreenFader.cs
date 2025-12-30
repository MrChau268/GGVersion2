using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1f;

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

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        yield return StartCoroutine(FadeIn());

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        while (!async.isDone)
            yield return null;

        yield return StartCoroutine(FadeOut());
    }

    public IEnumerator FadeIn()
    {
        canvasGroup.gameObject.SetActive(true);
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    public IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;
        canvasGroup.gameObject.SetActive(false);
    }
}
