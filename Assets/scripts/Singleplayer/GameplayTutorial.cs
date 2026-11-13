using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;



public class GameplayTutorial : MonoBehaviour
{
    public GameObject slide1;
    public GameObject slide2;
    public GameObject slide3;
    public GameObject slide4;
    public GameObject slide5;

    public GameObject tapCS1;
    public GameObject tapCS2;
    public GameObject tapCS3;
    public GameObject flashCardTap;

    public TMP_Text topText;
    public TMP_Text bottomText;
    public TMP_Text toppestText;

    public GameObject scorePanel;
    public GameObject streakpanel;
    public GameObject hintPanel;
    public GameObject clockPanel;


    public GameObject nextButton;
    public GameObject layer1;


    public int currentSlide = 0;
    public int itemSlide = 0;

    public GameObject layer2;
    public GameObject layer3;
    public GameObject layer4;
    public GameObject layer5;
    public GameObject layer6;
    public GameObject layer7;


    public Button s1c1;
    public Button s1c2;
    public Button s1c3;

    public Button s2c2;

    public Button s3c1;
    public Button s4c3;

    public Button hintButton;
    public Animator hintAnim;
    public GameObject hintFrontSide;
    public GameObject hintBackSide;



    public TMP_Text scoreTxt;
    public TMP_Text streakTxt;
    public TMP_Text hintTxt;




    [Header("Translations")]
    [SerializeField] private LocalizedString text1; //Greetings
    [SerializeField] private LocalizedString text2; //Newplayer
    [SerializeField] private LocalizedString text3; //Offer
    [SerializeField] private LocalizedString text4; //QuizInfo
    [SerializeField] private LocalizedString text5; //ChoicesInfo
    [SerializeField] private LocalizedString text6; //Try C1
    [SerializeField] private LocalizedString text7; // C1 Info
    [SerializeField] private LocalizedString text8; //Try C2
    [SerializeField] private LocalizedString text9; //C2 info
    [SerializeField] private LocalizedString text10;    //Try C3
    [SerializeField] private LocalizedString text11;    //C3 info
    [SerializeField] private LocalizedString text12;    //Score info
    [SerializeField] private LocalizedString text13;    //Next question info
    [SerializeField] private LocalizedString text14;    //Hint info
    [SerializeField] private LocalizedString text15;    //Hint display info
    [SerializeField] private LocalizedString text16;    //Try Correct from hint
    [SerializeField] private LocalizedString text17;    //Streak intro
    [SerializeField] private LocalizedString text18;    //Keep going
    [SerializeField] private LocalizedString text19;    //got hint from streak
    [SerializeField] private LocalizedString text20;    //elaborate streak
    [SerializeField] private LocalizedString text21;    //try self
    [SerializeField] private LocalizedString text22;    //timer intro
    [SerializeField] private LocalizedString text23;    //minor details
    [SerializeField] private LocalizedString text24;    //transfer to main

    void Start()
    {
        CleanUpText();
        slide1.SetActive(false);
        slide2.SetActive(false);
        slide3.SetActive(false);
        slide4.SetActive(false);
        slide5.SetActive(false);


        scorePanel.SetActive(false);
        streakpanel.SetActive(false);
        clockPanel.SetActive(false);
        hintPanel.SetActive(false);


        tapCS1.SetActive(false);
        tapCS2.SetActive(false);
        tapCS3.SetActive(false);
        flashCardTap.SetActive(false);

        hintButton.gameObject.SetActive(false);

        UpdateDisplaySlides();

    }

    void UpdateDisplaySlides()
    {
        if(currentSlide == 0)
        {
            switch (itemSlide)
            {
                case 0:
                    {
                        string mess = text1.GetLocalizedString() + PlayerPrefs.GetString("UserName", "Player") + "!";
                        layer1.SetActive(true);
                        DisplayText(mess, Color.white,  1);
                        itemSlide++;
                    }
                    break;
                case 2:
                    {
                        DisplayText(text2.GetLocalizedString(), Color.white, 1);
                    }
                    break;
                case 3:
                    {
                        DisplayText(text3.GetLocalizedString(), Color.white, 1);
                    }
                    break;
            }
        }

            if(currentSlide == 1)
            {
                switch (itemSlide)
                {
                    case 0:
                        {   
                            layer1.SetActive(false);
                            layer2.SetActive(true);
                            slide1.SetActive(true);
                            DisplayText(text4.GetLocalizedString(), Color.white, 0);
                        }
                        break;
                    case 1:
                    {
                        layer2.SetActive(false);
                        layer3.SetActive(true);
                        DisplayText(text5.GetLocalizedString(), Color.white, 1);
                    }
                    break;
                    case 2:
                    {
                        layer3.SetActive(false);
                        DisplayText(text6.GetLocalizedString(), Color.white, 2);
                        s1c1.interactable = true;
                        tapCS1.SetActive(true);
                        nextButton.SetActive(false);
                    }
                    break;
                    case 3:
                    {
                        s1c1.image.color = Color.red;
                        s1c1.GetComponentInChildren<TMP_Text>().color = Color.white;
                        PlayIncorrectSfx();
                        s1c1.interactable = false;
                        tapCS1.SetActive(false);
                        DisplayText(text7.GetLocalizedString(), Color.white, 2);
                        nextButton.SetActive(true);
                    }
                    break;
                    case 4:
                    {
                        s1c2.interactable = true;
                        tapCS2.SetActive(true);
                        DisplayText(text8.GetLocalizedString(), Color.white, 2);
                        nextButton.SetActive(false);
                    }
                    break;
                    case 5:
                    {
                        s1c2.interactable = false;
                        s1c2.image.color = Color.red;
                        s1c2.GetComponentInChildren<TMP_Text>().color = Color.white;
                        PlayIncorrectSfx();
                        DisplayText(text9.GetLocalizedString(), Color.white, 2);
                        tapCS2.SetActive(false);
                        nextButton.SetActive(true);
                    }
                    break;
                    case 6:
                    {
                        DisplayText(text10.GetLocalizedString(), Color.white, 2);
                        tapCS3.SetActive(true);
                        s1c3.interactable = true;
                        nextButton.SetActive(false);
                    }
                    break;
                    case 7:
                    {
                        s1c3.GetComponentInChildren<TMP_Text>().color = Color.white;
                        s1c3.interactable = false;
                        s1c3.image.color = Color.green;
                        tapCS3.SetActive(false);
                        PlayCorrectSfx();
                        DisplayText(text11.GetLocalizedString(), Color.white, 2);
                        nextButton.SetActive(true);
                    }
                    break;
                    case 8:
                    {
                        DisplayText(text12.GetLocalizedString(), Color.white, 1);
                        scoreTxt.text = "10";
                        scorePanel.SetActive(true);
                        layer4.SetActive(true);
                    }
                    break;
                }
            }
        if(currentSlide == 2)
        {
            switch (itemSlide)
            {
                case 0:
                    {
                        hintButton.gameObject.SetActive(true);
                        DisplayText(text13.GetLocalizedString(), Color.white, 2);
                        layer4.SetActive(false);
                        slide1.SetActive(false);
                        slide2.SetActive(true);
                    }
                    break;
                case 1:
                    {
                        DisplayText(text14.GetLocalizedString(), Color.white, 0);
                        layer5.SetActive(true);
                        hintPanel.SetActive(true);
                        flashCardTap.SetActive(true);
                        hintButton.interactable = true;
                        hintTxt.text = "1";
                        nextButton.SetActive(false);
                    }
                    break;
                case 2:
                    {
                        DisplayText(text15.GetLocalizedString(), Color.white, 0);
                        StartCoroutine(FlipHint());
                        hintButton.interactable = false;
                        hintTxt.text = "0";
                        layer5.SetActive(false);
                        nextButton.SetActive(true);
                        flashCardTap.SetActive(false);
                        
                    }
                    break;
                case 3:
                    {
                        DisplayText(text16.GetLocalizedString(), Color.white, 2);
                        s2c2.interactable = true;
                        tapCS2.SetActive(true);
                        nextButton.SetActive(false);
                    }
                    break;
                case 4:
                    {
                        DisplayText(text17.GetLocalizedString(), Color.white, 1);
                        tapCS2.SetActive(false);
                        s2c2.interactable = false;
                        s2c2.image.color = Color.green;
                        layer6.SetActive(true);
                        PlayCorrectSfx();
                        s2c2.GetComponentInChildren<TMP_Text>().color = Color.white;
                        scoreTxt.text = "20";
                        nextButton.SetActive(true);
                        streakpanel.SetActive(true);
                        streakTxt.text = "1";
                    }
                    break;
            }
        }
        if(currentSlide == 3)
        {
            switch (itemSlide)
            {
                case 0:
                    {
                        DisplayText(text18.GetLocalizedString(), Color.white, 2);
                        layer6.SetActive(false);
                        slide2.SetActive(false);
                        slide3.SetActive(true);
                        hintButton.gameObject.SetActive(false);
                        s3c1.interactable = true;
                        tapCS1.SetActive(true);
                        nextButton.SetActive(false);
                    }
                    break;
                case 1:
                    {   
                        s3c1.image.color = Color.green;
                        s3c1.GetComponentInChildren<TMP_Text>().color = Color.white;
                        PlayCorrectSfx(); 
                        s4c3.interactable = true;
                        tapCS1.SetActive(false);
                        scoreTxt.text = "30";
                        streakTxt.text = "2";
                        StartCoroutine(DelaySwitchSlide(slide3, slide4));
                    }
                    break;
                case 2:
                    {
                        scoreTxt.text = "40";
                        tapCS3.SetActive(false);
                        streakTxt.text = "3";
                        hintTxt.text = "1";
                        s4c3.image.color = Color.green;
                        s4c3.GetComponentInChildren<TMP_Text>().color = Color.white;
                        PlayCorrectSfx();
                        DisplayText(text19.GetLocalizedString(), Color.white, 1);
                        nextButton.SetActive(true);
                        layer5.SetActive(true);
                    }
                    break;
                case 3:
                    {
                        DisplayText(text20.GetLocalizedString(), Color.white, 1);
                    }
                    break;
                case 4:
                    {
                        DisplayText(text21.GetLocalizedString(), Color.white, 1);
                        layer5.SetActive(false);
                        layer7.SetActive(true);
                    }
                    break;
                case 5:
                    {
                        DisplayText(text22.GetLocalizedString(), Color.white, 1);
                        clockPanel.SetActive(true);
                        PlayClock();
                    }
                    break;
                case 6:
                    {
                        DisplayText(text23.GetLocalizedString(), Color.white, 1);
                    }
                    break;
                case 7:
                    {
                        DisplayText(text24.GetLocalizedString(), Color.white, 1);
                        layer7.SetActive(false);
                        layer1.SetActive(true);
                    }
                    break;
                case 8:
                    {
                        if (AudioManager.Instance != null)
                        {
                            AudioManager.Instance.StopTimesUpSfx();
                        }
                        PlayerPrefs.SetInt("TimeDuration", 1);
                        PlayerPrefs.SetInt("DifficultyLevel", 1);
                        PlayerPrefs.SetInt("DestinationScene", 1);
                        PlayerPrefs.SetInt("IsFirstTime", 1);
                        PlayerPrefs.Save();
                        LoadingScreen.isFromTutorial = true;
                        LoadingScreen.destinationScene = "Single_Indu";
                        LoadingScreen.isMultiplayer = false;
                        SceneManager.LoadScene("LoadingScreen");
                    }
                    break;
            }

        }

    }

    public void ProcessAnswer(bool isCorrect)
    {
        NextButton();
    }

    public void NextButton()
    {
        if(currentSlide == 0)
        {
            if(itemSlide < 3)
                itemSlide++;
            else
            {
                currentSlide++;
                itemSlide = 0;
            }
        }
        else if(currentSlide == 1)
        {
            if(itemSlide <= 7)
                itemSlide++;
            else
            {
                itemSlide = 0;
                currentSlide++;
            }
        }
        else if(currentSlide == 2)
        {
            if(itemSlide < 4)
                itemSlide++;
            else
            {
                itemSlide = 0;
                currentSlide++;
            }
        }
        else if(currentSlide == 3)
        {
            if(itemSlide <= 8)
                itemSlide++;
            else
            {
                itemSlide = 0;
                currentSlide++;
            }
        }


        UpdateDisplaySlides();
    }



    void CleanUpText()
    {
        topText.gameObject.SetActive(false);
        bottomText.gameObject.SetActive(false);
        toppestText.gameObject.SetActive(false);
        topText.text = "";
        bottomText.text = "";
        toppestText.text = "";
    }

    void DisplayText(string textDisplay, Color color,  int pos)
    {
        CleanUpText();

        if(pos == 0)
        {
            bottomText.gameObject.SetActive(true);
            bottomText.text = textDisplay;
            bottomText.color = color;
        }
        else if(pos == 1)
        {
            topText.gameObject.SetActive(true);
            topText.text = textDisplay;
            topText.color = color;
        }
        else
        {
            toppestText.gameObject.SetActive(true);
            toppestText.text = textDisplay;
            toppestText.color = color;
        }
    }

    System.Collections.IEnumerator FlipHint()
    {
        hintFrontSide.SetActive(true);
        hintBackSide.SetActive(false);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.CardFlipSfx1();
        }
        if (hintAnim != null)
        {
            hintAnim.ResetTrigger("Flip90");
            hintAnim.ResetTrigger("Flip0");

            hintAnim.SetTrigger("Flip90");
        }

        yield return new WaitForSeconds(0.7f);
        // switch back side
        hintFrontSide.SetActive(false);
        hintBackSide.SetActive(true);

        if (hintAnim != null)
        {
            hintAnim.ResetTrigger("Flip90");
            hintAnim.SetTrigger("Flip0");
        }
    }

    System.Collections.IEnumerator DelaySwitchSlide(GameObject oldSlide, GameObject newSlide)
    {
        oldSlide.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        oldSlide.SetActive(false);
        newSlide.SetActive(true);
        tapCS3.SetActive(true);
    }
    void PlayIncorrectSfx()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.WrongSfx();
        }
    }

    void PlayCorrectSfx()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.CorrectSfx();
        }
    }

    void PlayClock()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TickingSfx();
        }
    }

}

