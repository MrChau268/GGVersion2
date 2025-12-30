using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class PopupContentA
{
    public string title;
    [TextArea] public string message;
}

public class PopupController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI  titleText;
    [SerializeField] private TextMeshProUGUI  messageText;

    [Header("Popup Messages")]
    [SerializeField] private List<PopupContentA> popupMessages = new List<PopupContentA>();

    private int currentIndex = 0;
    private bool isPopupVisible = false;

    private void Start()
    {
        popupPanel.SetActive(false);
        Debug.Log("PopupController initialized. Total messages: " + popupMessages.Count);
    }

    // Call this method with a button click
    public void OnNextButtonClicked()
    {
        if (popupMessages.Count == 0)
        {
            Debug.LogWarning("No messages to show!");
            return;
        }

        if (!isPopupVisible)
        {
            isPopupVisible = true;
            popupPanel.SetActive(true);
            Debug.Log("Popup opened.");
        }

        ShowCurrentMessage();

        currentIndex++;

        if (currentIndex >= popupMessages.Count)
        {
            Debug.Log("Reached the end of messages. Closing popup.");
            currentIndex = 0;
            isPopupVisible = false;
            popupPanel.SetActive(false);
        }
    }

    private void ShowCurrentMessage()
    {
        if (popupMessages.Count == 0) return;

        PopupContentA content = popupMessages[currentIndex];
        titleText.text = content.title;
        messageText.text = content.message;

        Debug.Log("Showing message " + (currentIndex + 1) + "/" + popupMessages.Count + ": " 
                  + content.title + " -> " + content.message);
    }
}
