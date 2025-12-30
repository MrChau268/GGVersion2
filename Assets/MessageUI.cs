using TMPro;
using UnityEngine;

public class MessageUI : MonoBehaviour
{
    public static MessageUI instance;

    public GameObject panel;
    public TMP_Text messageText;

    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
            Debug.Log("MessageUI instance set.");
        }
        else 
        {
            Debug.LogWarning("Duplicate MessageUI instance found. Destroying this one.");
            Destroy(gameObject);
        }

        if (panel == null)
        {
            Debug.LogError("Panel reference is missing!");
        }

        if (messageText == null)
        {
            Debug.LogError("MessageText reference is missing!");
        }

        panel?.SetActive(false); // hide by default
        Debug.Log("MessageUI panel set inactive at Awake.");
    }

    public void ShowMessage(string message)
    {
        if (panel == null || messageText == null)
        {
            Debug.LogError("Cannot show message: Panel or MessageText is not assigned.");
            return;
        }

        messageText.text = message;
        panel.SetActive(true);
        Debug.Log("MessageUI ShowMessage called. Message: " + message);
    }

    public void HideMessage()
    {
        if (panel == null)
        {
            Debug.LogError("Cannot hide message: Panel is not assigned.");
            return;
        }

        panel.SetActive(false);
        Debug.Log("MessageUI HideMessage called. Panel hidden.");
    }
}
