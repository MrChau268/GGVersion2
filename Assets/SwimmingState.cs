using UnityEngine;

public class PlayerSwimState : MonoBehaviour
{
    [Header("References")]
    public Animator playerAnimator;        // Reference to your Animator
    public float floatOffset = 0.5f;       // How high above water (adjust as needed)

    private Vector3 originalPosition;      // To remember player's original Y position
    private bool isSwimming = false;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Water") && !isSwimming)
        {
            isSwimming = true;
            originalPosition = transform.position;


            // move player up slightly
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y + floatOffset,
                transform.position.z
            );


            if (playerAnimator != null)
            {
                playerAnimator.SetBool("IsSwimming", true);
            }
            else
            {
            }

        }
        else
        {
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Water") && isSwimming)
        {
            isSwimming = false;


            // move player back to roughly original Y
            transform.position = new Vector3(
                transform.position.x,
                originalPosition.y,
                transform.position.z
            );

            if (playerAnimator != null)
            {
                playerAnimator.SetBool("IsSwimming", false);
            }

        }
    }
}
