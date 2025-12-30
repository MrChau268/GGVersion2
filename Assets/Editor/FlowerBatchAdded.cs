using UnityEngine;
using UnityEditor;

public class FlowerBatchAdder : EditorWindow
{
    [MenuItem("Tools/Add Flower Script To All Flowers")]
    static void AddFlowerScript()
    {
        // Find all GameObjects tagged "Flower"
        GameObject[] flowers = GameObject.FindGameObjectsWithTag("Flower");

        foreach (GameObject flower in flowers)
        {
            // Add Flower script if it doesn't already exist
            if (flower.GetComponent<Flower>() == null)
            {
                flower.AddComponent<Flower>();
            }
        }

        Debug.Log("Flower script added to all tagged flowers!");
    }
}
