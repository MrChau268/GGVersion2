using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "HealthData", menuName = "Character/Health Data")]
public class HealthData : ScriptableObject
{
    public int maxHealth = 100;
    [HideInInspector] public int currentHealth;

    public UnityEvent onHealthChanged;

    private void OnEnable()
    {
        currentHealth = maxHealth;
        if (onHealthChanged == null)
            onHealthChanged = new UnityEvent();
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        onHealthChanged.Invoke();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        onHealthChanged.Invoke();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        onHealthChanged.Invoke();
    }
}
