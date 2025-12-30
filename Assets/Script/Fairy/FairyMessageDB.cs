using System.Collections.Generic;
using UnityEngine;


public class FairyMessageDatabase : MonoBehaviour
{
    [Header("All Fairy Messages")]
    public List<FairyMessageList> allFairyMessages = new List<FairyMessageList>();

    [Header("Current Active Fairy")]
    [SerializeField] private FairyType activeFairyType;

    private void Start()
    {
        this.enabled = false;  // Disable script when the scene starts
    }

    // Set which fairy type is currently active
    public void SetActiveFairy(FairyType type)
    {
        activeFairyType = type;

        foreach (var fairy in allFairyMessages)
        {
            // Only the chosen fairy type is active
            fairy.isActive = (fairy.fairyType == type);
        }

        Debug.Log($"Active Fairy set to: {activeFairyType}");
    }

    // Get messages for the currently active fairy
    public List<FairyMessage> GetActiveMessages()
    {
        FairyMessageList found = allFairyMessages.Find(f => f.fairyType == activeFairyType);
        return found != null ? found.messages : new List<FairyMessage>();
    }

    // Get messages for a specific fairy type (optional helper)
    public List<FairyMessage> GetMessages(FairyType type)
    {
        FairyMessageList found = allFairyMessages.Find(f => f.fairyType == type);
        return found != null ? found.messages : new List<FairyMessage>();
    }

    // Get which fairy type is currently active
    public FairyType GetActiveFairyType()
    {
        return activeFairyType;
    }
}

