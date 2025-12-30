using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StaminaUI : MonoBehaviour
{
    public StaminaData staminaData;

    [Header("UI")]
    public Image staminaFillImage;                
    public TextMeshProUGUI staminaText;           
    [Header("Tween")]
    public float tweenDuration = 0.3f;           

    private void OnEnable()
    {
        if (staminaData != null)
            staminaData.onStaminaChanged.AddListener(UpdateUI);
    }

    private void OnDisable()
    {
        if (staminaData != null)
            staminaData.onStaminaChanged.RemoveListener(UpdateUI);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        float fill = (float)staminaData.currentStamina / staminaData.maxStamina;

        staminaFillImage.DOFillAmount(fill, tweenDuration).SetEase(Ease.OutQuad);

        if (staminaText != null)
            staminaText.text = $"{staminaData.currentStamina}/{staminaData.maxStamina}";
    }
}
