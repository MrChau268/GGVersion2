using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;

public class PopupControllerNPC : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;

    [Header("Popup Settings")]
    public float autoNextDelay = 3f;
    [SerializeField] private CameraSwitcherNPC cameraSwitcher;

    [Header("NPC Reference")]
    public FairyNPCController npcController;

    [Header("Popup Animation Settings")]
    public float popupInDuration = 0.5f;
    public float popupOutDuration = 0.35f;
    public Ease popupEaseIn = Ease.OutBack;
    public Ease popupEaseOut = Ease.InBack;

    [Header("Text Animation Settings")]
    public float typewriterSpeed = 0.03f; // Seconds per character

    private string title;
    private string[] messages;
    private int currentIndex = 0;
    private bool isShowing = false;
    private Coroutine messageRoutine;
    private Coroutine typewriterRoutine;

    private void Start()
    {
        gameObject.SetActive(false);
        transform.localScale = Vector3.zero; // hidden initially
    }

    private void Update()
    {
        if (isShowing && Input.GetKeyDown(KeyCode.N))
        {
            // If text is typing, finish it instantly
            if (typewriterRoutine != null)
            {
                StopCoroutine(typewriterRoutine);
                typewriterRoutine = null;
                messageText.text = messages[currentIndex];
            }
            else
            {
                ShowNextMessage();
            }
        }
    }

    public void ShowPopup(string title, string[] messages)
    {
        this.title = title;
        this.messages = messages;
        currentIndex = 0;

        titleText.text = title;
        messageText.text = "";

        gameObject.SetActive(true);
        transform.localScale = Vector3.zero; // reset scale
        isShowing = true;

        // ✨ Animate popup appearing
        transform.DOScale(1f, popupInDuration)
            .SetEase(popupEaseIn)
            .OnComplete(() =>
            {
                if (messageRoutine != null)
                    StopCoroutine(messageRoutine);
                messageRoutine = StartCoroutine(ShowMessages());
            });
    }

    private IEnumerator ShowMessages()
    {
        while (currentIndex < messages.Length)
        {
            // Start typewriter animation
            if (typewriterRoutine != null)
                StopCoroutine(typewriterRoutine);
            typewriterRoutine = StartCoroutine(TypewriterEffect(messages[currentIndex]));

            float elapsed = 0f;

            // Wait for auto delay or key press
            while (elapsed < autoNextDelay)
            {
                if (Input.GetKeyDown(KeyCode.N))
                    break;

                elapsed += Time.deltaTime;
                yield return null;
            }

            currentIndex++;
        }

        ClosePopup();
    }

    private IEnumerator TypewriterEffect(string fullText)
    {
        messageText.text = "";
        foreach (char c in fullText)
        {
            messageText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        typewriterRoutine = null;
    }

    private void ShowNextMessage()
    {
        currentIndex++;

        if (currentIndex >= messages.Length)
        {
            ClosePopup();
        }
        else
        {
            if (typewriterRoutine != null)
                StopCoroutine(typewriterRoutine);
            typewriterRoutine = StartCoroutine(TypewriterEffect(messages[currentIndex]));
        }
    }

    private void ClosePopup()
    {
        if (!isShowing) return;
        isShowing = false;

        // ✨ Animate popup disappearing
        transform.DOScale(0f, popupOutDuration)
            .SetEase(popupEaseOut)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);

                if (cameraSwitcher != null)
                    cameraSwitcher.ActivateThirdPersonCam();

                if (npcController != null)
                    npcController.StopMovement();
            });
    }
}
