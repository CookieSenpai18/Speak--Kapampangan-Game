using UnityEngine;
using UnityEngine.EventSystems;

public class DevPanelDrag : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler
{
    [SerializeField]
    private RectTransform panel;

    private Canvas canvas;

    private void Awake()
    {
        canvas =
            GetComponentInParent<Canvas>();

        if (panel == null)
            panel =
                transform.parent
                .GetComponent<RectTransform>();
    }

    public void OnBeginDrag(
        PointerEventData eventData)
    {
    }

    public void OnDrag(
        PointerEventData eventData)
    {
        if (panel == null)
            return;

        float scaleFactor = 1f;

        if (canvas != null)
            scaleFactor =
                canvas.scaleFactor;

        panel.anchoredPosition +=
            eventData.delta /
            scaleFactor;
    }
}