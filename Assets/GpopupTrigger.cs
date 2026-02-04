using UnityEngine;

public class GPopupTrigger : MonoBehaviour
{
    public GPopupUI popup;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var data = new GPopupMessage("Hello Player");

        popup.OnOpen(data);
    }
}
