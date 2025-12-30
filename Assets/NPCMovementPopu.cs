using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleNPCFollower : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;
    public float speed = 2f;
    public float stopDistance = 1.5f;
    public float resumeDistance = 3f;

    [Header("Grounding Settings")]
    public float groundCheckDistance = 5f;
    public LayerMask groundMask = Physics.DefaultRaycastLayers;

    [Header("Highlight Settings")]
    public GameObject highlightMesh;   // assign glow mesh or outline object
    public float highlightDistance = 3f;

    private bool shouldFollow = false;
    private bool isClose = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        if (highlightMesh != null)
            highlightMesh.SetActive(false);
    }

    void LateUpdate()
    {
        if (player == null)
            return;

        HandleHighlight();

        if (!shouldFollow)
            return;

        FollowLogic();
    }

    // --- Highlight logic ---
    private void HandleHighlight()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (highlightMesh != null)
        {
            // Turn highlight on when within range, off when too far
            bool shouldHighlight = distance <= highlightDistance;
            if (highlightMesh.activeSelf != shouldHighlight)
                highlightMesh.SetActive(shouldHighlight);
        }
    }

    // --- Follow logic ---
    private void FollowLogic()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= stopDistance)
            isClose = true;
        else if (distance > resumeDistance)
            isClose = false;

        if (!isClose)
            MoveTowardPlayer(distance);
        else
            RotateTowardPlayer();

        KeepGrounded();
    }

    private void MoveTowardPlayer(float distance)
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        direction.Normalize();

        float step = speed * Time.deltaTime;
        if (distance - step < stopDistance)
            step = distance - stopDistance;

        transform.position += direction * step;
        RotateTowardPlayer();
    }

    private void RotateTowardPlayer()
    {
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    private void KeepGrounded()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y;
            transform.position = pos;
        }
    }

    // Public controls
    public void BeginFollow()
    {
        shouldFollow = true;
        Debug.Log($"{name} is now following the player.");
    }

    public void StopFollow()
    {
        shouldFollow = false;
        Debug.Log($"{name} stopped following the player.");
    }
}
