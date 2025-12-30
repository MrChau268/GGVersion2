using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class FairyAutoScale : MonoBehaviour
{
    [Header("Settings")]
    public Vector3 startScale = new Vector3(0.3f, 0.3f, 0.3f);
    public Vector3 targetScale = Vector3.one;
    public float scaleDuration = 2f;

    [Header("Target Transform Settings")]
    public Vector3 targetPosition = new Vector3(-1f, 0.5f, 0f);  // This will be applied on trigger
    public string lightZoneTag = "LightZone";                    // Tag of the trigger zone

    private bool hasScaled = false;

    void Start()
    {
        // Set starting scale
        transform.localScale = startScale;
    }

    void OnTriggerEnter(Collider other)
    {

        if (!hasScaled && other.CompareTag(lightZoneTag))
        {
            hasScaled = true;

            // Change position
            transform.position = targetPosition;

            // Start scaling coroutine
            StartCoroutine(ScaleUp());
        }
    }

    IEnumerator ScaleUp()
    {
        Vector3 initialScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < scaleDuration)
        {
            float t = elapsed / scaleDuration;
            transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
