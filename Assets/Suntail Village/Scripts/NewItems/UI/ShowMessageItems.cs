using UnityEngine;

[CreateAssetMenu(fileName = "NewMessage", menuName = "UI/Show Message Data")]
public class ShowMessageItems : ScriptableObject
{
    [TextArea(2, 4)] 
    public string message;

    public Color textColor = Color.white;
    public float duration = 2f;    // how long to show
}
