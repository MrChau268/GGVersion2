using UnityEngine;

public class DoorHighlightTrigger : MonoBehaviour
{
    public HighlightController highlightController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            highlightController.EnableHighlight();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            highlightController.DisableHighlight();
    }
}
