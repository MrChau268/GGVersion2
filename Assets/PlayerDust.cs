using UnityEngine;

public class PlayerDust : MonoBehaviour
{
    public GameObject dustEffectPrefab;
    public Transform footPosition;
    public float moveThreshold = 0.1f;
    public float spawnCooldown = 0.2f;

    private float lastSpawnTime;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("PlayerDust: Rigidbody not found.");
    }

    void Update()
    {
        float speed = rb.linearVelocity.magnitude;
        Debug.Log("Player Speed: " + speed);  // ← ADD THIS

        if (speed > moveThreshold && Time.time - lastSpawnTime > spawnCooldown)
        {
            SpawnDust();
            lastSpawnTime = Time.time;
        }
    }


    void SpawnDust()
    {
        if (dustEffectPrefab != null && footPosition != null)
        {
            GameObject dust = Instantiate(dustEffectPrefab, footPosition.position, Quaternion.identity);
            Destroy(dust, 1f); // 💥 Auto-destroy after 1 second
        }
    }

}
