using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.Localization;
using System.Collections;

public class DifficultySelector : MonoBehaviour
{
    public static DifficultySelector Instance;
    [Header("Difficulty Slider")]
    public Slider difficultySlider;
    public TMP_Text textHolderDiff;
            string dest;
    public enum Difficulties
    {
        Easy,
        Medium,
        Hard
    }
    public Difficulties currentDifficulty;

    [Header("Time Selectors")]
    public Button previousButton, nextButton;
    int index = 0;
    public TMP_Text textHolder;

    [Header("Data To Transfer")]
    public int timeDuration, difficultyLevel, destinationScene;

    [Header("Title Images")]
    public Sprite readingTitle, synonymTitle, antonymTitle;
    public TMP_Text gameModeTitle;
    [Header("Image")] public Image titleImage;

    [Header("Language")]
    [Header("Gamemode")]
    [SerializeField] private LocalizedString readingText;
    [SerializeField] private LocalizedString synonymsText;
    [SerializeField] private LocalizedString antonymsText;
    [Header("Difficulty")]
    [SerializeField] private LocalizedString easyText;
    [SerializeField] private LocalizedString mediumText;
    [SerializeField] private LocalizedString hardText;
    [Header("Clock")]
    [SerializeField] private LocalizedString slowText;
    [SerializeField] private LocalizedString fastText;


    

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        timeDuration = PlayerPrefs.GetInt("TimeDuration", 0);
        difficultyLevel = PlayerPrefs.GetInt("DifficultyLevel", 0);
        destinationScene = PlayerPrefs.GetInt("DestinationScene", 1);
        index = timeDuration;
        difficultySlider.value = difficultyLevel;
        difficultySlider.onValueChanged.AddListener(delegate {
            UpdateDifficulty();
        });
        UpdateText(index);
        LoadDifficulty();
        UpdateTitle();

    }

    void LoadDifficulty()
    {

        currentDifficulty = (Difficulties)difficultySlider.value;

        switch (currentDifficulty)
        {
            case Difficulties.Easy:
                textHolderDiff.text = easyText.GetLocalizedString();
                break;

            case Difficulties.Medium:
                textHolderDiff.text = mediumText.GetLocalizedString();
                break;

            case Difficulties.Hard:
                textHolderDiff.text = hardText.GetLocalizedString();
                break;
        }
        
    }

        void UpdateDifficulty()
    {
        currentDifficulty = (Difficulties)difficultySlider.value;

        switch (currentDifficulty)
        {
            case Difficulties.Easy:
                textHolderDiff.text = easyText.GetLocalizedString();
                break;

            case Difficulties.Medium:
                textHolderDiff.text = mediumText.GetLocalizedString();
                break;

            case Difficulties.Hard:
                textHolderDiff.text = hardText.GetLocalizedString();
                break;
        }

        Debug.Log("Current Difficulty: " + currentDifficulty);
        PlayClickSound();
    }

    void UpdateTitle()
    {
        switch (destinationScene)
        {
            case 1:
                titleImage.sprite = readingTitle;
                gameModeTitle.text = readingText.GetLocalizedString();
                break;
            case 2:
                titleImage.sprite = synonymTitle;
                gameModeTitle.text = synonymsText.GetLocalizedString();
                break;
            case 3:
                titleImage.sprite = antonymTitle;
                gameModeTitle.text = antonymsText.GetLocalizedString();
                break;
        }
    }

    public void PlayClickSound()
    {
        AudioManager.Instance.PlayClick();
    }


    void UpdateText(int indexNum)
    {
        string[] timeModes = { slowText.GetLocalizedString(), fastText.GetLocalizedString(), "∞" };
        
        textHolder.text = timeModes[indexNum];
        if (indexNum == 2)
        {
            textHolder.fontSizeMax = 90;
            //textHolder.fontSize = 90;
        }
        else
        {
            textHolder.fontSizeMax = 38;
            //textHolder.fontSize = 38;
        }
    }

        public void Previous()
    {
        if (index == 0)
        {
            index = 2;
            UpdateText(index);
        }
        else
        {
            index--;
            UpdateText(index);
        }
    }

    public void Next()
        {
            if (index == 2)
        {
            index = 0;
            UpdateText(index);
        }
        else
        {
            index++;
            UpdateText(index);
        }
        }

    public void StartGameSession()
    {
        PlayerPrefs.SetInt("TimeDuration", index);
        PlayerPrefs.SetInt("DifficultyLevel", (int)currentDifficulty);
        PlayerPrefs.SetInt("DestinationScene", destinationScene);
        PlayerPrefs.Save();
        OpenScene(destinationScene);
    }

    void OpenScene(int destinationScene)
    {
        switch (destinationScene)
        {
            case 1: 
                dest = "Single_Indu";
                break;
            case 2: 
                dest = "Single_Synonyms";
                break;
            case 3: 
                dest = "Single_Antonyms";
                break;
        }

            LoadingScreen.destinationScene = dest;
            LoadingScreen.isMultiplayer = false;

            SceneManager.LoadScene("LoadingScreen");
    }
}
