using UnityEngine;
using System.Collections;

public class LightZoneTrigger : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform fairy;

    [Header("Settings")]
    public float scaleDuration = 2f;
    public Vector3 fairyStartScale = new Vector3(0.3f, 0.3f, 0.3f);
    public Vector3 fairyTargetScale = Vector3.one;

    private bool playerInZone = false;
    private bool fairyInZone = false;
    private bool triggered = false;

    void Start()
    {
        if (fairy != null)
        {
            fairy.localScale = fairyStartScale;
            Debug.Log("[LightZone] Fairy scale initialized.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            Debug.Log("[LightZone] Player entered.");
        }

        if (other.CompareTag("Fairy"))
        {
            fairyInZone = true;
            Debug.Log("[LightZone] Fairy entered.");
        }

        if (playerInZone && fairyInZone && !triggered)
        {
            triggered = true;
            Debug.Log("[LightZone] Both Player and Fairy are in the zone. Scaling fairy.");
            StartCoroutine(ScaleFairy());
        }
    }

    IEnumerator ScaleFairy()
    {
        if (fairy == null)
        {
            Debug.LogError("[LightZone] Fairy reference missing.");
            yield break;
        }

        Vector3 startScale = fairy.localScale;
        float elapsed = 0f;

        while (elapsed < scaleDuration)
        {
            float t = elapsed / scaleDuration;
            fairy.localScale = Vector3.Lerp(startScale, fairyTargetScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fairy.localScale = fairyTargetScale;

        Debug.Log("[LightZone] Fairy scaling complete.");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            if (col is BoxCollider box)
            {
                Gizmos.DrawCube(box.center, box.size);
            }
        }
    }
}
