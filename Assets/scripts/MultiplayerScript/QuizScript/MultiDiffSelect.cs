using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

public class MultiDiffSelect : MonoBehaviour
{
    public static MultiDiffSelect Instance;


    [Header("Difficulty Slider")]
    public Slider difficultySlider;
    public TMP_Text textHolderDiff;


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


    public enum Difficulties
    {
        Easy,
        Medium,
        Hard
    }


    public Difficulties currentDifficulty;


    [Header("Time Selectors")]
    public Button previousButton;
    public Button nextButton;

    private int index = 0;

    public TMP_Text textHolder;


    [Header("Data To Transfer")]
    public int timeDuration;
    public int difficultyLevel;

    public string destinationScene;


    [Header("Game Mode")]
    public string gameMode;


    [Header("Title Images")]
    public Sprite readingTitle;
    public Sprite synonymTitle;
    public Sprite antonymTitle;

    [Header("Image")]
    public Image titleImage;
    public TMP_Text gameModeTitle;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    private void Start()
    {
        string[] timeModes = { slowText.GetLocalizedString(), fastText.GetLocalizedString(), "∞" };
        // ------------------------------------------
        // Load saved multiplayer settings
        // ------------------------------------------

        timeDuration =
            PlayerPrefs.GetInt(
                "MTimeDuration",
                0
            );

        difficultyLevel =
            PlayerPrefs.GetInt(
                "MDifficultyLevel",
                0
            );

        destinationScene =
            PlayerPrefs.GetString(
                "MDestinationScene",
                "Multi_Indu"
            );


        // ------------------------------------------
        // Determine game mode
        // ------------------------------------------

        DetermineGameMode();


        // ------------------------------------------
        // Configure difficulty slider
        // ------------------------------------------

        ConfigureDifficultySlider();


        // ------------------------------------------
        // Load difficulty
        // ------------------------------------------

        difficultySlider.onValueChanged.AddListener(
            delegate
            {
                UpdateDifficulty();
            }
        );


        // ------------------------------------------
        // Configure time
        // ------------------------------------------

        index = Mathf.Clamp(
            timeDuration,
            0,
            timeModes.Length - 1
        );

        UpdateText(index);


        // ------------------------------------------
        // Load current difficulty
        // ------------------------------------------

        LoadDifficulty();


        // ------------------------------------------
        // Update title
        // ------------------------------------------

        UpdateTitle();
    }


    // =========================================================
    // GAME MODE
    // =========================================================

    void DetermineGameMode()
    {
        switch (destinationScene)
        {
            case "Multi_Indu":

                gameMode = "INDU";

                break;


            case "Multi_Synonym":

                gameMode = "SYNONYM";

                break;


            case "Multi_Antonym":

                gameMode = "ANTONYM";

                break;


            default:

                gameMode = "INDU";

                break;
        }


        Debug.Log(
            "Multiplayer Game Mode: " + gameMode
        );
    }


    // =========================================================
    // DIFFICULTY SLIDER
    // =========================================================

    void ConfigureDifficultySlider()
    {
        if (difficultySlider == null)
        {
            Debug.LogError(
                "Difficulty Slider is not assigned!"
            );

            return;
        }


        if (gameMode == "INDU")
        {
            // Script game mode:
            //
            // 0 = Easy
            // 1 = Medium-Hard

            difficultySlider.minValue = 0;
            difficultySlider.maxValue = 1;
        }
        else
        {
            // Synonym / Antonym:
            //
            // 0 = Easy
            // 1 = Medium
            // 2 = Hard

            difficultySlider.minValue = 0;
            difficultySlider.maxValue = 2;
        }


        // Prevent an old value of 2 from
        // remaining selected after changing
        // into Script mode.

        difficultyLevel =
            Mathf.Clamp(
                difficultyLevel,
                (int)difficultySlider.minValue,
                (int)difficultySlider.maxValue
            );


        difficultySlider.value =
            difficultyLevel;
    }


    // =========================================================
    // LOAD DIFFICULTY
    // =========================================================

    void LoadDifficulty()
    {
        currentDifficulty =(Difficulties) Mathf.RoundToInt(difficultySlider.value);


        UpdateDifficultyText();
    }


    // =========================================================
    // DIFFICULTY CHANGED
    // =========================================================

    void UpdateDifficulty()
    {
        currentDifficulty =
            (Difficulties)
            Mathf.RoundToInt(
                difficultySlider.value
            );


        difficultyLevel =
            (int)currentDifficulty;


        UpdateDifficultyText();


        Debug.Log(
            "Current Multiplayer Difficulty: " +
            currentDifficulty
        );


        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClick();
        }
    }


    // =========================================================
    // DIFFICULTY TEXT
    // =========================================================

    void UpdateDifficultyText()
    {
        if (textHolderDiff == null)
            return;


        if (gameMode == "INDU")
        {
            switch (currentDifficulty)
            {
                case Difficulties.Easy:

                    textHolderDiff.text = easyText.GetLocalizedString();

                    break;


                case Difficulties.Medium:

                    textHolderDiff.text = hardText.GetLocalizedString();

                    break;


                default:

                    textHolderDiff.text = "UNKNOWN";

                    break;
            }
        }
        else
        {
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
    }


    // =========================================================
    // TITLE
    // =========================================================

    void UpdateTitle()
    {
        if (titleImage == null)
            return;


        switch (destinationScene)
        {
            case "Multi_Indu":

                titleImage.sprite =
                    readingTitle;

                gameModeTitle.text = readingText.GetLocalizedString();

                break;


            case "Multi_Synonym":

                titleImage.sprite =
                    synonymTitle;

                gameModeTitle.text = synonymsText.GetLocalizedString();

                break;


            case "Multi_Antonym":

                titleImage.sprite =
                    antonymTitle;

                gameModeTitle.text = antonymsText.GetLocalizedString();
                break;
        }
    }


    // =========================================================
    // TIME
    // =========================================================

    void UpdateText(int indexNum)
    {
        string[] timeModes = { slowText.GetLocalizedString(), fastText.GetLocalizedString(), "∞" };
        textHolder.text =
            timeModes[indexNum];

        if (indexNum == 2)
        {
            textHolder.fontSizeMax = 90;
        }
        else
        {
            textHolder.fontSizeMax = 38;
        }
    }


    public void Previous()
    {
        if (index == 0)
        {
            index = 2;
        }
        else
        {
            index--;
        }


        UpdateText(index);
    }


    public void Next()
    {
        if (index == 2)
        {
            index = 0;
        }
        else
        {
            index++;
        }


        UpdateText(index);
    }


    // =========================================================
    // START GAME SESSION
    // =========================================================

    public void StartGameSession()
    {
        PlayerPrefs.SetInt(
            "MTimeDuration",
            index
        );


        PlayerPrefs.SetInt(
            "MDifficultyLevel",
            (int)currentDifficulty
        );


        PlayerPrefs.SetString(
            "MDestinationScene",
            destinationScene
        );


        PlayerPrefs.SetString(
            "MGameMode",
            gameMode
        );


        PlayerPrefs.Save();


        Debug.Log(
            "=== MULTIPLAYER SETTINGS SAVED ==="
        );


        Debug.Log(
            "Time: " + index
        );


        Debug.Log(
            "Difficulty: " +
            (int)currentDifficulty
        );


        Debug.Log(
            "Game Mode: " +
            gameMode
        );


        Debug.Log(
            "Scene: " +
            destinationScene
        );


        // We intentionally DO NOT load the scene here.
        //
        // The host will eventually press the
        // multiplayer lobby Start button.
    }
}
