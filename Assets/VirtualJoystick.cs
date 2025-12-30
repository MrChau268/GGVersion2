using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform handle;   
    [Tooltip("Max distance the handle can move away from center")]
    public float moveRange = 10f;  // smaller value = moves less
    private Vector2 startPos;
    public Vector2 inputVector;

    void Start()
    {
        startPos = handle.anchoredPosition; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            handle.parent as RectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            // calculate direction
            Vector2 delta = pos - startPos;

            // normalize to -1..1 range
            inputVector = delta / moveRange;
            inputVector = (inputVector.magnitude > 1f) ? inputVector.normalized : inputVector;

            // move handle slightly (limited by moveRange)
            handle.anchoredPosition = startPos + (inputVector * moveRange);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = startPos; 
    }

    public float Horizontal() => inputVector.x;
    public float Vertical() => inputVector.y;
}
