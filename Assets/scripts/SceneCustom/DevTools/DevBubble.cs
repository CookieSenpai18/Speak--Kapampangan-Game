using UnityEngine;
using UnityEngine.EventSystems;

public class DevBubble : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [SerializeField]
    private DevOverlay devOverlay;

    public void OnPointerDown(
        PointerEventData eventData)
    {
        if (devOverlay != null)
            devOverlay.BubblePointerDown(eventData);
    }

    public void OnDrag(
        PointerEventData eventData)
    {
        if (devOverlay != null)
            devOverlay.BubbleDrag(eventData);
    }

    public void OnPointerUp(
        PointerEventData eventData)
    {
        if (devOverlay != null)
            devOverlay.BubblePointerUp(eventData);
    }
}