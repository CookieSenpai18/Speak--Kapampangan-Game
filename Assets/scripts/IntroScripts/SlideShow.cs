using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SlideShow : MonoBehaviour
{
    public static SlideShow Instance;
    public int currentSlide = 0, itemSlide = 0;
    public GameObject[] slides;

    [Header("Slide 1 Elements")]
    public Image titleImage1;
    public GameObject titleImage2;
    public Image sunrayImage;
    public GameObject nextButton1;
    public GameObject text1;

    [Header("Slide 3 Elements")]
    public GameObject object1, object2, object3;

    [Header("Slide 4 Elements")]
    public GameObject object4, object5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slides[currentSlide].SetActive(true);
        if (currentSlide == 0)
        {
            AnimateSlide1();
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
            Instance = this;
            // DontDestroyOnLoad(gameObject);
    }

    public void SwitchDisplayNext()
    {
        object1.SetActive(true);
        if (itemSlide == 0)
        {
            object1.SetActive(false);
            object2.SetActive(true);
            itemSlide++;
        }
        else if (itemSlide == 1)
        {
            object1.SetActive(false);
            object2.SetActive(false);
            object3.SetActive(true);
            itemSlide++;
        }
        else if (itemSlide == 2)
        {
            object1.SetActive(true);
            object3.SetActive(false);
            NextSlide();
            itemSlide = 0;
        }
    }

    public void SwitchDisplayPrevious()
    {
        if (itemSlide == 0)
        {
            PreviousSlide();
        }
        else if (itemSlide == 1)
        {
            object1.SetActive(true);
            object2.SetActive(false);
            object3.SetActive(false);
            itemSlide--;
        }
        else if (itemSlide == 2)
        {
            object1.SetActive(false);
            object2.SetActive(true);
            object3.SetActive(false);
            itemSlide--;
        }
    }

    public void WordNext()
    {

        object4.SetActive(true);
            if (itemSlide == 0)
            {
                object4.SetActive(false);
                object5.SetActive(true);
                itemSlide++;
            }
            else if (itemSlide == 1)
            {
                object5.SetActive(false);
                object4.SetActive(true);
                NextSlide();
                itemSlide = 0;
            }
    }

    public void WordPrevious()
    {
        if (itemSlide == 0)
        {
            PreviousSlide();
        }
        else if (itemSlide == 1)
        {
            object4.SetActive(true);
            object5.SetActive(false);
            itemSlide--;
        }
    }

    public void NextSlide()
    {
        if(currentSlide != 0)
        {
        slides[currentSlide].SetActive(false);
        currentSlide = (currentSlide + 1) % slides.Length;
        slides[currentSlide].SetActive(true);            
        }
        else
        {
        slides[currentSlide].SetActive(false);
        currentSlide = (currentSlide + 1) % slides.Length; 
        slides[currentSlide].SetActive(true);             
        }

    }

    public void PreviousSlide()
    {
        if(currentSlide > 1)
        { 
        slides[currentSlide].SetActive(false);
        currentSlide = (currentSlide - 1 + slides.Length) % slides.Length;
        slides[currentSlide].SetActive(true);
        }
    }

    public void SkipIntro()
    {
        // Load the main menu scene
        slides[currentSlide].SetActive(false);
        itemSlide = 0;
        currentSlide = 4;
        slides[currentSlide].SetActive(true);
    }

    void AnimateSlide1()
    {
        // Example animation for slide 1
        StartCoroutine(MoveTitle());
    }

    System.Collections.IEnumerator MoveTitle()
    {
        sunrayImage.gameObject.SetActive(false);
        nextButton1.SetActive(false);
        text1.SetActive(false);
        //yield return new WaitForSeconds(2f);
        StartCoroutine(Pop(titleImage1.rectTransform, 12f));
        yield return new WaitForSeconds(0.05f);
        titleImage2.SetActive(true);
        yield return new WaitForSeconds(1f);
        sunrayImage.gameObject.SetActive(true);
        yield return null;
        yield return new WaitForSeconds(2f);
        RectTransform rect = titleImage1.rectTransform;

        Vector2 start = rect.anchoredPosition;
        Vector2 target = new Vector2(0, 600);

        float t = 0f;
        float duration = 1f;

        while (t < duration)
        {
            t += Time.deltaTime;

            rect.anchoredPosition =
                Vector2.Lerp(start, target, t / duration);

            yield return null;
        }

        rect.anchoredPosition = target;

        yield return new WaitForSeconds(0.5f);
        text1.SetActive(true);
        yield return new WaitForSeconds(3f);
        nextButton1.SetActive(true);
    }

    System.Collections.IEnumerator Pop(RectTransform rect, float scale)
{
    Vector3 normal = rect.localScale;
    Vector3 pop = normal * scale;

    float t = 0f;

    while (t < 0.5f)
    {
        t += Time.deltaTime;
        rect.localScale = Vector3.Lerp(normal, pop, t / 0.5f);
        yield return null;
    }

    // t = 0f;

    // while (t < 0.5f)
    // {
    //     t += Time.deltaTime;
    //     rect.localScale = Vector3.Lerp(pop, normal, t / 0.5f);
    //     yield return null;
    // }

    // rect.localScale = normal;
}

    public void LoadGameplayTutorial(string dest)
    {
            LoadingScreen.destinationScene = dest;
            LoadingScreen.isMultiplayer = false;

            SceneManager.LoadScene("LoadingScreen");
    }

    public void FinishIntro()
    {
        PlayerPrefs.SetInt("IsFirstTime", 1);
        PlayerPrefs.Save();
        Destroy(gameObject);
    }

}
