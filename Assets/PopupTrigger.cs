using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

public class PopupTrigger : MonoBehaviour
{
    public enum PopupType
    {
        Fairy,
        MainNPC
    }

    [Header("Popup Type")]
    [SerializeField] private PopupType popupType = PopupType.MainNPC;

    [Header("Popup Settings")]
    [SerializeField] private GameObject popupUI;
    [SerializeField] private Transform player;
    [SerializeField] private float showDistance = 4f;

    [Header("Text Settings")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private float messageDelay = 5f;

    [Header("Popup Content")]
    [SerializeField] private List<PopupContent> fairyMessages = new List<PopupContent>();
    [SerializeField] private List<PopupContent> npcMessages = new List<PopupContent>();
    [SerializeField] private List<PopupContent> npcSecondMessages = new List<PopupContent>();

    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineCamera mainCam;
    [SerializeField] private CinemachineCamera firstPersonCam;
    [SerializeField] private CinemachineCamera thirdCam;

    [Header("Camera Transition Settings")]
    [SerializeField] private float thirdCamDelay = 2f; // how long after NPC popup before switching
    [SerializeField] private float returnToMainDelay = 2f; // how long thirdCam stays active before returning

    [Header("NPC Settings")]
    [SerializeField] private SimpleNPCFollower npcFollower;
    [SerializeField] private float npcStopDistance = 2f; // distance to player where NPC stops
    [Header("Light Settings")]
    [SerializeField] private GameObject lightObject; // The light you want to turn off
    private bool finalSequencePlayed = false;



    private Coroutine sequenceCoroutine;
    private Coroutine typingCoroutine;
    private bool isShown = false;
    private bool hasPlayed = false;

    void Start()
    {
        popupUI.SetActive(false);

        if (mainCam != null) mainCam.Priority = 20;
        if (firstPersonCam != null) firstPersonCam.Priority = 10;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (!isShown && !hasPlayed && distance <= showDistance)
        {
            ShowPopupSequence();
            hasPlayed = true;
        }
    }

    private void ShowPopupSequence()
    {
        isShown = true;

        if (sequenceCoroutine != null)
            StopCoroutine(sequenceCoroutine);

        // Select the correct list of messages
        List<PopupContent> selectedMessages =
            popupType == PopupType.Fairy ? fairyMessages : npcMessages;

        // 🔥 Turn off light if popup is Fairy type
        if (popupType == PopupType.Fairy && lightObject != null)
        {
            lightObject.SetActive(false);
        }

        sequenceCoroutine = StartCoroutine(ShowMessagesInSequence(selectedMessages));
    }


    private IEnumerator ShowMessagesInSequence(List<PopupContent> messages)
    {
        popupUI.SetActive(true);
        popupUI.transform.localScale = Vector3.zero;

        yield return popupUI.transform
            .DOScale(Vector3.one, 1f)
            .SetEase(Ease.OutBack)
            .WaitForCompletion();

        for (int i = 0; i < messages.Count; i++)
        {
            var content = messages[i];
            titleText.text = content.title;

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeMessage(content.message));
            yield return typingCoroutine;

            yield return new WaitForSeconds(messageDelay);
        }

        HidePopup();
    }

    private IEnumerator TypeMessage(string message)
    {
        messageText.text = "";
        foreach (char c in message)
        {
            messageText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    private void HidePopup()
    {
        isShown = false;

        if (sequenceCoroutine != null)
            StopCoroutine(sequenceCoroutine);
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        popupUI.transform
            .DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                popupUI.SetActive(false);
                titleText.text = "";
                messageText.text = "";

                if (popupType == PopupType.Fairy)
                {
                    // After Fairy popup → NPC starts moving toward player
                    if (npcFollower != null)
                        StartCoroutine(MoveNPCAndTriggerPopup());
                }
                else if (popupType == PopupType.MainNPC)
                {
                    // After NPC popup → switch to First Person camera
                    SwitchToFirstPersonCam();

                    // Then after a short delay, switch to third camera
                    StartCoroutine(SwitchToThirdCamSequence());
                }
            });
    }
    private IEnumerator SwitchToThirdCamSequence()
    {
        // Wait before switching to third camera
        yield return new WaitForSeconds(thirdCamDelay);
        SwitchToThirdCam();

        // Stay on the third camera for a bit
        yield return new WaitForSeconds(returnToMainDelay);
        SwitchBackToMainCam();

        // Fully stop all other cameras
        DisableAllCamerasExceptMain();

        // Wait 5 seconds before showing the second NPC messages
        yield return new WaitForSeconds(5f);

        // Show second NPC popup
        if (!finalSequencePlayed)
        {
            finalSequencePlayed = true;
            ShowSecondNpcPopup();
        }
    }


    private void DisableAllCamerasExceptMain()
    {
        if (firstPersonCam != null)
        {
            firstPersonCam.Priority = 0;
            firstPersonCam.gameObject.SetActive(false); // fully disable
        }

        if (thirdCam != null)
        {
            thirdCam.Priority = 0;
            thirdCam.gameObject.SetActive(false); // fully disable
        }

        if (mainCam != null)
        {
            mainCam.gameObject.SetActive(true);
            mainCam.Priority = 100; // ensure mainCam takes control
        }

        Debug.Log("✅ All cinematic cameras disabled — only MainCam active.");
    }


    private void ShowSecondNpcPopup()
    {
        Debug.Log("Showing second NPC popup after cinematic...");

        popupType = PopupType.MainNPC; // same NPC style

        if (sequenceCoroutine != null)
            StopCoroutine(sequenceCoroutine);

        // Use the new second messages
        sequenceCoroutine = StartCoroutine(ShowMessagesInSequence(npcSecondMessages));
    }




    private void SwitchToFirstPersonCam()
    {
        if (mainCam != null) mainCam.Priority = 10;
        if (firstPersonCam != null) firstPersonCam.Priority = 20;
        if (thirdCam != null) thirdCam.Priority = 5; // keep it low until needed
        Debug.Log("Switched to First Person camera.");
    }

    private void SwitchToThirdCam()
    {
        if (firstPersonCam != null) firstPersonCam.Priority = 10;
        if (thirdCam != null) thirdCam.Priority = 20;
        Debug.Log("Switched to Third Cinematic camera!");
    }

    private void SwitchBackToMainCam()
    {
        if (thirdCam != null) thirdCam.Priority = 10;
        if (mainCam != null) mainCam.Priority = 20;
        Debug.Log("Returned to Main Camera.");
    }


    private IEnumerator MoveNPCAndTriggerPopup()
    {
        // Make sure NPC is active
        if (npcFollower != null)
            npcFollower.BeginFollow();

        // Wait until NPC is within stop distance
        while (Vector3.Distance(npcFollower.transform.position, player.position) > npcStopDistance)
        {
            yield return null;
        }

        // Trigger NPC popup
        popupType = PopupType.MainNPC;
        ShowPopupSequence();
    }


}

[System.Serializable]
public class PopupContent
{
    public string title;
    [TextArea] public string message;
}
