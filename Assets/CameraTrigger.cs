using UnityEngine;
using UnityEngine.UI;

public class CameraTriggerButton : MonoBehaviour
{
    [Header("Camera Trigger Reference")]
    public CinematicCameraTrigger cameraTrigger;

    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        if (btn == null)
        {
            Debug.LogError($"No Button component found on {gameObject.name}!");
            return;
        }

        btn.onClick.AddListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        if (btn != null)
            btn.onClick.RemoveListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (cameraTrigger == null)
        {
            Debug.LogWarning($"{gameObject.name} has no CinematicCameraTrigger assigned!");
            return;
        }

        Debug.Log($"{gameObject.name} clicked — triggering cinematic...");
        cameraTrigger.TriggerCinematicFromButton();
    }
}
