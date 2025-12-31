using UnityEngine;

public enum SpawnType
{
    TREE,
    STONE,
    FlOWER
}

public struct SpawnData
{
    public Vector3 position;
    public SpawnType type;
    public int variantIndex;
}