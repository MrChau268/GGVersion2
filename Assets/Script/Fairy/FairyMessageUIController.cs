using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class FairyMessageUI : MonoBehaviour
{
    [Header("References")]
    public FairyMessageDatabase database;
    public FairyType currentFairy;
    public CinematicCameraTrigger cameraTrigger;

    [Header("UI Elements")]
    public GameObject panel;
    public TMP_Text titleText;
    public TMP_Text messageText;

    [Header("Choice Buttons")]
    public Button choiceButton1;
    public Button choiceButton2;

    private List<FairyMessage> messages;
    private int currentIndex = -1;
    private Coroutine closeCoroutine;
    private bool isSessionEnded = false;

    [Header("NPC Reference")]
    public FairyNPCController npcController;

    [Header("Cinematic Settings")]
    public bool triggerCameraOnStart = true;
    public float delayBeforeCameraEnd = 1f;

    [Header("Screen FX (Blink)")]
    public Image fadeOverlay;
    public float blinkDuration = 0.4f;
    public Color blinkColor = Color.black;

    private bool waitingForNPC = false;

    [Header("Text Animation Settings")]
    public float typewriterSpeed = 0.03f; // seconds per character
    private Coroutine typewriterCoroutine;

    [Header("Popup Animation Settings")]
    public float popupInDuration = 2f;
    public float popupOutDuration = 0.35f;
    public Ease popupEaseIn = Ease.OutBack;
    public Ease popupEaseOut = Ease.InBack;

    private bool isActivated = false;
    public GameObject movementLockPrefab;
    private GameObject movementLockInstance;



    private void Start()
    {
        if (panel != null)
        {
            panel.SetActive(false);
            panel.transform.localScale = Vector3.zero; // Start hidden
        }

        if (choiceButton1 != null) choiceButton1.gameObject.SetActive(false);
        if (choiceButton2 != null) choiceButton2.gameObject.SetActive(false);

        messages = database.GetMessages(currentFairy);

        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            fadeOverlay.color = new Color(blinkColor.r, blinkColor.g, blinkColor.b, 0f);
        }
    }

    private void Update()
    {
        if (!isActivated) return;
        if (Input.GetKeyDown(KeyCode.N))
        {
            ShowNextMessage();
        }
    }

    private void AnimateText()
    {
        if (titleText != null)
        {
            titleText.alpha = 0;
            titleText.DOFade(1f, 0.5f);
        }

        if (messageText != null)
        {
            messageText.alpha = 1f;
            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = StartCoroutine(TypewriterEffect(messages[currentIndex].message));
        }
    }

    private IEnumerator TypewriterEffect(string fullText)
    {
        messageText.text = "";
        foreach (char c in fullText)
        {
            messageText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }

    public void ShowNextMessage()
    {
        if (!isActivated)
            return;
        // If typing is still running, skip to full message instead of next
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
            messageText.text = messages[currentIndex].message;
            return;
        }

        if (isSessionEnded || messages == null || messages.Count == 0)
            return;

        // If no message has been shown yet, start with first message
        if (currentIndex < 0)
        {
            currentIndex = 0;
        }
        else
        {
            currentIndex++;
        }

        if (currentIndex >= messages.Count)
        {
            ClosePanel();
            return;
        }

        if (panel != null && !panel.activeSelf)
        {
            movementLockInstance = Instantiate(movementLockPrefab);
            panel.SetActive(true);
            panel.transform.localScale = Vector3.zero;

            // 👇 Scale bounce-in animation
            panel.transform.DOScale(1f, popupInDuration)
                .SetEase(popupEaseIn)
                .OnComplete(() =>
                {
                    DisplayMessage(currentIndex);
                    AnimateText();
                });
            return;
        }

        DisplayMessage(currentIndex);
        AnimateText();

        if (currentIndex == 0)
        {
            // First message → show choice buttons
        }
        else if (currentIndex == 1)
        {
            ShowChoiceButtons();

            if (closeCoroutine != null)
                StopCoroutine(closeCoroutine);
            closeCoroutine = StartCoroutine(AutoCloseAfterDelay(5f));
        }
        else
        {
            HideChoiceButtons();
        }
    }

    private void ShowChoiceButtons()
    {
        if (choiceButton1 == null || choiceButton2 == null) return;

        choiceButton1.gameObject.SetActive(true);
        choiceButton2.gameObject.SetActive(true);

        choiceButton1.transform.localScale = Vector3.zero;
        choiceButton2.transform.localScale = Vector3.zero;

        choiceButton1.transform.DOScale(1f, 0.4f)
            .SetEase(Ease.OutBack)
            .SetDelay(0.1f);

        choiceButton2.transform.DOScale(1f, 0.4f)
            .SetEase(Ease.OutBack)
            .SetDelay(0.25f);

        choiceButton1.onClick.RemoveAllListeners();
        choiceButton2.onClick.RemoveAllListeners();

        choiceButton1.onClick.AddListener(() => OnChoiceSelected(1));
        choiceButton2.onClick.AddListener(() => OnChoiceSelected(2));
    }

    private void HideChoiceButtons()
    {
        if (choiceButton1 != null && choiceButton1.gameObject.activeSelf)
        {
            choiceButton1.transform.DOScale(0f, 0.25f)
                .SetEase(Ease.InBack)
                .OnComplete(() => choiceButton1.gameObject.SetActive(false));
        }

        if (choiceButton2 != null && choiceButton2.gameObject.activeSelf)
        {
            choiceButton2.transform.DOScale(0f, 0.25f)
                .SetEase(Ease.InBack)
                .OnComplete(() => choiceButton2.gameObject.SetActive(false));
        }
    }

    private void OnChoiceSelected(int buttonIndex)
    {
        Debug.Log($"Choice Button {buttonIndex} clicked.");
        HideChoiceButtons();
        ShowNextMessage();
    }

    private IEnumerator AutoCloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClosePanel();
    }

    private void ClosePanel()
    {
        if (panel == null) return;

        HideChoiceButtons();
        currentIndex = -1;
        isSessionEnded = true;
        Destroy(movementLockInstance);


        // 👇 Scale out animation
        panel.transform.DOScale(0f, popupOutDuration)
            .SetEase(popupEaseOut)
            .OnComplete(() =>
            {
                panel.SetActive(false);

                if (cameraTrigger != null)
                    StartCoroutine(PlayBlinkThenSwitchCamera());

                if (npcController != null)
                    npcController.ComeToPlayer();
            });
    }
    public void ActivateFairyDialogue()
    {
        isActivated = true;
        isSessionEnded = false;
        currentIndex = -1;
        ShowNextMessage();
    }

    private IEnumerator PlayBlinkThenSwitchCamera()
    {
        if (fadeOverlay != null)
        {
            fadeOverlay.color = new Color(blinkColor.r, blinkColor.g, blinkColor.b, 0f);
            yield return fadeOverlay.DOFade(1f, blinkDuration * 0.5f).WaitForCompletion();
            yield return new WaitForSeconds(0.05f);
            yield return fadeOverlay.DOFade(0f, blinkDuration * 0.5f).WaitForCompletion();
        }

        yield return new WaitForSeconds(delayBeforeCameraEnd);

        if (cameraTrigger != null)
            cameraTrigger.conversationEnded = true;
    }

    private void DisplayMessage(int index)
    {
        if (titleText != null)
            titleText.text = messages[index].title;

        if (messageText != null)
            messageText.text = messages[index].message;
    }
}
