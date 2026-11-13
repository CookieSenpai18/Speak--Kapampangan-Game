using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterTracingController :
    MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IDragHandler
{
    [Header("UI")]
    [SerializeField] private Image tracingImage;
    [SerializeField] private Image completedImage;

    [Header("Tracing")]
    [SerializeField] private Color32 grayTargetColor =
        new Color32(160, 160, 160, 255);

    [SerializeField, Range(0f, 1f)]
    public float grayTolerance = 0.12f;

    [Tooltip(
        "Brush radius as a percentage of the displayed image width."
    )]
    [SerializeField, Range(0.1f, 10f)]
    public float brushRadiusPercent = 1.0f;

    [Tooltip("Percentage of the gray path that must be traced.")]
    [SerializeField, Range(0.5f, 1f)]
    public float completionThreshold = 0.95f;

    [Header("Visual")]
    [SerializeField] private Color32 tracedColor =
        new Color32(70, 170, 255, 255);


    public TMP_Text sizeTxt;


    // ============================================================
    // RUNTIME TEXTURE DATA
    // ============================================================

    private Texture2D runtimeTexture;

    private Color32[] pixels;
    private bool[] traceablePixels;
    private bool[] tracedPixels;

    private int width;
    private int height;

    private int totalTraceablePixels;
    private int tracedPixelCount;


    // ============================================================
    // STATE
    // ============================================================

    private bool isTracing;
    private bool isComplete;

    private Sprite currentBackSprite;
    private Sprite currentFrontSprite;

    private RectTransform imageRect;

    private float currentBrushRadius;


    public bool IsComplete => isComplete;

    public event Action TraceCompleted;


    // ============================================================
    // UNITY
    // ============================================================

    private void Awake()
    {
        imageRect = tracingImage.rectTransform;

        if (completedImage != null)
        {
            completedImage.gameObject.SetActive(false);
        }
    }


    // ============================================================
    // BEGIN TRACE
    // ============================================================

    public void BeginTrace(
        Sprite backSprite,
        Sprite frontSprite)
    {
        CleanupRuntimeTexture();

        currentBackSprite = backSprite;
        currentFrontSprite = frontSprite;

        isComplete = false;
        isTracing = false;

        tracedPixelCount = 0;
        totalTraceablePixels = 0;


        if (completedImage != null)
        {
            completedImage.gameObject.SetActive(false);
        }

        tracingImage.gameObject.SetActive(true);


        if (backSprite == null)
        {
            Debug.LogError(
                "CharacterTracingController: " +
                "Back sprite is null."
            );

            return;
        }


        // ========================================================
        // GET SOURCE TEXTURE
        // ========================================================

        Texture2D sourceTexture =
            backSprite.texture;

        width = sourceTexture.width;
        height = sourceTexture.height;


        // ========================================================
        // LOAD ORIGINAL PIXELS
        // ========================================================

        pixels =
            sourceTexture.GetPixels32();


        traceablePixels =
            new bool[pixels.Length];

        tracedPixels =
            new bool[pixels.Length];


        // ========================================================
        // BUILD TRACE MASK
        // ========================================================

        BuildTraceMask();


        // ========================================================
        // CALCULATE BRUSH SIZE
        // ========================================================

        CalculateBrushRadius();


        // ========================================================
        // CREATE RUNTIME TEXTURE
        // ========================================================

        runtimeTexture =
            new Texture2D(
                width,
                height,
                TextureFormat.RGBA32,
                false
            );

        runtimeTexture.name =
            $"{backSprite.name}_RuntimeTrace";


        runtimeTexture.SetPixels32(pixels);
        runtimeTexture.Apply();


        // ========================================================
        // CREATE RUNTIME SPRITE
        // ========================================================

        Sprite runtimeSprite =
            Sprite.Create(
                runtimeTexture,
                new Rect(
                    0,
                    0,
                    width,
                    height
                ),
                new Vector2(0.5f, 0.5f),
                backSprite.pixelsPerUnit
            );


        tracingImage.sprite =
            runtimeSprite;

        tracingImage.preserveAspect =
            true;


        // ========================================================
        // SET COMPLETED IMAGE
        // ========================================================

        if (completedImage != null)
        {
            completedImage.sprite =
                frontSprite;

            completedImage.preserveAspect =
                true;
        }
    }


    // ============================================================
    // CALCULATE BRUSH RADIUS
    // ============================================================

    private void CalculateBrushRadius()
    {
        /*
         * The old system used a fixed number of texture pixels:
         *
         *     brushRadius = 10
         *
         * That means a 10-pixel radius regardless of the image
         * resolution.
         *
         * Instead, we calculate the radius based on the width
         * of the currently loaded image.
         *
         * Example:
         *
         * 1000 px wide image
         * 1% = 10 px radius
         *
         * 2000 px wide image
         * 1% = 20 px radius
         *
         * This keeps the brush visually consistent.
         */

        currentBrushRadius =
            width * (brushRadiusPercent / 100f);


        // Make sure the brush always has at least a 1-pixel radius.

        currentBrushRadius =
            Mathf.Max(
                1f,
                currentBrushRadius
            );


        Debug.Log(
            $"Image: {width}x{height} | " +
            $"Brush radius: {currentBrushRadius:F1} texture pixels"
        );
    }


    // ============================================================
    // BUILD TRACE MASK
    // ============================================================

    private void BuildTraceMask()
    {
        totalTraceablePixels = 0;


        for (int i = 0;
             i < pixels.Length;
             i++)
        {
            if (IsGrayPixel(pixels[i]))
            {
                traceablePixels[i] = true;

                totalTraceablePixels++;
            }
        }


        Debug.Log(
            $"Tracing path contains " +
            $"{totalTraceablePixels} pixels."
        );
    }


    // ============================================================
    // CHECK GRAY PIXEL
    // ============================================================

    private bool IsGrayPixel(Color32 pixel)
    {
        // Ignore transparent pixels.

        if (pixel.a < 20)
            return false;


        float r =
            pixel.r / 255f;

        float g =
            pixel.g / 255f;

        float b =
            pixel.b / 255f;


        float max =
            Mathf.Max(r, g, b);

        float min =
            Mathf.Min(r, g, b);


        // How close the RGB values are.

        float saturation =
            max - min;


        // Gray colors have low saturation.

        if (saturation > grayTolerance)
            return false;


        // Prevent very dark/very light pixels
        // from becoming traceable.

        float brightness =
            (r + g + b) / 3f;


        return brightness > 0.25f &&
               brightness < 0.85f;
    }


    // ============================================================
    // POINTER DOWN
    // ============================================================

    public void OnPointerDown(
        PointerEventData eventData)
    {
        if (isComplete)
            return;


        isTracing = true;


        TraceAtScreenPosition(
            eventData.position,
            eventData.pressEventCamera
        );
    }


    // ============================================================
    // DRAG
    // ============================================================

    public void OnDrag(
        PointerEventData eventData)
    {
        if (!isTracing ||
            isComplete)
        {
            return;
        }


        TraceAtScreenPosition(
            eventData.position,
            eventData.pressEventCamera
        );
    }


    // ============================================================
    // POINTER UP
    // ============================================================

    public void OnPointerUp(
        PointerEventData eventData)
    {
        isTracing = false;
    }


    // ============================================================
    // SCREEN → IMAGE PIXEL
    // ============================================================

    private void TraceAtScreenPosition(
        Vector2 screenPosition,
        Camera eventCamera)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                imageRect,
                screenPosition,
                eventCamera,
                out Vector2 localPoint))
        {
            return;
        }


        Rect rect =
            imageRect.rect;


        float normalizedX =
            Mathf.InverseLerp(
                rect.xMin,
                rect.xMax,
                localPoint.x
            );


        float normalizedY =
            Mathf.InverseLerp(
                rect.yMin,
                rect.yMax,
                localPoint.y
            );


        if (normalizedX < 0f ||
            normalizedX > 1f ||
            normalizedY < 0f ||
            normalizedY > 1f)
        {
            return;
        }


        int pixelX =
            Mathf.RoundToInt(
                normalizedX *
                (width - 1)
            );


        int pixelY =
            Mathf.RoundToInt(
                normalizedY *
                (height - 1)
            );


        TraceCircle(
            pixelX,
            pixelY
        );
    }


    // ============================================================
    // TRACE CIRCLE
    // ============================================================

    private void TraceCircle(
        int centerX,
        int centerY)
    {
        int radius =
            Mathf.CeilToInt(
                currentBrushRadius
            );


        float radiusSquared =
            currentBrushRadius *
            currentBrushRadius;


        int minX =
            Mathf.Max(
                0,
                centerX - radius
            );


        int maxX =
            Mathf.Min(
                width - 1,
                centerX + radius
            );


        int minY =
            Mathf.Max(
                0,
                centerY - radius
            );


        int maxY =
            Mathf.Min(
                height - 1,
                centerY + radius
            );


        bool changed = false;


        for (int y = minY;
             y <= maxY;
             y++)
        {
            for (int x = minX;
                 x <= maxX;
                 x++)
            {
                int dx =
                    x - centerX;

                int dy =
                    y - centerY;


                if (dx * dx +
                    dy * dy >
                    radiusSquared)
                {
                    continue;
                }


                int index =
                    y * width + x;


                if (!traceablePixels[index])
                    continue;


                if (tracedPixels[index])
                    continue;


                tracedPixels[index] = true;

                tracedPixelCount++;


                // Change gray pixel to trace color.

                pixels[index] =
                    tracedColor;


                changed = true;
            }
        }


        if (!changed)
            return;


        runtimeTexture.SetPixels32(
            pixels
        );

        runtimeTexture.Apply(false);


        CheckCompletion();
    }


    // ============================================================
    // CHECK COMPLETION
    // ============================================================

    private void CheckCompletion()
    {
        if (totalTraceablePixels <= 0)
            return;


        float progress =
            (float)tracedPixelCount /
            totalTraceablePixels;


        if (progress >= completionThreshold)
        {
            CompleteTrace();
        }
    }


    // ============================================================
    // COMPLETE
    // ============================================================

    private void CompleteTrace()
    {
        if (isComplete)
            return;


        isComplete = true;
        isTracing = false;


        tracingImage.gameObject.SetActive(
            false
        );


        if (completedImage != null)
        {
            completedImage.gameObject.SetActive(
                true
            );
        }


        TraceCompleted?.Invoke();
    }


    // ============================================================
    // RESET
    // ============================================================

    public void ResetTrace()
    {
        if (currentBackSprite == null)
            return;


        BeginTrace(
            currentBackSprite,
            currentFrontSprite
        );
    }


    // ============================================================
    // CLEANUP
    // ============================================================

    private void CleanupRuntimeTexture()
    {
        if (runtimeTexture != null)
        {
            Destroy(runtimeTexture);

            runtimeTexture = null;
        }
    }


    // ============================================================
    // DESTROY
    // ============================================================

    private void OnDestroy()
    {
        CleanupRuntimeTexture();
    }



    public void ChangeBrushSize(float bsize)
    {
        sizeTxt.text = (bsize * 10).ToString("F1") + "%";
        brushRadiusPercent = bsize;
        CalculateBrushRadius();
    }
}
