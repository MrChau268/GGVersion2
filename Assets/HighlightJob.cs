using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct HighlightJob : IJob
{
    [ReadOnly] public NativeArray<float3> positions;
    public float3 playerPos;
    public float3 forward;
    public float maxDistance;
    public float dotThreshold;

    public NativeArray<int> resultIndex;

    public void Execute()
    {
        int best = -1;
        float bestDot = dotThreshold;

        for (int i = 0; i < positions.Length; i++)
        {
            float3 dir = positions[i] - playerPos;
            float dist = math.length(dir);

            if (dist > maxDistance) continue;

            float dot = math.dot(math.normalize(dir), forward);

            if (dot > bestDot)
            {
                bestDot = dot;
                best = i;
            }
        }

        resultIndex[0] = best;
    }
}
