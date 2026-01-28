using System;

public class GPopupData
{
    public string message;
    public float autoCloseTime;
    public Action onCancel;
    public Action onConfirm;

    public GPopupData(string message, float autoCloseTime = 0, Action onCancel = null, Action onConfirm = null)
    {
        this.message = message;
        this.autoCloseTime = autoCloseTime;
        this.onCancel = onCancel;
        this.onConfirm = onConfirm;
    }
}
