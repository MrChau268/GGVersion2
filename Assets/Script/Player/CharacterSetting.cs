using UnityEngine;

[CreateAssetMenu(menuName = "Player/Character Setting")]
public class CharacterSetting : ScriptableObject
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
}
