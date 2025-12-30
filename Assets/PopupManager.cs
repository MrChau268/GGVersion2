using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class PopupManager : MonoBehaviour
{
    [Header("References")]
    public FairyMessageDatabase database;
    public FairyType currentFairy;

    [Header("UI Elements")]
    public GameObject panel;
    public TMP_Text titleText;
    public TMP_Text messageText;

    [Header("Choice Buttons (Optional)")]
    public Button choiceButton1;
    public Button choiceButton2;
    public bool useChoiceButtons = true; // toggle if you want to show buttons

    [Header("Auto Message Settings")]
    public bool autoStart = true;
    public float timePerMessage = 5f; // time before moving to next message
    public float delayBeforeClose = 1f; // small delay before panel closes at end

    private List<FairyMessage> messages;
    private int currentIndex = -1;
    private bool isSessionEnded = false;
    private Coroutine autoFlowCoroutine;

    [Header("NPC Reference")]
    public FairyNPCController npcController;

    [Header("Screen FX (Blink)")]
    public Image fadeOverlay;
    public float blinkDuration = 0.4f;
    public Color blinkColor = Color.black;

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);

        if (choiceButton1 != null) choiceButton1.gameObject.SetActive(false);
        if (choiceButton2 != null) choiceButton2.gameObject.SetActive(false);

        messages = database != null ? database.GetMessages(currentFairy) : new List<FairyMessage>();

        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            fadeOverlay.color = new Color(blinkColor.r, blinkColor.g, blinkColor.b, 0f);
        }

        if (autoStart)
            StartAutoFlow();
    }

    // ✅ Automatically begin showing all messages
    public void StartAutoFlow()
    {
        if (autoFlowCoroutine != null)
            StopCoroutine(autoFlowCoroutine);

        autoFlowCoroutine = StartCoroutine(AutoFlowRoutine());
    }

    private IEnumerator AutoFlowRoutine()
    {
        if (messages == null || messages.Count == 0)
        {
            Debug.LogWarning("No messages found in the database for this fairy type!");
            yield break;
        }

        isSessionEnded = false;
        currentIndex = 0;

        if (panel != null)
            panel.SetActive(true);

        while (currentIndex < messages.Count)
        {
            DisplayMessage(currentIndex);
            AnimateText();

            Debug.Log($"Showing message {currentIndex + 1}/{messages.Count}");

            if (useChoiceButtons && currentIndex == 1)
            {
                ShowChoiceButtons();
                yield return new WaitForSeconds(timePerMessage + 2f); // slightly longer when choices appear
                HideChoiceButtons();
            }
            else
            {
                yield return new WaitForSeconds(timePerMessage);
            }

            currentIndex++;
        }

        yield return new WaitForSeconds(delayBeforeClose);
        ClosePanel();
    }

    private void ShowChoiceButtons()
    {
        if (!useChoiceButtons)
            return;

        if (choiceButton1 == null || choiceButton2 == null)
        {
            Debug.LogWarning("Choice buttons not assigned, skipping button display.");
            return;
        }

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
    }

    private void ClosePanel()
    {
        if (panel != null)
            panel.SetActive(false);

        HideChoiceButtons();
        isSessionEnded = true;

        StartCoroutine(PlayBlinkThenMoveFairy());
    }

    private IEnumerator PlayBlinkThenMoveFairy()
    {
        if (fadeOverlay != null)
        {
            fadeOverlay.color = new Color(blinkColor.r, blinkColor.g, blinkColor.b, 0f);
            yield return fadeOverlay.DOFade(1f, blinkDuration * 0.5f).WaitForCompletion();
            yield return new WaitForSeconds(0.05f);
            yield return fadeOverlay.DOFade(0f, blinkDuration * 0.5f).WaitForCompletion();
        }

        if (npcController != null)
        {
            npcController.ComeToPlayer();
            Debug.Log("Fairy is now moving toward the player (UI closed).");
        }
    }

    private void DisplayMessage(int index)
    {
        if (titleText != null)
            titleText.text = messages[index].title;

        if (messageText != null)
            messageText.text = messages[index].message;
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
            messageText.alpha = 0;
            messageText.DOFade(1f, 0.5f);
        }
    }
}
