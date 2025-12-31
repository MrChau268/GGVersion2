using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.VisualScripting;

[BurstCompile]
public struct ChunkGenerationJob : IJob
{
    public int chunkSize;
    public float spacing;
    public float spawnChance;
    public float maxSlope;

    public int2 chunkCoord;
    public int seed;
    public int stoneCount;

    [WriteOnly]
    public NativeList<SpawnData> results;

    public void Execute()
    {
        Unity.Mathematics.Random rng = new Unity.Mathematics.Random((uint)seed);

        for (float x = 0; x < chunkSize; x += spacing)
        {
            for (float z = 0; z < chunkSize; z += spacing)
            {
                if (rng.NextFloat() > spawnChance)
                    continue;

                float3 worldPos = new float3(
                    chunkCoord.x * chunkSize + x,
                    0,
                    chunkCoord.y * chunkSize + z
                );

                float noise = Unity.Mathematics.noise.snoise(worldPos.xz * 0.1f);

                if (noise > 0.5f)
                {
                    results.Add(new SpawnData
                    {
                        position = worldPos,
                        type = SpawnType.TREE
                    });
                }
                else if (noise > 0.3f)
                {
                    results.Add(new SpawnData
                    {
                        position = worldPos,
                        type = SpawnType.STONE,
                        variantIndex = rng.NextInt(0, stoneCount)
                    });
                }
            }
        }

        // Flower clusters
        int clusters = rng.NextInt(2, 5);
        for (int i = 0; i < clusters; i++)
        {
            float3 center = new float3(
                chunkCoord.x * chunkSize + rng.NextFloat(0, chunkSize),
                0,
                chunkCoord.y * chunkSize + rng.NextFloat(0, chunkSize)
            );

            int count = rng.NextInt(3, 8);
            for (int j = 0; j < count; j++)
            {
                results.Add(new SpawnData
                {
                    position = center + new float3(
                        rng.NextFloat(-1f, 1f),
                        0,
                        rng.NextFloat(-1f, 1f)
                    ),
                    type = SpawnType.FlOWER
                });
            }
        }
    }
}
