using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text titleText;    // TextMeshPro for title
    public TMP_Text messageText;  // TextMeshPro for message
    public Button nextButton;

    private NPCDialogue currentDialogue;
    private int messageIndex;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        dialoguePanel.SetActive(false);
        nextButton.onClick.AddListener(NextMessage);
    }

    public void StartDialogue(NPCDialogue dialogue)
    {
        currentDialogue = dialogue;
        messageIndex = 0;
        dialoguePanel.SetActive(true);
        ShowMessage();
    }

    private void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.N))
        {
            NextMessage();
        }
    }

    void ShowMessage()
    {
        if (currentDialogue == null || currentDialogue.messages.Length == 0)
            return;

        titleText.text = currentDialogue.title;
        messageText.text = currentDialogue.messages[messageIndex];
    }

    public void NextMessage()
    {
        messageIndex++;

        if (messageIndex >= currentDialogue.messages.Length)
        {
            EndDialogue();
        }
        else
        {
            ShowMessage();
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentDialogue = null;
        messageIndex = 0;
    }
}
