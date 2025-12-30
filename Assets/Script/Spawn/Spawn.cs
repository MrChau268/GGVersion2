using UnityEngine;

[CreateAssetMenu(fileName = "MovingObjectData", menuName = "Custom/Moving Object Data")]
public class MovingObjectData : ScriptableObject
{
    [Header("Spawn Settings")]
    public Vector2 rangeX = new Vector2(-5, 5);
    public Vector2 rangeZ = new Vector2(-5, 5);

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float moveRadius = 3f;
}
