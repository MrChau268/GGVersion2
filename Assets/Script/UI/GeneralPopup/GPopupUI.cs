using UnityEngine;
using TMPro;

public class GPopupUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;

   

    public virtual void OnOpen(GPopupMessage data)
    {
        if (messageText == null)
        {
            Debug.LogError("There is an empty text");
            return;
        }
        messageText.text = data.message;
        gameObject.SetActive(true);
    }

    public virtual void OnClose()
    {
        gameObject.SetActive(false);
    }
}
