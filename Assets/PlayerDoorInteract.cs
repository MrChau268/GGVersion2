using UnityEngine;

namespace Suntail
{
    public class PlayerInteractTrigger : MonoBehaviour
    {
        [SerializeField] private KeyCode interactKey = KeyCode.E;

        private Door currentDoor;

        private void OnTriggerEnter(Collider other)
        {
            Door door = other.GetComponentInParent<Door>();
            if (door != null)
            {
                currentDoor = door;
                Debug.Log("Entered door trigger: " + door.name);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Door door = other.GetComponentInParent<Door>();

            // If "door" is null, stop here — prevents NullReferenceException
            if (door == null)
                return;

            if (door == currentDoor)
            {
                currentDoor = null;
                Debug.Log("Exited door trigger: " + door.name);
            }
        }


        private void Update()
        {
            if (currentDoor != null)
            {
                if (Input.GetKeyDown(interactKey))
                {
                    Debug.Log("Interacting with door: " + currentDoor.name);
                    currentDoor.PlayDoorAnimation();
                }
            }
        }
    }
}
