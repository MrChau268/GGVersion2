using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageUIManager : MonoBehaviour
{
    public static MessageUIManager Instance;

    public CanvasGroup canvasGroup;
    public TMP_Text messageText;
    public RectTransform messageRectTransform;


    private float timer;

    void Awake()
    {
        Instance = this;
        canvasGroup.alpha = 0; // hidden by default
        Instance = this;


    }

    void Update()
    {
        if (canvasGroup.alpha > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                canvasGroup.alpha = 0;
            }
        }
    }

    // public void ShowMessage(ShowMessageItems data)
    // {
    //     messageText.text = data.message;
    //     messageText.color = data.textColor;

    //     canvasGroup.alpha = 1;
    //     timer = data.duration;
    // }

    public void ShowMessage(ShowMessageItems data, Vector3 worldPosition)
    {
        messageText.text = data.message;
        messageText.color = data.textColor;

        // Position above object
        messageRectTransform.position = worldPosition + new Vector3(0, 1.5f, 0);

        // Show
        canvasGroup.alpha = 1;
        timer = data.duration;
    }





}
