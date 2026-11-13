using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public class DevOverlay : MonoBehaviour
{
    public static DevOverlay Instance;

    [Header("UI")]
    [SerializeField] private RectTransform bubble;
    [SerializeField] private GameObject devPanel;

    [Header("Bubble")]
    [SerializeField] private float edgePadding = 20f;
    [SerializeField] private float snapSpeed = 10f;
    [SerializeField] private float dragThreshold = 10f;

    [Header("Panel")]
    [SerializeField] private bool closePanelWhenClickingOutside = true;

    private RectTransform canvasRect;
    private Canvas canvas;

    private bool dragging;
    private bool movedDuringDrag;

    private Vector2 pointerStartPosition;
    private Vector2 bubbleStartPosition;

    private Coroutine snapCoroutine;

private void Awake()
{
// #if !UNITY_EDITOR && !DEVELOPMENT_BUILD
//     Destroy(gameObject);
//     return;
// #endif

    // Singleton protection
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }

    Instance = this;

    // This MUST be called on the root DevOverlay object.
    DontDestroyOnLoad(gameObject);

    canvas = GetComponentInChildren<Canvas>();

    if (canvas == null)
    {
        Debug.LogError(
            "DevOverlay: Canvas not found."
        );
        return;
    }

    canvasRect =
        canvas.GetComponent<RectTransform>();

    if (bubble == null)
    {
        Debug.LogError(
            "DevOverlay: Bubble reference is missing."
        );
    }

    if (devPanel == null)
    {
        Debug.LogError(
            "DevOverlay: Dev Panel reference is missing."
        );
    }

    if (devPanel != null)
        devPanel.SetActive(false);

    SetupBubble();
}
    private void Start()
    {
        if (bubble == null)
            return;

        // Put bubble on the right side initially.
        Vector2 position = bubble.anchoredPosition;

        position.x =
            canvasRect.rect.width / 2f -
            bubble.rect.width / 2f -
            edgePadding;

        position.y = 0f;

        bubble.anchoredPosition = position;
    }

    private void SetupBubble()
    {
        if (bubble == null)
            return;

        // Make sure this object receives UI events.
        Image image = bubble.GetComponent<Image>();

        if (image != null)
            image.raycastTarget = true;
    }

    // =========================================================
    // BUBBLE DRAGGING
    // =========================================================

    public void BubblePointerDown(PointerEventData eventData)
    {
        if (bubble == null)
            return;

        dragging = true;
        movedDuringDrag = false;

        pointerStartPosition =
            eventData.position;

        bubbleStartPosition =
            bubble.anchoredPosition;

        if (snapCoroutine != null)
        {
            StopCoroutine(snapCoroutine);
            snapCoroutine = null;
        }
    }

    public void BubbleDrag(PointerEventData eventData)
    {
        if (!dragging)
            return;

        Vector2 delta =
            eventData.position -
            pointerStartPosition;

        if (delta.magnitude > dragThreshold)
            movedDuringDrag = true;

        float scaleFactor = 1f;

        if (canvas != null)
            scaleFactor = canvas.scaleFactor;

        Vector2 newPosition =
            bubbleStartPosition +
            delta / scaleFactor;

        bubble.anchoredPosition =
            ClampBubblePosition(newPosition);
    }

    public void BubblePointerUp(PointerEventData eventData)
    {
        if (!dragging)
            return;

        dragging = false;

        // If the user barely moved the bubble,
        // treat it as a tap.
        if (!movedDuringDrag)
        {
            TogglePanel();
            return;
        }

        SnapBubbleToEdge();
    }

    // =========================================================
    // BUBBLE POSITION
    // =========================================================

    private Vector2 ClampBubblePosition(
        Vector2 position)
    {
        if (canvasRect == null || bubble == null)
            return position;

        Rect canvas = canvasRect.rect;

        float halfBubbleWidth =
            bubble.rect.width / 2f;

        float halfBubbleHeight =
            bubble.rect.height / 2f;

        float minX =
            canvas.xMin +
            halfBubbleWidth +
            edgePadding;

        float maxX =
            canvas.xMax -
            halfBubbleWidth -
            edgePadding;

        float minY =
            canvas.yMin +
            halfBubbleHeight +
            edgePadding;

        float maxY =
            canvas.yMax -
            halfBubbleHeight -
            edgePadding;

        position.x =
            Mathf.Clamp(
                position.x,
                minX,
                maxX
            );

        position.y =
            Mathf.Clamp(
                position.y,
                minY,
                maxY
            );

        return position;
    }

    private void SnapBubbleToEdge()
    {
        if (canvasRect == null || bubble == null)
            return;

        Vector2 current =
            bubble.anchoredPosition;

        Rect canvas =
            canvasRect.rect;

        float halfWidth =
            bubble.rect.width / 2f;

        float leftX =
            canvas.xMin +
            halfWidth +
            edgePadding;

        float rightX =
            canvas.xMax -
            halfWidth -
            edgePadding;

        float targetX;

        if (current.x <
            (canvas.xMin + canvas.xMax) / 2f)
        {
            targetX = leftX;
        }
        else
        {
            targetX = rightX;
        }

        Vector2 target =
            new Vector2(
                targetX,
                current.y
            );

        target =
            ClampBubblePosition(target);

        if (snapCoroutine != null)
            StopCoroutine(snapCoroutine);

        snapCoroutine =
            StartCoroutine(
                SmoothSnap(target)
            );
    }

    private IEnumerator SmoothSnap(
        Vector2 target)
    {
        while (
            Vector2.Distance(
                bubble.anchoredPosition,
                target
            ) > 0.5f)
        {
            bubble.anchoredPosition =
                Vector2.Lerp(
                    bubble.anchoredPosition,
                    target,
                    Time.deltaTime * snapSpeed
                );

            yield return null;
        }

        bubble.anchoredPosition = target;

        snapCoroutine = null;
    }

    // =========================================================
    // PANEL
    // =========================================================

    public void TogglePanel()
    {
        if (devPanel == null)
            return;

        bool newState =
            !devPanel.activeSelf;

        devPanel.SetActive(newState);
    }

    public void OpenPanel()
    {
        if (devPanel != null)
            devPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        if (devPanel != null)
            devPanel.SetActive(false);
    }

    public bool IsPanelOpen()
    {
        return devPanel != null &&
               devPanel.activeSelf;
    }

    // =========================================================
    // OPTIONAL KEYBOARD TOGGLE
    // =========================================================

    private void Update()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            TogglePanel();
        }

#endif
    }
}