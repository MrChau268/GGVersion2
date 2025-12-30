using UnityEngine;

[System.Serializable]
public class FairyMessage
{
    public string title;              // The title of the message
    [TextArea(2, 5)] public string message; // The content of the message (multi-line)
}
