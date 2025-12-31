using System.Collections.Generic;
using UnityEngine;

public class ChunkWorldData
{
    public Vector2Int coord;
    public List<SpawnData> spawnList;

    public ChunkWorldData(Vector2Int coord)
    {
        this.coord = coord;
        spawnList = new List<SpawnData>();
    }
}
