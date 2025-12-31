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
    [Header("Stones Setting")]

    //List of stones
    public GameObject[] stonePrefab;


    [Header("Terrain Constraints")]
    public float maxSlopeAngle = 30f; // Maximum slope (in degrees) to place objects

    [Header("Flower Setting")]
    public GameObject flowerPrefab;
    public int minFlowerAppear = 2;
    public int maxFlowerAppear = 5;
    public int minFlowerPerCluster = 3;
    public int maxFlowerPerCluster = 8;

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
        chunk.isStatic = true;
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
                else if (noise > 0.3f && stonePrefab.Length > 0)      // Stone
                {
                    GameObject randomStone = stonePrefab[Random.Range(0, stonePrefab.Length)];
                    Spawn(randomStone, pos, parent);
                }

                //Flower gonna skipped here
            }
        }

        /* Benefit of Sloping Flower 
        Flowers can have different density and spacing.

        Flowers can tolerate different slopes than trees/stones.

        Flowers can have special behaviors (sway, move, etc.) without affecting trees and stones.

        Keeps generation modular and easy to tweak.
        */
        int clusters = Random.Range(minFlowerAppear, maxFlowerAppear); // number of flower groups per chunk
        int flowersPerCluster = Random.Range(minFlowerPerCluster, maxFlowerPerCluster); // flowers per group

        for (int i = 0; i < clusters; i++)
        {
            // Random cluster center within chunk
            Vector3 clusterCenter = parent.position + new Vector3(
                Random.Range(0f, chunkSize),
                0,
                Random.Range(0f, chunkSize)
            );
            clusterCenter.y = terrain.SampleHeight(clusterCenter);

            for (int j = 0; j < flowersPerCluster; j++)
            {
                // Small random offset from cluster center
                Vector3 offset = new Vector3(
                    Random.Range(-1f, 1f),
                    0,
                    Random.Range(-1f, 1f)
                );
                Vector3 pos = clusterCenter + offset;
                pos.y = terrain.SampleHeight(pos);

                // Optional: check slope for flowers
                float slope = terrain.terrainData.GetSteepness(pos.x / terrain.terrainData.size.x, pos.z / terrain.terrainData.size.z);
                if (slope > maxSlopeAngle + 10f) continue;
                Spawn(flowerPrefab, pos, parent);
            }

        }
    }

    private void Spawn(GameObject prefab, Vector3 pos, Transform parent)
    {
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity, parent);
        obj.transform.localScale *= Random.Range(0.8f, 1.2f);
        obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        // Mark as occlusion static for Unity's culling system
        obj.isStatic = true;
    }

    #endregion
}
