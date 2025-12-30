using UnityEngine;

[DisallowMultipleComponent]
public class OrbitCompanionWithAnim : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator companionAnimator;
    public Animator playerAnimator;
    public Rigidbody playerRigidbody;
    public CharacterController playerController;

    [Header("Animator Parameters")]
    public string playerSpeedParam = "Speed";
    public string companionSpeedParam = "Speed";

    [Header("Settings")]
    public float speedScale = 1f;
    public float animSmooth = 8f;

    [Header("Follow & Hover Settings")]
    public Vector3 followOffset = new Vector3(1.5f, 1.2f, 0f); // Initial follow offset
    public float yOffsetTransitionSpeed = 2f;
    public float hoverAmplitude = 0.25f;
    public float hoverSpeed = 2f;
    public float followSmooth = 5f;
    public float rotateSmooth = 5f;

    [Header("Control")]
    [Tooltip("Whether the companion should auto-rotate to face the player.")]
    public bool allowAutoRotation = true;

    [Header("Light Zone Interaction")]
    public string lightZoneTag = "Lighting";

    // Internal state
    private float animParamCurrent = 0f;
    private Vector3 lastPlayerPos;
    private float hoverTimer = 0f;
    private bool hasEnteredLightZone = false;

    void Start()
    {
        InitializeReferences();
    }

    void LateUpdate()
    {
        if (player == null) return;

        UpdateHoverFollow();

        if (allowAutoRotation)
            RotateToFacePlayer();

        SyncCompanionAnimation();
        UpdateLastPlayerPosition();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasEnteredLightZone && other.CompareTag(lightZoneTag))
        {
            Debug.Log("[OrbitCompanionWithAnim] Entered LightZone — directly setting followOffset.y = -1f");
            followOffset.y = -1f; // <-- Directly changing the offset
            hasEnteredLightZone = true;
        }
    }

    // ================================================================
    #region === INITIALIZATION ===
    private void InitializeReferences()
    {
        if (player == null)
            Debug.LogError("[OrbitCompanionWithAnim] Player not assigned.", this);

        lastPlayerPos = player != null ? player.position : Vector3.zero;

        if (companionAnimator == null)
            companionAnimator = GetComponent<Animator>();
    }
    #endregion

    // ================================================================
    #region === FOLLOW + HOVER ===
    private void UpdateHoverFollow()
    {
        if (player == null) return;

        // Update Y offset only after entering LightZone
        if (hasEnteredLightZone)
        {
            followOffset.y = -1f; // Directly apply new height
        }

        Vector3 desiredPos = player.position + player.TransformDirection(followOffset);
        hoverTimer += Time.deltaTime * hoverSpeed;
        desiredPos.y += Mathf.Sin(hoverTimer) * hoverAmplitude;

        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSmooth);
    }

    #endregion

    // ================================================================
    #region === FACING PLAYER ===
    private void RotateToFacePlayer()
    {
        if (player == null) return;

        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;

        if (lookDir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotateSmooth);
    }

    public void EnableAutoRotation(bool enable)
    {
        allowAutoRotation = enable;
    }

    public void DisconnectPlayer()
    {
        Debug.Log("[OrbitCompanionWithAnim] Player reference disconnected.");
        player = null;
    }
    #endregion

    // ================================================================
    #region === ANIMATION CONTROL ===
    private void SyncCompanionAnimation()
    {
        if (player == null) return;

        float playerSpeed = GetPlayerSpeed();
        float mappedSpeed = playerSpeed * speedScale;

        animParamCurrent = Mathf.Lerp(animParamCurrent, mappedSpeed, Time.deltaTime * animSmooth);

        if (companionAnimator != null)
            companionAnimator.SetFloat(companionSpeedParam, animParamCurrent);
    }
    #endregion

    // ================================================================
    #region === SPEED CALCULATION ===
    private float GetPlayerSpeed()
    {
        if (player == null) return 0f;

        if (playerAnimator != null && HasAnimatorParameter(playerAnimator, playerSpeedParam))
            return playerAnimator.GetFloat(playerSpeedParam);

        if (playerRigidbody != null)
        {
            Vector3 v = playerRigidbody.linearVelocity;
            v.y = 0;
            return v.magnitude;
        }

        if (playerController != null)
        {
            Vector3 v = playerController.velocity;
            v.y = 0;
            return v.magnitude;
        }

        Vector3 delta = (player.position - lastPlayerPos) / Mathf.Max(Time.deltaTime, 1e-6f);
        delta.y = 0;
        return delta.magnitude;
    }

    private bool HasAnimatorParameter(Animator anim, string paramName)
    {
        if (anim == null || string.IsNullOrEmpty(paramName)) return false;

        foreach (var p in anim.parameters)
            if (p.name == paramName) return true;

        return false;
    }
    #endregion

    // ================================================================
    #region === STATE UPDATES ===
    private void UpdateLastPlayerPosition()
    {
        if (player != null)
            lastPlayerPos = player.position;
    }
    #endregion
}
