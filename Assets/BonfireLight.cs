using UnityEngine;

public class BonfireFlicker : MonoBehaviour
{
    [Header("Intensity Settings")]
    public float minIntensity = 2f;
    public float maxIntensity = 4f;

    [Header("Speed of Flicker")]
    public float flickerSpeed = 0.1f;

    [Header("Optional: Color Flicker")]
    public bool useColorFlicker = false;
    public Color colorA = new Color(1f, 0.5f, 0.2f);   // warm orange
    public Color colorB = new Color(1f, 0.3f, 0.1f);   // darker orange

    private Light bonfireLight;
    private float targetIntensity;

    void Start()
    {
        bonfireLight = GetComponent<Light>();
        targetIntensity = Random.Range(minIntensity, maxIntensity);
    }

    void Update()
    {
        // Smoothly adjust intensity
        bonfireLight.intensity = Mathf.Lerp(bonfireLight.intensity, targetIntensity, Time.deltaTime * 8f);

        // When close to target, choose a new random intensity
        if (Mathf.Abs(bonfireLight.intensity - targetIntensity) < 0.05f)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
        }

        // Optional color flicker
        if (useColorFlicker)
        {
            bonfireLight.color = Color.Lerp(colorA, colorB, Mathf.PingPong(Time.time * 5f, 1));
        }
    }
}
