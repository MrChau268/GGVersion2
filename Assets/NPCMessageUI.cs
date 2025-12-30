using UnityEngine;

public class NPCMessageUI : MonoBehaviour
{
    public NPCDialogue dialogue;
    private int currentMessageIndex = 0;
    private bool isPlayerInteracting = false;

    void Update()
    {
        if (!isPlayerInteracting)
            return;

        if (dialogue == null || dialogue.messages.Length == 0)
            return;

        if (Input.GetKeyDown(KeyCode.N) || Input.GetMouseButtonDown(0))
        {
            ShowNextMessage();
        }
    }

    void ShowNextMessage()
    {
        if (currentMessageIndex >= dialogue.messages.Length)
        {
            Debug.Log("End of dialogue.");
            isPlayerInteracting = false; // End interaction
            return;
        }

        Debug.Log($"[{dialogue.title}] {dialogue.messages[currentMessageIndex]}");
        currentMessageIndex++;
    }

    // Called when player interacts with NPC
    public void StartDialogue(NPCDialogue newDialogue)
    {
        dialogue = newDialogue;
        currentMessageIndex = 0;
        isPlayerInteracting = true;
        ShowNextMessage();
    }

    // Called when player leaves or cancels
    public void EndDialogue()
    {
        isPlayerInteracting = false;
    }
}
