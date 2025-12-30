using UnityEngine;

public class FairyNPCController : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float stopDistance = 1.5f;


    private bool shouldMove = false;
    public RandomWalkingNPC randomWalkingNPC;

    private void Update()
    {
        if (player == null || !shouldMove)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            // Move towards player smoothly
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Rotate to face player
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
        else
        {
            // Close enough, stop moving
            shouldMove = false;
            // Optionally do something here, e.g. notify that NPC reached the player
        }
    }

    // Call this method to make the NPC start moving toward the player
    public void ComeToPlayer()
    {
        shouldMove = true;
        if (randomWalkingNPC != null)
        {
            randomWalkingNPC.StopRandomWalking();

        }
    }

    // Stops movement immediately
    public void StopMovement()
    {
        shouldMove = false;

        // If you want, also stop random walking
        if (randomWalkingNPC != null)
        {
            randomWalkingNPC.StopRandomWalking();
        }
    }


}
