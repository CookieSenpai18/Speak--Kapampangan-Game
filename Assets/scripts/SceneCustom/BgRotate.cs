using UnityEngine;

public class BgRotate : MonoBehaviour
{
    public float rotateSpeed = 1;
    void Update()
    {
        this.transform.Rotate(0, 0 , rotateSpeed, Space.World);    // x = Red | y = Green | z = Blue
    }
    RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        StartCoroutine(ScaleIn());
    }

System.Collections.IEnumerator ScaleIn()
{
    Vector3 start = Vector3.one * 0.7f;  // already visible, slightly smaller
    Vector3 target = 70 * Vector3.one;  // very large scale for background

    rect.localScale = start;

    float t = 0f;
    float duration = 0.4f;

    while (t < duration)
    {
        t += Time.deltaTime;

        rect.localScale =
            Vector3.Lerp(start, target, t / duration);

        yield return null;
    }

    rect.localScale = target;
}
    
}
