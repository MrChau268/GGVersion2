using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CinematicCameraTrigger : MonoBehaviour
{
    [Header("References")]
    public CinemachineCamera Main_PlayerCam;
    public CinemachineCamera ConverseCam;
    public CinemachineCamera SkyViewCam;
    public CinemachineCamera PlayerSwitchCam; // Optional transitional cam
    public Transform player;
    public Transform fairy;
    public Transform focusParent;

    [Header("Cinematic Settings")]
    public float blendDuration = 1.5f;
    public float transitionToThirdDuration = 1.5f;
    public float switchBackToConverseDuration = 1.5f;

    [Header("Focus Settings")]
    [Range(0f, 1f)]
    public float focusBiasToPlayer = 0.7f;
    public Vector3 focusOffset = new Vector3(0, 1.5f, 0);

    [Header("Conversation Flag")]
    public bool conversationEnded = false;

    private bool isCinematicActive = false;
    private Transform focusPoint;

    // 👇 Reference to Fairy NPC Controller
    [Header("NPC Reference")]
    public FairyNPCController fairyNPCController;

    void Start()
    {
        // Setup the focus point (used by ConverseCam)
        if (focusParent != null)
        {
            Transform existing = focusParent.Find("CinematicFocus");
            if (existing != null)
                focusPoint = existing;
            else
            {
                GameObject go = new GameObject("CinematicFocus");
                go.transform.SetParent(focusParent);
                focusPoint = go.transform;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player && !isCinematicActive)
        {
            StartCoroutine(StartCinematic());
        }
    }

    IEnumerator StartCinematic()
    {
        isCinematicActive = true;

        // Step 1: Switch from Main Player Cam to Converse Cam
        ConverseCam.Priority = Main_PlayerCam.Priority + 10;

        // Setup the cinematic focus point
        if (focusPoint != null)
        {
            Vector3 newFocusPos = Vector3.Lerp(fairy.position, player.position, focusBiasToPlayer);
            newFocusPos += focusOffset;
            focusPoint.position = newFocusPos;

            ConverseCam.LookAt = focusPoint;
            ConverseCam.Follow = focusPoint;
        }

        // Wait for the blend duration
        yield return new WaitForSeconds(blendDuration);

        // Wait until the conversation ends (set externally)
        yield return new WaitUntil(() => conversationEnded);

        // Step 2: Blend smoothly to SkyView
        yield return StartCoroutine(SwitchToSkyViewSmooth());

        isCinematicActive = false;
    }

    IEnumerator SwitchToSkyViewSmooth()
    {
        float elapsed = 0f;
        int basePriority = Main_PlayerCam.Priority;

        ConverseCam.Priority = basePriority + 10;
        SkyViewCam.Priority = basePriority;

        // Smooth blend
        while (elapsed < transitionToThirdDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionToThirdDuration);

            ConverseCam.Priority = Mathf.RoundToInt(Mathf.Lerp(basePriority + 10, basePriority - 10, t));
            SkyViewCam.Priority = Mathf.RoundToInt(Mathf.Lerp(basePriority, basePriority + 15, t));

            yield return null;
        }

        // Keep SkyViewCam higher temporarily
        ConverseCam.Priority = basePriority - 10;
        SkyViewCam.Priority = basePriority + 15;


        // Wait 3 seconds before giving ConverseCam higher priority
        yield return new WaitForSeconds(4f);

        ConverseCam.Priority = basePriority + 20;  // ConverseCam now higher
        SkyViewCam.Priority = basePriority + 5;    // SkyViewCam lowered


        // 👇 NEW: Trigger Fairy to move toward player
        if (fairyNPCController != null)
        {
            fairyNPCController.ComeToPlayer();
        }
        else
        {
            Debug.LogWarning("FairyNPCController not assigned in CinematicCameraTrigger!");
        }
    }

    public void SwitchBackToConverseCamera()
    {
        StartCoroutine(SwitchBackToConverseSmooth());
    }

    IEnumerator SwitchBackToConverseSmooth()
    {
        CinemachineCamera activeCam = GetActiveCamera();

        if (activeCam == null)
        {
            yield break;
        }

        if (activeCam == ConverseCam)
        {
            yield break;
        }

        int basePriority = Main_PlayerCam.Priority;
        float elapsed = 0f;

        // Raise ConverseCam priority
        ConverseCam.Priority = basePriority + 15;

        while (elapsed < switchBackToConverseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / switchBackToConverseDuration);

            // Smoothly lower the active camera
            int activeNewPriority = Mathf.RoundToInt(Mathf.Lerp(basePriority + 10, basePriority - 10, t));
            activeCam.Priority = activeNewPriority;

            // Smoothly raise ConverseCam
            int converseNewPriority = Mathf.RoundToInt(Mathf.Lerp(basePriority, basePriority + 20, t));
            ConverseCam.Priority = converseNewPriority;


            yield return null;
        }

        // Final assignment
        activeCam.Priority = basePriority - 10;
        ConverseCam.Priority = basePriority + 50;

    }

    CinemachineCamera GetActiveCamera()
    {
        CinemachineCamera[] allCams = { Main_PlayerCam, ConverseCam, SkyViewCam, PlayerSwitchCam };
        CinemachineCamera highest = null;
        int highestPriority = int.MinValue;

        foreach (var cam in allCams)
        {
            if (cam != null && cam.Priority > highestPriority)
            {
                highest = cam;
                highestPriority = cam.Priority;
            }
        }

        return highest;
    }

    public void TriggerCinematicFromButton()
    {
        if (!isCinematicActive)
        {
            StartCoroutine(StartCinematic());
        }
    }
}
