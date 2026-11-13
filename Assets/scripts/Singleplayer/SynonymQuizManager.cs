using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

public class SynonymQuizManager : MonoBehaviour
{
    public static SynonymQuizManager Instance;

    public float flashAnswerDuration = 2f;

    [Header("CardAnimation")]
    public Animator flashcardAnimator;
    public float artificialDelay = 0.8f;
    public Button flashCard;
    public GameObject blockChoices;

    [Header("Hints and Streaks")]
    public TMP_Text streakTimed;
    public TMP_Text streakUntimed;

    public TMP_Text hintTimed;
    public TMP_Text hintUntimed;

    public TMP_Text totalHintUsed;
    public TMP_Text highestStreakTxt;

    public int currentStreak = 0;
    public int highestStreak = 0;
    int roundMistake = 1;

    int hintUsed = 0;
    public int hintRemaining = 3;

    [Header("UI")]

    public Button[] answerButtons;
    public TMP_Text[] answerTexts;

    [Header("Data")]
    public string jsonFileName = "Kulitan_src";

    private List<SynonymItem> vocabulary;
    private SynonymItem currentQuestion;
    private List<SynonymItem> shuffledVocabulary;
    private int currentQuestionIndex = 0;

    [Header("Flashcard")]



    public GameTimer gameTimer;

    public int score = 0;
    public TMP_Text scoreText, untimedText;


    public List<Button> allButtonsToDisable;
    public GameObject flashCardButton;


    public float flashcardMoveDuration = 0.2f;
    public RectTransform flashcardRect;

    public GameObject choicesGroup;

    // public Image backgroundImage;

    public Sprite normalBackground;
    public Sprite wrongBackground;
    public Sprite correctBackground;
    private Vector2 originalFlashcardPos;
    private Vector2[] originalButtonPositions;
    public float wrongDuration = 1f, correctDuration = 0.9f;
    

    public TMP_Text wordText;
    public GameObject frontSide;
    public GameObject backSide;

    public TMP_Text antonymText;


    [Header("Game Results")]
    private int correctCount = 0;
    public int wrongCount = 0;
    public TMP_Text correctText;
    public TMP_Text wrongText;
    public TMP_Text pointsText, difficultyText, timeText;

    private int difficultyLevel, timeDuration;


    [Header("Difficulty")]
    [SerializeField] private LocalizedString easyText;
    [SerializeField] private LocalizedString mediumText;
    [SerializeField] private LocalizedString hardText;
    [Header("Clock")]
    [SerializeField] private LocalizedString slowText;
    [SerializeField] private LocalizedString fastText;
    [SerializeField] private LocalizedString endlessText;


    void Start()
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();
        if (guessBeh != null)
        {
            guessBeh.NewRound();
        }
        blockChoices.SetActive(false);
        UpdateStreak();
        QuestionDifficulty();
        AudioManager.Instance.PlayGameplayMusic();
        LoadVocabulary();
        GenerateQuestion();
        score = 0;
        UpdateScoreUI();
        originalFlashcardPos = flashcardRect.anchoredPosition;
        UpdateStatsUI();
        PrepareResults();

        originalButtonPositions =
        new Vector2[answerButtons.Length];

        for (int i = 0; i < answerButtons.Length; i++)
        {
            originalButtonPositions[i] =
                answerButtons[i]
                .GetComponent<RectTransform>()
                .anchoredPosition;
        }
    }

    void Update()
    {
        if (gameTimer != null && gameTimer.timeUp)
        {
            UpdateStatsUI();
            DisableAllButtons();
            // GameTimer.Instance.HideAllGameElements();
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

        void QuestionDifficulty()
    {
        difficultyLevel = PlayerPrefs.GetInt("DifficultyLevel", 0);
        switch (difficultyLevel)
        {
            case 0:
                jsonFileName = "SynonymsEasy";
                break;
            case 1:
                jsonFileName = "SynonymsMedium";
                break;
            case 2:
                jsonFileName = "SynonymsHard";
                break;
        }
    }

    public void PrepareResults()
    {

        difficultyLevel = PlayerPrefs.GetInt("DifficultyLevel", 0);
        timeDuration = PlayerPrefs.GetInt("TimeDuration", 0);
        switch (difficultyLevel)
        {
            case 0:
                difficultyText.text = easyText.GetLocalizedString();
                break;
            case 1:
                difficultyText.text = mediumText.GetLocalizedString();
                break;
            case 2:
                difficultyText.text = hardText.GetLocalizedString();
                break;
            default:
                difficultyText.text = "Unknown";
                break;
        }

        switch (timeDuration)
        {
            case 0:
                timeText.text = slowText.GetLocalizedString();
                break;
            case 1:
                timeText.text = fastText.GetLocalizedString();
                break;
            case 2:
                timeText.text = ConvertTime(gameTimer.elapsedTime);
                break;
            default:
                timeText.text = "UNKNOWN";
                break;
        }
    }
    string ConvertTime(int timeVal)
    {
        int minutes = timeVal / 60;
        int seconds = timeVal % 60;

        string formattedTime = string.Format("{0:D2}:{1:D2}", minutes, seconds);

        return formattedTime;
    }
    void LoadQuestionWord()
    {
        wordText.text = currentQuestion.name;

        antonymText.text = currentQuestion.answer;

        frontSide.SetActive(true);
        backSide.SetActive(false);

    }

        public void FlipCard()
    {
        if(hintRemaining > 0)
        {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();
        if (guessBeh != null)
        {
            guessBeh.hintUsed = true;
        } 
            hintRemaining--;
            hintUsed++;
            flashCard.interactable = false;
            UpdateHint();
            StartCoroutine(FlipCardRoutine());
        }
        else return;
    }

    System.Collections.IEnumerator FlipCardRoutine()
    {
        blockChoices.SetActive(true);

        // Front
        frontSide.SetActive(true);
        backSide.SetActive(false);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.CardFlipSfx1();
        }
        if (flashcardAnimator != null)
        {
            flashcardAnimator.ResetTrigger("Flip90");
            flashcardAnimator.ResetTrigger("Flip0");

            flashcardAnimator.SetTrigger("Flip90");
        }

        yield return new WaitForSeconds(artificialDelay);
        // switch back side
        frontSide.SetActive(false);
        backSide.SetActive(true);

        if (flashcardAnimator != null)
        {
            flashcardAnimator.ResetTrigger("Flip90");
            flashcardAnimator.SetTrigger("Flip0");
        }

        // display duration
        yield return new WaitForSeconds(flashAnswerDuration);


        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.CardFlipSfx2();
        }

        if (flashcardAnimator != null)
        {
            flashcardAnimator.ResetTrigger("Flip0");
            flashcardAnimator.SetTrigger("Flip90");
        }

        yield return new WaitForSeconds(artificialDelay);

        // flip back
        frontSide.SetActive(true);
        backSide.SetActive(false);

        if (flashcardAnimator != null)
        {
            flashcardAnimator.ResetTrigger("Flip90");
            flashcardAnimator.SetTrigger("Flip0");
        }

        flashCard.interactable = true;
        blockChoices.SetActive(false);
    }

    void UpdateStatsUI()
    {
        correctText.text = "" + correctCount;
        wrongText.text = "" + wrongCount;
        pointsText.text = "" + score;
        totalHintUsed.text = hintUsed.ToString();
        highestStreakTxt.text = highestStreak.ToString();
    }
    void DisableAllButtons()
    {
        for (int i = 0; i < allButtonsToDisable.Count; i++)
        {
            allButtonsToDisable[i].interactable = false;
        }
    }

    void UpdateScoreUI()
    {
        scoreText.text = "" + score;
        untimedText.text = "" + score;
    }

    void LoadVocabulary()
    {
        TextAsset jsonFile =
            Resources.Load<TextAsset>(jsonFileName);

        SynonymDatabase database =
            JsonUtility.FromJson<SynonymDatabase>(
                jsonFile.text
            );

        vocabulary = new List<SynonymItem>(database.items);

        // Make a separate copy for question ordering
        shuffledVocabulary = new List<SynonymItem>(vocabulary);

        // Shuffle the questions
        ShuffleQuestions();

        currentQuestionIndex = 0;

        Debug.Log("Loaded Words: " + vocabulary.Count);
    }
    void ShuffleQuestions()
    {
        // Fisher-Yates shuffle
        for (int i = shuffledVocabulary.Count - 1; i > 0; i--)
        {
            int randomIndex =
                Random.Range(0, i + 1);

            SynonymItem temp =
                shuffledVocabulary[i];

            shuffledVocabulary[i] =
                shuffledVocabulary[randomIndex];

            shuffledVocabulary[randomIndex] =
                temp;
        }

        // Prevent the first question of a new cycle
        // from being the same as the previous question
        if (shuffledVocabulary.Count > 1 &&
            currentQuestion != null &&
            shuffledVocabulary[0].name == currentQuestion.name)
        {
            int swapIndex =
                Random.Range(1, shuffledVocabulary.Count);

            SynonymItem temp =
                shuffledVocabulary[0];

            shuffledVocabulary[0] =
                shuffledVocabulary[swapIndex];

            shuffledVocabulary[swapIndex] =
                temp;
        }

        Debug.Log(
            "Questions shuffled: " +
            shuffledVocabulary.Count
        );
    }
    void GenerateQuestion()
    {
        var guessBeh =
            FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh != null)
        {
            guessBeh.NewRound();
            guessBeh.isTimerRunning = true;
        }

        roundMistake = 0;

        if (shuffledVocabulary == null ||
            shuffledVocabulary.Count == 0)
        {
            Debug.LogError(
                "SynonymQuizManager: No shuffled questions available."
            );
            return;
        }

        // Get the next question from the shuffled list
        currentQuestion =
            shuffledVocabulary[currentQuestionIndex];

        Debug.Log(
            "Question " +
            (currentQuestionIndex + 1) +
            "/" +
            shuffledVocabulary.Count +
            ": " +
            currentQuestion.name
        );

        // Move to the next question
        currentQuestionIndex++;

        // All questions have been used
        if (currentQuestionIndex >= shuffledVocabulary.Count)
        {
            currentQuestionIndex = 0;

            // Shuffle again for the next cycle
            ShuffleQuestions();

            Debug.Log(
                "All questions used. Starting a new shuffled cycle."
            );
        }

        GenerateChoices();
        LoadQuestionWord();
    }

void GenerateChoices()
{
    List<string> choices =
        new List<string>();

    choices.Add(currentQuestion.answer);

    while (choices.Count < 3)
    {
        int rand =
            Random.Range(0, vocabulary.Count);

        string wrongAnswer =
            vocabulary[rand].answer;

        if (!choices.Contains(wrongAnswer))
        {
            choices.Add(wrongAnswer);
        }
    }

    // Shuffle
    for (int i = 0; i < choices.Count; i++)
    {
        string temp = choices[i];

        int randomIndex =
            Random.Range(i, choices.Count);

        choices[i] = choices[randomIndex];
        choices[randomIndex] = temp;
    }

    // Reset buttons
    for (int i = 0; i < answerButtons.Length; i++)
    {
        answerButtons[i].interactable = true;
        answerTexts[i].color = Color.black;
        answerButtons[i].image.color = Color.white;
    }

    // Assign choices
    for (int i = 0; i < answerTexts.Length; i++)
    {
        answerTexts[i].text = choices[i];

        string selectedAnswer = choices[i];

        Button currentButton = answerButtons[i];

        answerButtons[i].onClick.RemoveAllListeners();

        answerButtons[i].onClick.AddListener(() =>
        {
            CheckAnswer(selectedAnswer, currentButton);
        });
    }
}
    void CheckAnswer(string answer, Button clickedButton)
    {
        if (answer == currentQuestion.answer)
        {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();
        if (guessBeh != null)
        {
            guessBeh.CheckAnswer(true);
        }
            // CORRECT ANSWER
            UpdateStreak();
            score += 10;
            UpdateScoreUI();
            correctCount++;
            AudioManager.Instance.CorrectSfx();
            clickedButton.image.color = Color.green;

            StartCoroutine(CorrectAnswerTransition(clickedButton));
        }
        else
{
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();
        if (guessBeh != null)
        {
            guessBeh.CheckAnswer(false);
        }
    // WRONG ANSWER
    AudioManager.Instance.WrongSfx();

    clickedButton.image.color = Color.red;

    // Change clicked text to red
    TMP_Text clickedText =
        clickedButton.GetComponentInChildren<TMP_Text>();

    if (clickedText != null)
    {
        clickedText.color = Color.white;
    }

    clickedButton.onClick.RemoveAllListeners();

    wrongCount++;
    roundMistake++;
    currentStreak=0;
    UpdateStreakUI();

    StartCoroutine(WrongAnswerTransition());
}
    }

    void EnableAllButtons()
    {
        for (int i = 0; i < allButtonsToDisable.Count; i++)
        {
            allButtonsToDisable[i].interactable = true;
        }
    }


    System.Collections.IEnumerator WrongAnswerTransition()
{
    // hide choices first
    choicesGroup.SetActive(false);
    GameTimer.Instance.HideElements();

    // change background to red
    PersistentEnvironment.Instance.backgroundImg.sprite = wrongBackground;

    // move flashcard to center
    yield return StartCoroutine(MoveFlashcard(Vector2.zero, flashcardMoveDuration));

    yield return new WaitForSeconds(wrongDuration);

    // reset background
    PersistentEnvironment.Instance.backgroundImg.sprite = normalBackground;

    // move flashcard back
    yield return StartCoroutine(MoveFlashcard(originalFlashcardPos, 0.4f));

    // show choices again
    if (gameTimer != null && !gameTimer.timeUp)
    {
        choicesGroup.SetActive(true);
        GameTimer.Instance.ShowElements();
    }

}


System.Collections.IEnumerator CorrectAnswerTransition(Button correctButton)
{
    flashCard.interactable = false;
    DisableAllButtons();
    GameTimer.Instance.PauseGame();
    // show positive background
    PersistentEnvironment.Instance.backgroundImg.sprite = correctBackground;
    GameTimer.Instance.HideElements();

    // hide ONLY wrong buttons
    for (int i = 0; i < answerButtons.Length; i++)
    {
        answerTexts[i].color = Color.white;
        if (answerButtons[i] != correctButton)
        {
            answerButtons[i].gameObject.SetActive(false);
        }
    }

    // move flashcard to center
    StartCoroutine(
        MoveFlashcard(
            Vector2.zero,
            flashcardMoveDuration
        )
    );

    // move correct answer below flashcard
    RectTransform correctRect =
        correctButton.GetComponent<RectTransform>();

    Vector2 targetPos =
    new Vector2(
        correctRect.anchoredPosition.x,
        -43f
    );

    StartCoroutine(
        MoveButton(
            correctRect,
            targetPos,
            0.2f
        )
    );
    yield return new WaitForSeconds(0.2f);
    // memorization pause
    yield return new WaitForSeconds(correctDuration);

    // move flashcard back
    yield return StartCoroutine(
        MoveFlashcard(
            originalFlashcardPos,
            0.2f
        )
    );

    // load next question
    GenerateQuestion();

    // restore all buttons
    for (int i = 0; i < answerButtons.Length; i++)
    {
        answerButtons[i].gameObject.SetActive(true);
    }

    for (int i = 0; i < answerButtons.Length; i++)
    {
        answerButtons[i]
            .GetComponent<RectTransform>()
            .anchoredPosition =
            originalButtonPositions[i];
    }

    PersistentEnvironment.Instance.backgroundImg.sprite = normalBackground;

    if (gameTimer != null && !gameTimer.timeUp)
    {
        GameTimer.Instance.ShowElements();
        EnableAllButtons();
        GameTimer.Instance.ResumeGame();
        flashCard.interactable = true;    
    }

}

    public void HideAllButtons()
    {
        choicesGroup.SetActive(false);
        flashCardButton.SetActive(false);
    }


System.Collections.IEnumerator MoveButton(
    RectTransform rect,
    Vector2 target,
    float duration)
{
    Vector2 startPos = rect.anchoredPosition;

    float time = 0f;

    while (time < duration)
    {
        time += Time.deltaTime;

        rect.anchoredPosition =
            Vector2.Lerp(
                startPos,
                target,
                time / duration
            );

        yield return null;
    }

    rect.anchoredPosition = target;
}


    System.Collections.IEnumerator NextQuestionTransition()
{
    // hide choices
    choicesGroup.SetActive(false);

    // change background (optional)
    PersistentEnvironment.Instance.backgroundImg.sprite = wrongBackground;

    // move flashcard to center
    flashcardRect.anchoredPosition = Vector2.zero;

    yield return new WaitForSeconds(0.6f);

    // load next question
    GenerateQuestion();

    yield return new WaitForSeconds(0.2f);

    // move back to original position
    flashcardRect.anchoredPosition = originalFlashcardPos;

    // restore background
    PersistentEnvironment.Instance.backgroundImg.sprite = normalBackground;

    // show choices again
    choicesGroup.SetActive(true);
}


System.Collections.IEnumerator MoveFlashcard(Vector2 target, float duration)
{
    Vector2 startPos = flashcardRect.anchoredPosition;

    float time = 0f;

    while (time < duration)
    {
        time += Time.deltaTime;

        flashcardRect.anchoredPosition =
            Vector2.Lerp(startPos, target, time / duration);

        yield return null;
    }

    flashcardRect.anchoredPosition = target;
}

    public void UpdateStreak()
    {
        if(roundMistake == 0)
        {
            currentStreak++;

            if(highestStreak <= currentStreak)
            {
                highestStreak = currentStreak;
            }
        }
        else
        {
            currentStreak = 0;
        }

        if(currentStreak % 3 == 0 && currentStreak != 0)
        {
            hintRemaining++;
        }
        UpdateHint();
        UpdateStreakUI();

    }

    public void UpdateStreakUI()
    {
        streakTimed.text = currentStreak.ToString();
        streakUntimed.text = currentStreak.ToString();
    }

    public void UpdateHint()
    {
        hintTimed.text = hintRemaining.ToString();
        hintUntimed.text = hintRemaining.ToString();
    }


}

