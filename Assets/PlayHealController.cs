using UnityEngine;
using UnityEngine.InputSystem;  // New input system namespace

public class PlayerHealthController : MonoBehaviour
{
    public HealthData healthData;
    private Keyboard keyboard;

    private void Start()
    {
        healthData.ResetHealth();
        keyboard = Keyboard.current;
    }

    private void Update()
    {
        if (keyboard.hKey.wasPressedThisFrame)
        {
            healthData.TakeDamage(10);
        }

        if (keyboard.jKey.wasPressedThisFrame)
        {
            healthData.Heal(5);
        }
    }
}
