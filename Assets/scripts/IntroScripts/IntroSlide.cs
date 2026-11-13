using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroSlide : MonoBehaviour
{

    public GameObject slide3;
    public GameObject slide4;

    public GameObject a1, a2, a3;
    public GameObject b1, b2;
    int currentSlide = 0;
    int itemSlide = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextSlide()
    {
        if (currentSlide == 0)
        {
            itemSlide++;
            switch (itemSlide)
            {
                case 0:
                    a1.SetActive(true);
                    a2.SetActive(false);
                    break;
                case 1:
                    a2.SetActive(true);
                    a1.SetActive(false);
                    break;
                case 2:
                    a3.SetActive(true);
                    a2.SetActive(false);
                    break;
                case 3:
                    // a1.SetActive(true);
                    // a3.SetActive(false);
                    currentSlide++;
                    slide3.SetActive(false);
                    slide4.SetActive(true);
                    itemSlide = 0;
                    break;
            }
        }
            else if (currentSlide == 1)
            {
                itemSlide++;
                switch (itemSlide)
                {
                    case 0:
                        b1.SetActive(true);
                        b2.SetActive(false);
                        break;
                    case 1:
                        b2.SetActive(true);
                        b1.SetActive(false);
                        break;
                    case 2:
                        SceneManager.LoadScene("MainGame");
                        break;
                }
            }
    }

    public void PreviousSlide()
    {
        if (currentSlide == 0)
        {
            itemSlide--;
            switch (itemSlide)
            {
                case 0:
                    a1.SetActive(true);
                    a2.SetActive(false);
                    break;
                case 1:
                    a2.SetActive(true);
                    a1.SetActive(false);
                    a3.SetActive(false);
                    break;
                case 2:
                    a3.SetActive(true);
                    a2.SetActive(false);
                    break;
                case -1:
                    SceneManager.LoadScene("Information");
                    break;
            }
        }
        else if (currentSlide == 1)
        {
            itemSlide--;
            switch (itemSlide)
            {
                case 0:
                    b1.SetActive(true);
                    b2.SetActive(false);
                    break;
                case 1:
                    b2.SetActive(true);
                    b1.SetActive(false);
                    break;
                case -1:
                    currentSlide--;
                    slide4.SetActive(false);
                    slide3.SetActive(true);
                    itemSlide = 2;
                    break;
            }
        }
    }
}
