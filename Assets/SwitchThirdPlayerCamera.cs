using UnityEngine;
using Unity.Cinemachine;

public class CameraSwitcherNPC : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineCamera conversationCam;
    public CinemachineCamera thirdPersonCam;

    [Header("Cameras Priority")]
    public int conversationPriority = 20;
    public int thirdPersonPriority = 10;


    public void ActivateThirdPersonCam()
    {
        if (conversationCam != null && thirdPersonCam != null)
        {
            conversationCam.Priority = thirdPersonPriority; // lower priority
            thirdPersonCam.Priority = conversationPriority; // higher priority
        }
    }
}
