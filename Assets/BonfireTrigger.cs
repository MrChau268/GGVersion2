using UnityEngine;
using System.Collections;

public class BonfireTextTrigger : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform rect;
    public CanvasGroup canvasGroup;

    [Header("Appear")]
    public float appearOffset = 120f;
    public float appearTime = 0.35f;

    [Header("Bounce Settings")]
    public float bounceHeight = 25f;
    public float bounceTime = 0.25f;

    public float secondaryBounceHeight = 10f;
    public float secondaryBounceTime = 0.18f;

    [Header("Exit Float")]
    public float exitDistance = 200f;
    public float exitTime = 2f;

    private Coroutine routine;

    // PARTICLE EFFECTS
    [Header("Bonfire Particle Effects")]
    public ParticleSystem[] particleSystems;

    // LIGHT CONTROL
    [Header("Bonfire Light")]
    public Light bonfireLight;
    public float lightFadeInTime = 1f;
    public float lightFadeOutTime = 1f;
    private float originalLightIntensity;

    // FADE SETTINGS
    [Header("Particle Fade Settings")]
    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;

    private ParticleSystem.MinMaxGradient[] originalColors;

    void Start()
    {
        // CACHE ORIGINAL INTENSITY FIRST
        if (bonfireLight != null)
        {
            originalLightIntensity = bonfireLight.intensity;

            // FORCE DISABLE FIRST (Unity sometimes ignores first intensity write)
            bonfireLight.enabled = false;
            bonfireLight.intensity = 0f;
        }

        // CACHE PARTICLE COLORS
        originalColors = new ParticleSystem.MinMaxGradient[particleSystems.Length];
        for (int i = 0; i < particleSystems.Length; i++)
            originalColors[i] = particleSystems[i].main.startColor;

        // SET PARTICLES INVISIBLE
        SetParticleAlpha(0f);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(Play());

            StartCoroutine(FadeInParticles(fadeInTime));
            StartCoroutine(FadeInLight(lightFadeInTime));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeOutParticles(fadeOutTime));
            StartCoroutine(FadeOutLight(lightFadeOutTime));
        }
    }

    // ======================================================
    // PARTICLE FADE-IN / FADE-OUT
    // ======================================================

    IEnumerator FadeInParticles(float duration)
    {
        foreach (var ps in particleSystems)
            ps.Play();

        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / duration);

            SetParticleAlpha(alpha);

            yield return null;
        }
    }

    IEnumerator FadeOutParticles(float duration)
    {
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(t / duration);

            SetParticleAlpha(alpha);

            yield return null;
        }

        foreach (var ps in particleSystems)
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void SetParticleAlpha(float alpha)
    {
        for (int i = 0; i < particleSystems.Length; i++)
        {
            var ps = particleSystems[i];
            var main = ps.main;

            ParticleSystem.MinMaxGradient c = originalColors[i];

            Color baseColor = c.color;
            baseColor.a = alpha;       // apply new alpha
            c.color = baseColor;

            main.startColor = c;
        }
    }

    // ======================================================
    // LIGHT FADE-IN / FADE-OUT
    // ======================================================

    IEnumerator FadeInLight(float time)
    {
        if (bonfireLight == null) yield break;

        bonfireLight.enabled = true;  // <— important

        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            bonfireLight.intensity = Mathf.Lerp(0f, originalLightIntensity, t / time);
            yield return null;
        }
    }


    IEnumerator FadeOutLight(float time)
    {
        if (bonfireLight == null) yield break;

        float start = bonfireLight.intensity;
        float t = 0;

        while (t < time)
        {
            t += Time.deltaTime;
            bonfireLight.intensity = Mathf.Lerp(start, 0f, t / time);
            yield return null;
        }

        bonfireLight.intensity = 0f;
        bonfireLight.enabled = false;   // <— force OFF completely
    }


    // ======================================================
    // TEXT ANIMATION (unchanged)
    // ======================================================

    IEnumerator Play()
    {
        canvasGroup.alpha = 0;
        rect.localScale = Vector3.one;

        Vector2 basePos = Vector2.zero;
        Vector2 startPos = new Vector2(0, -appearOffset);
        rect.anchoredPosition = startPos;

        yield return Animate(appearTime, p =>
        {
            rect.anchoredPosition = Vector2.Lerp(startPos, basePos, EaseOutCubic(p));
            canvasGroup.alpha = p;
        });

        Vector2 bigTop = basePos + new Vector2(0, bounceHeight);
        yield return Animate(bounceTime * 0.5f, p =>
            rect.anchoredPosition = Vector2.Lerp(basePos, bigTop, EaseOutQuad(p)));
        yield return Animate(bounceTime * 0.5f, p =>
            rect.anchoredPosition = Vector2.Lerp(bigTop, basePos, EaseInQuad(p)));

        Vector2 smallTop = basePos + new Vector2(0, secondaryBounceHeight);
        yield return Animate(secondaryBounceTime * 0.5f, p =>
            rect.anchoredPosition = Vector2.Lerp(basePos, smallTop, EaseOutQuad(p)));
        yield return Animate(secondaryBounceTime * 0.5f, p =>
            rect.anchoredPosition = Vector2.Lerp(smallTop, basePos, EaseInQuad(p)));

        Vector2 endPos = basePos + new Vector2(0, exitDistance);
        yield return Animate(exitTime, p =>
        {
            rect.anchoredPosition = Vector2.Lerp(basePos, endPos, EaseOutCubic(p));
            canvasGroup.alpha = 1f - p;
        });

        canvasGroup.alpha = 0;
        rect.anchoredPosition = basePos;
    }

    IEnumerator Animate(float time, System.Action<float> update)
    {
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            update(Mathf.Clamp01(t / time));
            yield return null;
        }
        update(1f);
    }

    float EaseOutCubic(float x) => 1 - Mathf.Pow(1 - x, 3);
    float EaseOutQuad(float x) => 1 - (1 - x) * (1 - x);
    float EaseInQuad(float x) => x * x;
}
