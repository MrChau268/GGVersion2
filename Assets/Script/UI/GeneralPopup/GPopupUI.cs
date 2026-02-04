using UnityEngine;
using TMPro;

public class GPopupUI : MonoBehaviour
{
    private TMP_Text messageText;


    public virtual void OnOpen(GPopupMessage  data)
    {
        messageText.text = data.message;
        gameObject.SetActive(true);
    }

    public virtual void OnClose()
    {
        gameObject.SetActive(false);
    }
}
