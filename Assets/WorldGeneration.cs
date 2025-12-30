using UnityEngine;
using System.Collections.Generic;

public class ChunkWorld : MonoBehaviour
{
    [Header("References")]
    public Terrain terrain;
    public Transform player;

    [Header("Chunk Settings")]
    public int chunkSize = 16;
    public int viewDistance = 3;

    [Header("Object Settings")]
    public float objectSpacing = 2f;       // Distance between objects
    [Range(0f, 1f)] public float spawnChance = 0.8f;

    public GameObject treePrefab;
    public GameObject stonePrefab;
    public GameObject flowerPrefab;

    [Header("Terrain Constraints")]
    public float maxSlopeAngle = 30f; // Maximum slope (in degrees) to place objects

    private Dictionary<Vector2Int, GameObject> loadedChunks = new Dictionary<Vector2Int, GameObject>();

    private void Start()
    {
        // Place player slightly above terrain
        Vector3 startPos = player.position;
        startPos.y = terrain.SampleHeight(startPos) + 1f; // +1 to prevent falling
        player.position = startPos;

        // Generate initial chunks
        GenerateChunksAroundPlayer();
    }

    private void Update()
    {
        GenerateChunksAroundPlayer();
        UnloadDistantChunks();
    }

    #region Chunk Management

    private void GenerateChunksAroundPlayer()
    {
        Vector2Int playerChunk = GetChunkCoord(player.position);

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector2Int coord = new Vector2Int(playerChunk.x + x, playerChunk.y + z);
                if (!loadedChunks.ContainsKey(coord))
                    CreateChunk(coord);
            }
        }
    }

    private void UnloadDistantChunks()
    {
        Vector2Int playerChunk = GetChunkCoord(player.position);
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();

        foreach (var kvp in loadedChunks)
        {
            Vector2Int chunkCoord = kvp.Key;
            int distance = Mathf.Max(Mathf.Abs(chunkCoord.x - playerChunk.x), Mathf.Abs(chunkCoord.y - playerChunk.y));

            if (distance > viewDistance)
                chunksToRemove.Add(chunkCoord);
        }

        foreach (var coord in chunksToRemove)
        {
            Destroy(loadedChunks[coord]);
            loadedChunks.Remove(coord);
        }
    }

    private Vector2Int GetChunkCoord(Vector3 pos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(pos.x / chunkSize),
            Mathf.FloorToInt(pos.z / chunkSize)
        );
    }

    private void CreateChunk(Vector2Int coord)
    {
        GameObject chunk = new GameObject("Chunk " + coord);
        chunk.transform.parent = transform;
        chunk.transform.position = new Vector3(coord.x * chunkSize, 0, coord.y * chunkSize);

        GenerateObjects(chunk.transform, coord);
        loadedChunks.Add(coord, chunk);
    }

    #endregion

    #region Object Generation

    private void GenerateObjects(Transform parent, Vector2Int coord)
    {
        for (float x = 0; x < chunkSize; x += objectSpacing)
        {
            for (float z = 0; z < chunkSize; z += objectSpacing)
            {
                if (Random.value > spawnChance) continue;

                Vector3 basePos = parent.position + new Vector3(x, 0, z);
                float terrainHeight = terrain.SampleHeight(basePos);
                Vector3 pos = new Vector3(basePos.x, terrainHeight, basePos.z);

                // Check terrain slope
                float slope = terrain.terrainData.GetSteepness(pos.x / terrain.terrainData.size.x, pos.z / terrain.terrainData.size.z);
                if (slope > maxSlopeAngle)
                    continue; // skip steep areas

                // Perlin noise for variety
                float noise = Mathf.PerlinNoise(
                    (coord.x * chunkSize + x) * 0.1f,
                    (coord.y * chunkSize + z) * 0.1f
                );

                // Adjusted probabilities: more trees, fewer stones
                if (noise > 0.5f)           // Tree
                    Spawn(treePrefab, pos, parent);
                else if (noise > 0.3f)      // Stone
                    Spawn(stonePrefab, pos, parent);
                else                         // Flower
                    Spawn(flowerPrefab, pos, parent);
            }
        }
    }

    private void Spawn(GameObject prefab, Vector3 pos, Transform parent)
    {
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity, parent);
        obj.transform.localScale *= Random.Range(0.8f, 1.2f);
        obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }

    #endregion
}
