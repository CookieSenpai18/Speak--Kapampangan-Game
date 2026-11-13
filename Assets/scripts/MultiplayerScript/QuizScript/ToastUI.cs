using System.Collections;
using TMPro;
using UnityEngine;

public class ToastUI : MonoBehaviour
{
    [Header("References")]
    public TMP_Text messageText;
    public CanvasGroup canvasGroup;

    [Header("Timing")]
    public float fadeInDuration = 0.25f;
    public float displayDuration = 2.5f;
    public float fadeOutDuration = 0.25f;

    public void Show(string message)
    {
        if (messageText != null)
            messageText.text = message;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        // -------------------------
        // FADE IN
        // -------------------------

        yield return Fade(
            0f,
            1f,
            fadeInDuration
        );

        // -------------------------
        // DISPLAY
        // -------------------------

        yield return new WaitForSeconds(
            displayDuration
        );

        // -------------------------
        // FADE OUT
        // -------------------------

        yield return Fade(
            1f,
            0f,
            fadeOutDuration
        );

        // -------------------------
        // DESTROY
        // -------------------------

        Destroy(gameObject);
    }

    private IEnumerator Fade(
        float from,
        float to,
        float duration)
    {
        if (canvasGroup == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / duration
                );

            canvasGroup.alpha =
                Mathf.Lerp(from, to, t);

            yield return null;
        }

        canvasGroup.alpha = to;
    }
}