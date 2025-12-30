using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Dialogue", menuName = "NPC/Dialogue")]
public class NPCDialogue : ScriptableObject
{
    [Header("Dialogue Info")]
    public string title;            // NPC Name
    [TextArea(2, 10)]
    public string[] messages;       // Multiple messages in an array
}
