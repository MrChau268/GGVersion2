using UnityEngine;

public class FlowerTextTrigger : MonoBehaviour
{
    public CanvasGroup canvasGroup;   // reference to CanvasGroup on the canvas
    public float fadeDuration = 0.5f; // fade speed
    public string playerTag = "Player";

    private bool isFading;

    private void Start()
    {
        canvasGroup.alpha = 0f;  // start invisible
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            FadeTo(1f); // fade in
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            FadeTo(0f); // fade out
    }

    private void FadeTo(float targetAlpha)
    {
        if (!gameObject.activeInHierarchy) return;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(targetAlpha));
    }

    private System.Collections.IEnumerator FadeRoutine(float target)
    {
        float start = canvasGroup.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;
    }
}
