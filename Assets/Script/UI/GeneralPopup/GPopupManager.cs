using System.Collections.Generic;
using UnityEngine;

public class GPopupManager : MonoBehaviour
{
    public static GPopupManager Instance;

    [SerializeField] private Transform popupParent;

    private Stack<GPopupUI> popupStack = new Stack<GPopupUI>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public T ShowPopup<T>(T popupPrefab, GPopupMessage data = null) where T : GPopupUI
    {
        T popup = Instantiate(popupPrefab, popupParent);
        popup.OnOpen(data);
        popupStack.Push(popup);
        return popup;
    }

    public void CloseTopPopup()
    {
        if (popupStack.Count == 0)
            return;

        GPopupUI popup = popupStack.Pop();
        popup.OnClose();
    }

    public void CloseAllPopups()
    {
        while (popupStack.Count > 0)
        {
            popupStack.Pop().OnClose();
        }
    }
}
