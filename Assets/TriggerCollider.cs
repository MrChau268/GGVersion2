using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        NPCMessageManager npc = other.GetComponent<NPCMessageManager>();
        if (npc != null)
        {
            npc.OnPlayerInteract();
        }
    }

    void OnTriggerExit(Collider other)
    {
        NPCMessageManager npc = other.GetComponent<NPCMessageManager>();
        if (npc != null)
        {
            npc.OnPlayerLeave();
        }
    }
}
