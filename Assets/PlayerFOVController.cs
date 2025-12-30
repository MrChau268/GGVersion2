using UnityEngine;
using System.Collections;

public class PlayerFOVController_Debug : MonoBehaviour
{
    [Header("FOV Settings")]
    public Camera playerCamera;
    public float normalFOV = 100f;
    public float zoomFOV = 50f;
    public float transitionDuration = 0.4f; // seconds for zoom animation
    public float lockDuration = 10f;

    [Header("Trigger (Proximity)")]
    [Tooltip("If assigned, this Transform is used as the trigger center. If empty, manualRootPosition is used.")]
    public Transform movementRoot;
    [Tooltip("Used when movementRoot is empty.")]
    public Vector3 manualRootPosition;
    [Tooltip("Trigger radius (units). When player is within this distance from root, FOV triggers.")]
    public float triggerRadius = -5f;

    [Header("Disable While Locked")]
    public MonoBehaviour[] componentsToDisable;

    [Header("Lock Player Movement")]
    [Tooltip("Movement scripts (e.g. CharacterController, PlayerMovement, etc.) to disable while locked.")]
    public MonoBehaviour[] playerMovementComponents;

    [Header("Look At Target")]
    [Tooltip("Who the player should look at (usually the Fairy)")]
    public Transform lookAtTarget;
    [Tooltip("Should the target also look at the player?")]
    public bool makeTargetLookAtPlayer = true;

    [Header("Debug")]
    public bool verbose = true;
    [Tooltip("How often (sec) to emit repeating status logs while zoom animates.")]
    public float debugLogInterval = 0.5f;

    // internals
    private bool isLocked = false;
    private float lastLogTime = 0f;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (playerCamera != null) playerCamera.fieldOfView = normalFOV;
        Log($"[Start] playerCamera={(playerCamera != null)}, movementRoot={(movementRoot != null ? movementRoot.name : "null (manual)")} manualPos={manualRootPosition} triggerRadius={triggerRadius}");
    }

    void Update()
    {
        // Manual trigger for quick testing
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Log("[Input] Manual trigger (Z) pressed.");
            TryStartLockAndZoom();
        }

        // Check proximity to root regularly
        if (!isLocked)
        {
            Vector3 rootPos = GetRootPosition();
            float distToRoot = Vector3.Distance(transform.position, rootPos);

            if (verbose && Time.time - lastLogTime >= 1f)
            {
                Log($"[Status] PlayerPos={RoundVec(transform.position)} RootPos={RoundVec(rootPos)} Distance={distToRoot:F3}");
                lastLogTime = Time.time;
            }

            if (distToRoot <= triggerRadius)
            {
                Log($"[Trigger] Within range ({distToRoot:F3} <= {triggerRadius}). Starting LockAndZoom.");
                TryStartLockAndZoom();
            }
        }
    }

    private void TryStartLockAndZoom()
    {
        if (!isLocked)
        {
            StartCoroutine(LockAndZoomCoroutine());
        }
        else
        {
            Log("[TryStart] Already locked, ignoring request.");
        }
    }

    private Vector3 GetRootPosition()
    {
        return movementRoot != null ? movementRoot.position : manualRootPosition;
    }

    private IEnumerator LockAndZoomCoroutine()
    {
        isLocked = true;
        Log("[Coroutine] LockAndZoom started.");

        // Disable movement scripts
        foreach (var comp in playerMovementComponents)
        {
            if (comp != null)
            {
                comp.enabled = false;
                Log($"[Movement] Disabled {comp.GetType().Name}");
            }
        }

        // Disable extra components
        if (componentsToDisable != null)
        {
            foreach (var comp in componentsToDisable)
            {
                if (comp != null)
                {
                    comp.enabled = false;
                    Log($"[Disable] {comp.GetType().Name} disabled.");
                }
            }
        }

        // Zoom In + LookAt
        if (playerCamera == null)
        {
            Log("[ERROR] playerCamera is null. Assign a Camera in inspector or have a Camera tagged MainCamera.", true);
        }
        else
        {
            float timer = 0f;
            float startFOV = playerCamera.fieldOfView;
            Quaternion startRotation = transform.rotation;
            Quaternion targetRotation = lookAtTarget != null
                ? Quaternion.LookRotation((lookAtTarget.position - transform.position).normalized)
                : startRotation;

            Quaternion targetReverseRotation = Quaternion.identity;
            Quaternion targetStartRotation = Quaternion.identity;
            if (makeTargetLookAtPlayer && lookAtTarget != null)
            {
                targetStartRotation = lookAtTarget.rotation;
                targetReverseRotation = Quaternion.LookRotation((transform.position - lookAtTarget.position).normalized);
            }

            Log($"[ZoomIn] from {startFOV:F2} to {zoomFOV:F2} (duration {transitionDuration}s).");

            while (timer < transitionDuration)
            {
                timer += Time.deltaTime;
                float t = timer / Mathf.Max(0.0001f, transitionDuration);

                // Zoom
                playerCamera.fieldOfView = Mathf.Lerp(startFOV, zoomFOV, t);

                // Rotate player toward target
                if (lookAtTarget != null)
                    transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

                // Optional: Rotate target toward player
                if (makeTargetLookAtPlayer && lookAtTarget != null)
                    lookAtTarget.rotation = Quaternion.Slerp(targetStartRotation, targetReverseRotation, t);

                if (verbose && Time.time - lastLogTime >= debugLogInterval)
                {
                    Log($"[ZoomIn] FOV={playerCamera.fieldOfView:F2}, T={t:F2}");
                    lastLogTime = Time.time;
                }

                yield return null;
            }

            playerCamera.fieldOfView = zoomFOV;
            transform.rotation = targetRotation;
            if (makeTargetLookAtPlayer && lookAtTarget != null)
                lookAtTarget.rotation = targetReverseRotation;

            Log("[ZoomIn] complete.");
        }

        // Wait during lock
        Log($"[Locked] Waiting for lockDuration = {lockDuration}s.");
        yield return new WaitForSeconds(lockDuration);

        // Zoom Out
        if (playerCamera != null)
        {
            float timer2 = 0f;
            float startFOV2 = playerCamera.fieldOfView;
            Log($"[ZoomOut] from {startFOV2:F2} to {normalFOV:F2} (duration {transitionDuration}s).");

            while (timer2 < transitionDuration)
            {
                timer2 += Time.deltaTime;
                float t = timer2 / Mathf.Max(0.0001f, transitionDuration);
                playerCamera.fieldOfView = Mathf.Lerp(startFOV2, normalFOV, t);

                if (verbose && Time.time - lastLogTime >= debugLogInterval)
                {
                    Log($"[ZoomOut] FOV={playerCamera.fieldOfView:F2}");
                    lastLogTime = Time.time;
                }

                yield return null;
            }

            playerCamera.fieldOfView = normalFOV;
            Log("[ZoomOut] complete.");
        }

        // Re-enable movement
        foreach (var comp in playerMovementComponents)
        {
            if (comp != null)
            {
                comp.enabled = true;
                Log($"[Movement] Re-enabled {comp.GetType().Name}");
            }
        }

        // Re-enable extra components
        if (componentsToDisable != null)
        {
            foreach (var comp in componentsToDisable)
            {
                if (comp != null)
                {
                    comp.enabled = true;
                    Log($"[Enable] {comp.GetType().Name} re-enabled.");
                }
            }
        }

        isLocked = false;
        Log("[Coroutine] LockAndZoom finished.");
    }

    private void Log(string msg, bool force = false)
    {
        if (verbose || force){}
            // Debug.Log(msg, this);
    }

    private void OnDrawGizmos()
    {
        Vector3 rootPos = movementRoot != null ? movementRoot.position : manualRootPosition;
        bool inside = Application.isPlaying ? (Vector3.Distance(transform.position, rootPos) <= triggerRadius) : false;

        Gizmos.color = inside ? new Color(1f, 0.2f, 0.2f, 0.35f) : new Color(0.2f, 1f, 0.2f, 0.15f);
        Gizmos.DrawSphere(rootPos, Mathf.Max(0.02f, triggerRadius));
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(rootPos, triggerRadius);
    }

    private string RoundVec(Vector3 v)
    {
        return $"({v.x:F2},{v.y:F2},{v.z:F2})";
    }
}
