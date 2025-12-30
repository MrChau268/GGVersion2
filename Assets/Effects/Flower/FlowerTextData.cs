using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "FlowerTextData", menuName = "Flower/Flower Text Data")]
public class FlowerTextData : ScriptableObject
{
    [Header("Text Settings")]
    public string text = "Hello!";
    public Color textColor = Color.white;
    public float textSize = 3f;

    [Header("Position Settings")]
    public Vector3 worldOffset = new Vector3(0, 2f, 0);
}
