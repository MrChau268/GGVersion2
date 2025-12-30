using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RandomWalkingNPC : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float rotationSpeed = 3f;
    public float walkRadius = 5f;       // Max distance from starting point
    public float waitTime = 2f;         // Pause at each target
    public LayerMask groundMask;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float waitTimer = 0f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;   // physics ON
        // rb.useGravity = true;     // gravity ON
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        startPosition = transform.position;
        ChooseNewTarget();
    }

    void Update()
    {
        if (!isMoving)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                ChooseNewTarget();
                waitTimer = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
            MoveTowardTarget();
    }

    void ChooseNewTarget()
    {
        // Pick a random point in circle on XZ plane
        Vector2 randomCircle = Random.insideUnitCircle * walkRadius;
        targetPosition = startPosition + new Vector3(randomCircle.x, 0, randomCircle.y);

        // Only move horizontally — Rigidbody handles Y
        targetPosition.y = transform.position.y; 

        isMoving = true;
    }

    void MoveTowardTarget()
    {
        Vector3 currentPos = transform.position;

        // direction only on XZ
        Vector3 direction = new Vector3(
            targetPosition.x - currentPos.x,
            0f,
            targetPosition.z - currentPos.z
        );

        float distance = direction.magnitude;

        if (distance < 0.1f)
        {
            isMoving = false;
            return;
        }

        direction.Normalize();

        // Move with Rigidbody (correct method)
        Vector3 newPos = currentPos + direction * speed * Time.fixedDeltaTime;
        newPos.y = currentPos.y;  // keep physics-controlled height

        rb.MovePosition(newPos);

        // Smooth rotation
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    public void StopRandomWalking()
    {
        isMoving = false;
        enabled = false;
    }
}
