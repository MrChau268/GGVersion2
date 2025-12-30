using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RadialWaveThin : MonoBehaviour
{
    public int segments = 100;
    public float waveSpeed = 2f;
    public float waveWidth = 0.2f;

    public float movementMultiplier = 0.3f; // <<< NEW — lower = closer spacing

    private float maxRadius;
    private float currentRadius = 0f;

    private LineRenderer line;

    private bool isActive = false;

    void Start()
    {
        MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
        if (meshFilter != null)
        {
            Bounds bounds = meshFilter.sharedMesh.bounds;
            float scaleFactor = 0.5f;    // <<< Smaller number = smaller wave
            maxRadius = Mathf.Max(bounds.extents.x, bounds.extents.z) * scaleFactor;

        }
        else
        {
            maxRadius = 1f;
        }

        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = segments + 1;
        line.startWidth = waveWidth;
        line.endWidth = waveWidth;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.white;
        line.endColor = Color.white;
        line.useWorldSpace = false;
        line.loop = true;
    }

    void Update()
    {
        // Smaller multiplier = radius grows slower = closer rings
        currentRadius += Time.deltaTime * waveSpeed * movementMultiplier;

        DrawRing(currentRadius);

        if (currentRadius >= maxRadius)
        {
            currentRadius = 0f;
        }
    }

    void DrawRing(float radius)
    {
        float angleStep = 360f / segments;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            line.SetPosition(i, new Vector3(x, 0.8f, z));
        }
    }
    // === Triggering the wave on player collision ===

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure your player has tag "Player"
        {
            isActive = true;
            line.enabled = true;
        }
    }

    // Optional: turn off when player leaves
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = false;
            line.enabled = false;
            currentRadius = 0f;
        }
    }
}
