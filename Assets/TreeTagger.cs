using UnityEngine;

public class TreeTagger : MonoBehaviour
{
    void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.tag = "Tree";
        }

    }
}
