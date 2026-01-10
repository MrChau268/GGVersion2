using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using System.Collections.Generic;

public class ChunkWorldVersion2 : MonoBehaviour
{
    [Header("References")]
    public Terrain terrain;
    public Transform player;

    [Header("Chunk Settings")]
    public int chunkSize = 16;
    public int viewDistance = 3;

    [Header("Object Settings")]
    public float objectSpacing = 2f;
    [Range(0f, 1f)] public float spawnChance = 0.8f;

    [Header("Prefabs")]
    public GameObject treePrefab;
    public GameObject[] stonePrefabs;
    public GameObject flowerPrefab;

    [Header("Highlight Settings")]
    public float highlightDistance = 5f;
    public Material highlightMaterial;
    private GameObject currentHighlighted;
    private Material originalMaterial;
    private Renderer currentRenderer;

    private Dictionary<Vector2Int, GameObject> loadedChunks = new();

    void Start()
    {
        GenerateChunksAroundPlayer();
    }

    void Update()
    {
        GenerateChunksAroundPlayer();
        UnloadDistantChunks();
        HandleHighlight();
    }

    void GenerateChunksAroundPlayer()
    {
        Vector2Int playerChunk = GetChunkCoord(player.position);

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector2Int coord = new(playerChunk.x + x, playerChunk.y + z);
                if (!loadedChunks.ContainsKey(coord))
                    CreateChunk(coord);
            }
        }
    }

    void UnloadDistantChunks()
    {
        Vector2Int playerChunk = GetChunkCoord(player.position);
        List<Vector2Int> toRemove = new();

        foreach (var kvp in loadedChunks)
        {
            int dist = Mathf.Max(
                Mathf.Abs(kvp.Key.x - playerChunk.x),
                Mathf.Abs(kvp.Key.y - playerChunk.y)
            );

            if (dist > viewDistance)
                toRemove.Add(kvp.Key);
        }

        foreach (var c in toRemove)
        {
            Destroy(loadedChunks[c]);
            loadedChunks.Remove(c);
        }
    }

    Vector2Int GetChunkCoord(Vector3 pos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(pos.x / chunkSize),
            Mathf.FloorToInt(pos.z / chunkSize)
        );
    }

    void CreateChunk(Vector2Int coord)
    {

        ChunkWorldData data = GenerateChunkData(coord);

        GameObject chunk = new GameObject($"Chunk {coord}");
        chunk.transform.parent = transform;

        foreach (var spawn in data.spawnList)
        {
            Vector3 pos = spawn.position;
            pos.y = terrain.SampleHeight(pos);

            GameObject prefab = spawn.type switch
            {
                SpawnType.TREE => treePrefab,
                SpawnType.STONE => stonePrefabs[spawn.variantIndex],
                SpawnType.FlOWER => flowerPrefab,
                _ => null
            };

            Instantiate(prefab, pos, Quaternion.identity, chunk.transform);
        }


        loadedChunks.Add(coord, chunk);
    }

    ChunkWorldData GenerateChunkData(Vector2Int coord)
    {
        ChunkWorldData data = new(coord);

        NativeList<SpawnData> jobResults =
            new NativeList<SpawnData>(Allocator.TempJob);

        ChunkGenerationJob job = new()
        {
            chunkSize = chunkSize,
            spacing = objectSpacing,
            spawnChance = spawnChance,
            chunkCoord = new int2(coord.x, coord.y),
            seed = coord.GetHashCode(),
            stoneCount = stonePrefabs.Length,
            results = jobResults
        };

        job.Schedule().Complete();

        foreach (var spawn in jobResults)
            data.spawnList.Add(spawn);

        jobResults.Dispose();
        return data;
    }

    void HandleHighlight()
    {
        Ray ray = new Ray(player.position + Vector3.up * 1.5f, player.forward);
        Debug.DrawRay(ray.origin, ray.direction * highlightDistance, Color.red);

        GameObject hitHighlightable = null;
        Renderer hitRenderer = null;

        if (Physics.Raycast(ray, out RaycastHit hit, highlightDistance))
        {
            Transform t = hit.collider.transform;

            // Walk up to Highlightable root
            while (t != null && !t.CompareTag("Highlightable"))
                t = t.parent;

            if (t != null)
            {
                hitRenderer = t.GetComponentInChildren<Renderer>();
                if (hitRenderer != null)
                {
                    hitHighlightable = t.gameObject;

                    if (currentHighlighted != hitHighlightable)
                    {
                        ResetHighlight();

                        originalMaterial = hitRenderer.material;
                        hitRenderer.material = highlightMaterial;

                        currentHighlighted = hitHighlightable;
                        currentRenderer = hitRenderer;

                        Debug.Log("Highlighted: " + hitHighlightable.name);
                    }
                }
            }
        }

        // 🔑 THIS guarantees un-highlight when looking away
        if (currentHighlighted != null && currentHighlighted != hitHighlightable)
        {
            ResetHighlight();
        }
    }





    void ResetHighlight()
    {
        if (currentRenderer != null && originalMaterial != null)
        {
            currentRenderer.material = originalMaterial;
        }

        currentHighlighted = null;
        currentRenderer = null;
        originalMaterial = null;
    }


}
