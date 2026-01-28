using System.Collections.Generic;
using UnityEngine;

public class GPopupManager : MonoBehaviour
{
    public static GPopupManager Instance;
    [SerializeField]
    protected GPopupUI popupPrefab;

    Queue<GPopupData> popupQueue = new Queue<GPopupData>();
    GPopupUI currentPopup;

    protected void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowPopupData(GPopupData data)
    {
        popupQueue.Enqueue(data);

        if (currentPopup == null || currentPopup.gameObject.activeSelf)
        {
            ShowNext();
        }
    }

    public void ShowNext()
    {
        if (popupQueue.Count == 0)
        {
            return;
        }

        if (currentPopup == null)
        {
            currentPopup = Instantiate(popupPrefab, FindAnyObjectByType<Canvas>().transform);

        }
        currentPopup.ShowPopup(popupQueue.Dequeue());
    }

    public void NotifyPopupClosed()
    {
        ShowNext();
    }

}
