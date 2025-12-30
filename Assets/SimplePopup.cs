using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class SimpleNPCMessageUI : MonoBehaviour
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
    public bool useChoiceButtons = true; // ✅ Toggle buttons on/off easily in Inspector

    [Header("NPC Reference (Optional)")]
    public FairyNPCController npcController;

    private List<FairyMessage> messages;
    private int currentIndex = -1;
    private bool isSessionEnded = false;
    private Coroutine closeCoroutine;

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);

        // Hide buttons if they exist
        if (choiceButton1 != null) choiceButton1.gameObject.SetActive(false);
        if (choiceButton2 != null) choiceButton2.gameObject.SetActive(false);

        messages = database.GetMessages(currentFairy);
    }

    private void Update()
    {
        // For quick testing — press N to go to next line
        if (Input.GetKeyDown(KeyCode.N))
        {
            ShowNextMessage();
        }
    }

    public void StartConversation()
    {
        isSessionEnded = false;
        currentIndex = -1;
        ShowNextMessage();
    }

    public void ShowNextMessage()
    {
        if (isSessionEnded || messages == null || messages.Count == 0)
            return;

        // Step message index
        currentIndex = (currentIndex < 0) ? 0 : currentIndex + 1;

        // Reached end
        if (currentIndex >= messages.Count)
        {
            ClosePanel();
            return;
        }

        // Activate panel
        if (panel != null && !panel.activeSelf)
            panel.SetActive(true);

        // Update UI text
        DisplayMessage(currentIndex);
        AnimateText();

        // Show or hide buttons depending on settings
        if (useChoiceButtons && currentIndex == 1)
        {
            ShowChoiceButtons();

            // Optional auto close
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
        // If buttons not assigned or feature disabled, skip
        if (!useChoiceButtons || choiceButton1 == null || choiceButton2 == null)
            return;

        choiceButton1.gameObject.SetActive(true);
        choiceButton2.gameObject.SetActive(true);

        // Animate buttons
        choiceButton1.transform.localScale = Vector3.zero;
        choiceButton2.transform.localScale = Vector3.zero;

        choiceButton1.transform.DOScale(1f, 0.4f)
            .SetEase(Ease.OutBack)
            .SetDelay(0.1f);

        choiceButton2.transform.DOScale(1f, 0.4f)
            .SetEase(Ease.OutBack)
            .SetDelay(0.25f);

        // Setup listeners
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
        if (panel != null)
            panel.SetActive(false);

        HideChoiceButtons();
        currentIndex = -1;
        isSessionEnded = true;

        // Optional: Make NPC move to player when dialogue ends
        if (npcController != null)
            npcController.ComeToPlayer();
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
