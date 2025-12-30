using UnityEngine;

public class MovingObjectSpawner : MonoBehaviour
{
    public GameObject prefab; // assign your cube/sphere prefab
    public Transform player;  // drag your player object here
    public int count = 5;
    public float spawnRange = 10f; // how far from the player

    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnRange, spawnRange),
                Random.Range(-1f, 1f), // little variation in height
                Random.Range(-spawnRange, spawnRange)
            );

            Vector3 spawnPos = player.position + randomOffset;
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
}
