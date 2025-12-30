using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "StaminaData", menuName = "Character/StaminaData", order = 2)]
public class StaminaData : ScriptableObject
{
    public int maxStamina = 100;
    public int currentStamina;

    public float regenRate = 5f; 

    public UnityEvent onStaminaChanged;

    private void OnEnable()
    {
        currentStamina = maxStamina;
    }

    public void ConsumeStamina(int amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0) currentStamina = 0;

        onStaminaChanged?.Invoke();
    }

    public void RegenerateStamina(float deltaTime)
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += Mathf.RoundToInt(regenRate * deltaTime);
            if (currentStamina > maxStamina)
                currentStamina = maxStamina;

            onStaminaChanged?.Invoke();
        }
    }

    public void ResetStamina()
    {
        currentStamina = maxStamina;
        onStaminaChanged?.Invoke();
    }
}
