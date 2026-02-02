using UnityEngine;

public class GPopupTrigger : MonoBehaviour
{
    [TextArea]
    [SerializeField]
    protected string message = "Hello popup!";

    protected bool autoClose;
    protected float autoCloseTime = 2f;

    bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by: {other.name}");
        if (triggered) return;
        if (!other.CompareTag("Player"))
        {
            Debug.Log("Not player – ignored");
            return;
        }
        triggered = true;

        GPopupManager.Instance.ShowPopupData(
            new GPopupData(
                message,
                autoClose ? autoCloseTime : 0f,
                onCancel: () => Debug.Log("Popup canceled"),
                onConfirm: () => Debug.Log("Popup confirmed")
            )
        );
    }
}
