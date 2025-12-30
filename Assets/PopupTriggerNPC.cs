using UnityEngine;

public class PopupTriggerNPC : MonoBehaviour
{
    public PopupControllerNPC popup;
    public string popupTitle = "Mysterious Object";
    [TextArea]
    public string[] popupMessages = {
       
    };

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            popup.ShowPopup(popupTitle, popupMessages);
        }
    }
}
