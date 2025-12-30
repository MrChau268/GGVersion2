using System.Collections.Generic;
using UnityEngine;

public class NPCMessageManager : MonoBehaviour
{
    [Header("Fairy System")]
    public FairyMessageDatabase fairyDatabase;
    public FairyType defaultFairyType = FairyType.Fire;

    private List<FairyMessage> currentMessages;

    void Start()
    {
        if (fairyDatabase != null)
        {
            fairyDatabase.SetActiveFairy(defaultFairyType);
            currentMessages = fairyDatabase.GetActiveMessages();
        }
    }

    // Call this when player meets the NPC
    public void OnPlayerInteract()
    {
        if (currentMessages == null || currentMessages.Count == 0)
        {
            Debug.Log($"NPC '{name}' has no messages for {defaultFairyType} fairy.");
            return;
        }

        // Display first message in the UI (can expand to multiple later)
        MessageUI.instance.ShowMessage(currentMessages[0].message);
    }

    // Optional: hide messages
    public void OnPlayerLeave()
    {
        MessageUI.instance.HideMessage();
    }
}