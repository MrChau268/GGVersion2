using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public MovingObjectData data;

    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        // pick random start position in range
        float x = Random.Range(data.rangeX.x, data.rangeX.y);
        float z = Random.Range(data.rangeZ.x, data.rangeZ.y);
        startPos = new Vector3(x, 0, z);
        transform.position = startPos;

        PickNewTarget();
    }

    void Update()
    {
        // move toward target
        transform.position = Vector3.MoveTowards(transform.position, targetPos, data.moveSpeed * Time.deltaTime);

        // reached target? pick another
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            PickNewTarget();
        }
    }

    void PickNewTarget()
    {
        // pick random direction within moveRadius
        Vector2 circle = Random.insideUnitCircle * data.moveRadius;
        targetPos = startPos + new Vector3(circle.x, 0, circle.y);
    }
}
