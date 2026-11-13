using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

public class AvatarCropper : MonoBehaviour,
    IPointerDownHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IScrollHandler
{
    [Header("References")]
    public GameObject cropPanel;
    public RectTransform cropArea;
    public Image cropImage;

    [Header("Zoom")]
    public float minZoom = 1f;
    public float maxZoom = 3f;
    public float zoomSpeed = 0.01f;

    [Header("Settings")]
    public bool preventEmptySpace = true;

    private Texture2D sourceTexture;
    private Action<Texture2D> onComplete;

    private RectTransform imageRect;

    private float currentZoom = 1f;
    private bool dragging;

    private float lastPinchDistance;

    private void Awake()
    {
        imageRect = cropImage.rectTransform;

        // CropArea receives pointer events.
        Image areaImage = GetComponent<Image>();

        if (areaImage != null)
            areaImage.raycastTarget = true;

        // CropImage must NOT intercept pointer events.
        if (cropImage != null)
            cropImage.raycastTarget = false;
    }

    // =========================================================
    // START CROP
    // =========================================================

    public void StartCrop(
        Texture2D texture,
        Action<Texture2D> callback)
    {
        if (texture == null)
        {
            Debug.LogError(
                "AvatarCropper: StartCrop received NULL texture."
            );
            return;
        }

        if (cropPanel == null)
        {
            Debug.LogError(
                "AvatarCropper: Crop Panel is not assigned."
            );
            return;
        }

        if (cropImage == null)
        {
            Debug.LogError(
                "AvatarCropper: Crop Image is not assigned."
            );
            return;
        }

        if (cropArea == null)
        {
            Debug.LogError(
                "AvatarCropper: Crop Area is not assigned."
            );
            return;
        }

        sourceTexture = texture;
        onComplete = callback;

        imageRect = cropImage.rectTransform;

        // Open panel BEFORE calculating dimensions.
        cropPanel.SetActive(true);

        Canvas.ForceUpdateCanvases();

        currentZoom = 1f;
        lastPinchDistance = 0f;

        FitImageToCropArea();
    }

    // =========================================================
    // POINTER
    // =========================================================

    public void OnPointerDown(
        PointerEventData eventData)
    {
        //Debug.Log(
        //     "AvatarCropper: POINTER DOWN"
        // );
    }

    // =========================================================
    // BEGIN DRAG
    // =========================================================

    public void OnBeginDrag(
        PointerEventData eventData)
    {
        // Debug.Log(
        //     "AvatarCropper: BEGIN DRAG"
        // );

        dragging = true;
    }

    // =========================================================
    // DRAG
    // =========================================================

    public void OnDrag(
        PointerEventData eventData)
    {
        if (!dragging)
            return;

        Vector2 delta =
            eventData.delta;

        float scaleFactor = 1f;

        Canvas canvas =
            GetComponentInParent<Canvas>();

        if (canvas != null)
            scaleFactor = canvas.scaleFactor;

        imageRect.anchoredPosition +=
            delta / scaleFactor;

        ClampImagePosition();

        // Debug.Log(
        //     "AvatarCropper: DRAG " +
        //     delta
        // );
    }

    // =========================================================
    // END DRAG
    // =========================================================

    public void OnEndDrag(
        PointerEventData eventData)
    {
        // Debug.Log(
        //     "AvatarCropper: END DRAG"
        // );

        dragging = false;
    }

    // =========================================================
    // MOUSE WHEEL ZOOM
    // =========================================================

    public void OnScroll(
        PointerEventData eventData)
    {
        if (eventData.scrollDelta.y == 0)
            return;

        SetZoom(
            currentZoom +
            eventData.scrollDelta.y *
            zoomSpeed
        );
    }

    // =========================================================
    // PINCH ZOOM
    // =========================================================

    private void Update()
    {
        if (Touchscreen.current == null)
            return;

        var touches =
            Touchscreen.current.touches;

        if (touches.Count < 2)
        {
            lastPinchDistance = 0;
            return;
        }

        var touch0 = touches[0];
        var touch1 = touches[1];

        if (!touch0.press.isPressed ||
            !touch1.press.isPressed)
        {
            lastPinchDistance = 0;
            return;
        }

        Vector2 pos0 =
            touch0.position.ReadValue();

        Vector2 pos1 =
            touch1.position.ReadValue();

        float distance =
            Vector2.Distance(
                pos0,
                pos1
            );

        if (lastPinchDistance > 0)
        {
            float difference =
                distance -
                lastPinchDistance;

            SetZoom(
                currentZoom +
                difference * 0.005f
            );
        }

        lastPinchDistance =
            distance;
    }

    // =========================================================
    // FIT IMAGE
    // =========================================================

    private void FitImageToCropArea()
    {
        if (sourceTexture == null)
            return;

        if (cropArea == null ||
            cropImage == null)
            return;

        float cropWidth =
            cropArea.rect.width;

        float cropHeight =
            cropArea.rect.height;

        float textureWidth =
            sourceTexture.width;

        float textureHeight =
            sourceTexture.height;

        float scaleX =
            cropWidth / textureWidth;

        float scaleY =
            cropHeight / textureHeight;

        // Cover the entire crop area.
        float scale =
            Mathf.Max(
                scaleX,
                scaleY
            );

        imageRect.sizeDelta =
            new Vector2(
                textureWidth * scale,
                textureHeight * scale
            );

        imageRect.anchoredPosition =
            Vector2.zero;

        imageRect.localScale =
            Vector3.one;

        currentZoom = 1f;

        Sprite sprite =
            Sprite.Create(
                sourceTexture,
                new Rect(
                    0,
                    0,
                    sourceTexture.width,
                    sourceTexture.height
                ),
                new Vector2(
                    0.5f,
                    0.5f
                )
            );

        cropImage.sprite =
            sprite;

        ClampImagePosition();
    }

    // =========================================================
    // ZOOM
    // =========================================================

    private void SetZoom(
        float zoom)
    {
        float oldZoom =
            currentZoom;

        currentZoom =
            Mathf.Clamp(
                zoom,
                minZoom,
                maxZoom
            );

        imageRect.localScale =
            Vector3.one *
            currentZoom;

        ClampImagePosition();

        // Debug.Log(
        //     "AvatarCropper: ZOOM " +
        //     oldZoom +
        //     " -> " +
        //     currentZoom
        // );
    }

    // =========================================================
    // CLAMP
    // =========================================================

    private void ClampImagePosition()
    {
        if (cropArea == null ||
            imageRect == null)
            return;

        Rect cropRect =
            cropArea.rect;

        Vector2 imageSize =
            imageRect.rect.size *
            imageRect.localScale.x;

        float maxX =
            Mathf.Max(
                0,
                (imageSize.x -
                 cropRect.width) / 2f
            );

        float maxY =
            Mathf.Max(
                0,
                (imageSize.y -
                 cropRect.height) / 2f
            );

        Vector2 pos =
            imageRect.anchoredPosition;

        pos.x =
            Mathf.Clamp(
                pos.x,
                -maxX,
                maxX
            );

        pos.y =
            Mathf.Clamp(
                pos.y,
                -maxY,
                maxY
            );

        imageRect.anchoredPosition =
            pos;
    }

    // =========================================================
    // CONFIRM
    // =========================================================

    public void ConfirmCrop()
    {
        // SceneManagerScript.isAvatarActive = false;
        if (sourceTexture == null)
        {
            // Debug.LogError(
            //     "AvatarCropper: Source Texture is NULL!"
            // );
            return;
        }

        Texture2D result =
            CreateVisibleCrop();

        if (result == null)
        {
            Debug.LogError(
                "AvatarCropper: Crop failed."
            );
            return;
        }

        // Save/return the cropped texture.
        Action<Texture2D> callback =
            onComplete;

        // Close crop panel.
        if (cropPanel != null)
            cropPanel.SetActive(false);

        dragging = false;

        onComplete = null;

        callback?.Invoke(result);

        // Debug.Log(
        //     "AvatarCropper: Crop confirmed. " +
        //     $"Result = {result.width}x{result.height}"
        // );

        Cleanup();
    }

    // =========================================================
    // CREATE ACTUAL VISIBLE CROP
    // =========================================================

    private Texture2D CreateVisibleCrop()
    {
        if (sourceTexture == null)
            return null;

        Camera cam =
            GetUICamera();

        // -----------------------------------------------------
        // Get crop area's four screen corners
        // -----------------------------------------------------

        Vector3[] cropCorners =
            new Vector3[4];

        cropArea.GetWorldCorners(
            cropCorners
        );

        Vector2 bottomLeftScreen =
            RectTransformUtility.WorldToScreenPoint(
                cam,
                cropCorners[0]
            );

        Vector2 topRightScreen =
            RectTransformUtility.WorldToScreenPoint(
                cam,
                cropCorners[2]
            );

        // -----------------------------------------------------
        // Convert screen positions to image-local positions
        // -----------------------------------------------------

        Vector2 bottomLeftLocal;
        Vector2 topRightLocal;

        RectTransformUtility
            .ScreenPointToLocalPointInRectangle(
                imageRect,
                bottomLeftScreen,
                cam,
                out bottomLeftLocal
            );

        RectTransformUtility
            .ScreenPointToLocalPointInRectangle(
                imageRect,
                topRightScreen,
                cam,
                out topRightLocal
            );

        Rect imageLocalRect =
            imageRect.rect;

        // -----------------------------------------------------
        // Convert local UI coordinates to normalized UV
        // -----------------------------------------------------

        float normalizedXMin =
            Mathf.InverseLerp(
                imageLocalRect.xMin,
                imageLocalRect.xMax,
                bottomLeftLocal.x
            );

        float normalizedXMax =
            Mathf.InverseLerp(
                imageLocalRect.xMin,
                imageLocalRect.xMax,
                topRightLocal.x
            );

        float normalizedYMin =
            Mathf.InverseLerp(
                imageLocalRect.yMin,
                imageLocalRect.yMax,
                bottomLeftLocal.y
            );

        float normalizedYMax =
            Mathf.InverseLerp(
                imageLocalRect.yMin,
                imageLocalRect.yMax,
                topRightLocal.y
            );

        // -----------------------------------------------------
        // Clamp
        // -----------------------------------------------------

        normalizedXMin =
            Mathf.Clamp01(
                normalizedXMin
            );

        normalizedXMax =
            Mathf.Clamp01(
                normalizedXMax
            );

        normalizedYMin =
            Mathf.Clamp01(
                normalizedYMin
            );

        normalizedYMax =
            Mathf.Clamp01(
                normalizedYMax
            );

        // -----------------------------------------------------
        // Convert to texture pixels
        // -----------------------------------------------------

        int pixelX =
            Mathf.RoundToInt(
                normalizedXMin *
                sourceTexture.width
            );

        int pixelY =
            Mathf.RoundToInt(
                normalizedYMin *
                sourceTexture.height
            );

        int pixelWidth =
            Mathf.RoundToInt(
                (normalizedXMax -
                 normalizedXMin) *
                sourceTexture.width
            );

        int pixelHeight =
            Mathf.RoundToInt(
                (normalizedYMax -
                 normalizedYMin) *
                sourceTexture.height
            );

        // -----------------------------------------------------
        // Safety checks
        // -----------------------------------------------------

        pixelX =
            Mathf.Clamp(
                pixelX,
                0,
                sourceTexture.width - 1
            );

        pixelY =
            Mathf.Clamp(
                pixelY,
                0,
                sourceTexture.height - 1
            );

        pixelWidth =
            Mathf.Clamp(
                pixelWidth,
                1,
                sourceTexture.width - pixelX
            );

        pixelHeight =
            Mathf.Clamp(
                pixelHeight,
                1,
                sourceTexture.height - pixelY
            );

        // -----------------------------------------------------
        // Make result square
        // -----------------------------------------------------

        int cropSize =
            Mathf.Min(
                pixelWidth,
                pixelHeight
            );

        pixelX +=
            (pixelWidth -
             cropSize) / 2;

        pixelY +=
            (pixelHeight -
             cropSize) / 2;

        pixelX =
            Mathf.Clamp(
                pixelX,
                0,
                sourceTexture.width -
                cropSize
            );

        pixelY =
            Mathf.Clamp(
                pixelY,
                0,
                sourceTexture.height -
                cropSize
            );

        // -----------------------------------------------------
        // Get pixels
        // -----------------------------------------------------

        try
        {
            Color[] pixels =
                sourceTexture.GetPixels(
                    pixelX,
                    pixelY,
                    cropSize,
                    cropSize
                );

            Texture2D result =
                new Texture2D(
                    cropSize,
                    cropSize,
                    TextureFormat.RGBA32,
                    false
                );

            result.SetPixels(
                pixels
            );

            result.Apply();

            Debug.Log(
                "AvatarCropper: Cropped pixels " +
                $"X={pixelX}, Y={pixelY}, " +
                $"Size={cropSize}"
            );

            return result;
        }
        catch (Exception e)
        {
            Debug.LogError(
                "AvatarCropper: GetPixels failed: " +
                e.Message
            );

            return null;
        }
    }

    // =========================================================
    // GET UI CAMERA
    // =========================================================

    private Camera GetUICamera()
    {
        Canvas canvas =
            cropImage.canvas;

        if (canvas == null)
            return null;

        if (canvas.renderMode ==
            RenderMode.ScreenSpaceOverlay)
        {
            return null;
        }

        return canvas.worldCamera;
    }

    // =========================================================
    // CANCEL
    // =========================================================

    public void CancelCrop()
    {
        if (cropPanel != null)
            cropPanel.SetActive(false);

        dragging = false;

        onComplete = null;

        Cleanup();

        Debug.Log(
            "AvatarCropper: Crop cancelled."
        );
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void Cleanup()
    {
        dragging = false;

        if (cropImage != null)
        {
            cropImage.sprite = null;
        }

        if (sourceTexture != null)
        {
            sourceTexture = null;
        }

        onComplete = null;

        lastPinchDistance = 0f;
    }
}