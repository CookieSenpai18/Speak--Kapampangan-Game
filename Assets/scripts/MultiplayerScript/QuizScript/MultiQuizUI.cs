using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using System;

public class MultiQuizUI : MonoBehaviour
{
    public Button backButtonUI; 

    public GameObject hiddenBg;
    [Header("Return to Lobby Vote")]
    public Button returnToLobbyButton;
    public TMP_Text returnToLobbyButtonText;
    private bool hasVotedReturnToLobby = false;

    private HashSet<string> eliminatedChoices =
    new HashSet<string>();
    public static MultiQuizUI Instance;

    public Image backgroundImage;
    public Sprite normalBg;
    public Sprite wrongBg;
    public Sprite correctBg;
    public float bgCorrectDelay = 4f;

    [Header("Toast System")]
    public GameObject toastPrefab;
    public Transform toastContainer;

    [Header("Streak")]
    public TMP_Text streakText;
    public TMP_Text untimedStreakText;

    [Header("Score")]
    public TMP_Text scoreText;
    public TMP_Text untimedScoreText;

    [Header("Hints")]

    public TMP_Text hintText;
    public TMP_Text untimedHintText;

    [Header("Game Results")]
    public GameObject gameOverPanel;

    public TMP_Text resultRoundText;
    public TMP_Text resultScoreText;
    public TMP_Text resultMistakesText;
    public TMP_Text resultFirstChoiceText;
    public TMP_Text resultGameModeText;
    public TMP_Text resultDifficultyText;
    public TMP_Text resultTimeText;

    public string gameMode;

    [Header("Player Results")]
    public Transform playerResultsContent;
    public GameObject playerStatsPrefab;

    private List<GameObject> playerResultRows =
        new List<GameObject>();


    [Header("Flashcard Animation")]
    public Animator flashcardAnimator;
    [Header("QuizCard Animation")]
    public Animator quizCardAnimator;
    public float artificialDelay = 0.25f;

    [Header("Flashcard")]
    public Button flashcardButton;


    [Header("Flashcard Panels")]
    public GameObject flashcardFront;
    public GameObject flashcardBack;
    public TMP_Text correctAnswerText;


    [Header("Quiz Card")]
    public Button quizCardButton;
    public TMP_Text quizCardText;



    [Header("Role UI")]
    public GameObject cardHolder;
    public GameObject guesser;

    [Header("Card Holder Choices")]
    public Button[] cardHolderButtons;
    public TMP_Text[] cardHolderTexts;

    [Header("Guesser Choices")]
    public Button[] guesserButtons;
    public TMP_Text[] guesserTexts;

    [Header("Flashcard")]
    public Image characterImage;

    private NetworkPlayer localPlayer;

    private bool isShowingHint = false;

    [Header("Round Transition")]
    public GameObject roundTransition;
    public TMP_Text roundTransitionText;
    public GameObject startUpMessage;
    public float delayBeforeRound = 5.5f;

    [Header("End Game button")]
    public TMP_Text endGameVoteText;
    public Button endGameButton;

    private Coroutine backgroundFxCoroutine;

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
    [SerializeField] private LocalizedString roundText;
    [SerializeField] private LocalizedString waitingOtherPlayerTxt;
    [SerializeField] private LocalizedString endGameVote;
    [SerializeField] private LocalizedString returnToLobbyTxt;


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
        //SetupButtons();
        SetupAnswerButtons();

        SetupFlashcard();

        SetupQuizCard();

        SetupReturnToLobbyButton();

    }
    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame && backButtonUI != null)
        {
            HandleBackButton(backButtonUI);
        }
        if (localPlayer == null)
        {
            TryFindLocalPlayer();
        }
        CheckReturnToLobbyButton();
    }

    public void HandleBackButton(Button btn)
    {
        if (MultiGameManager.Instance == null)
            return;

        // Don't allow pause after the game is over
        if (MultiGameManager.Instance.gameOver)
            return;

        if (btn != null)
        {
            btn.onClick.Invoke();
        }
    }

    private void SetupReturnToLobbyButton()
    {
        if (returnToLobbyButton == null)
        {
            Debug.LogWarning(
                "Return to Lobby button is not assigned."
            );

            return;
        }

        returnToLobbyButton.onClick.RemoveAllListeners();

        returnToLobbyButton.onClick.AddListener(
            VoteReturnToLobby
        );
    }

    private void VoteReturnToLobby()
    {
        if (hasVotedReturnToLobby)
            return;

        if (localPlayer == null)
        {
            Debug.LogWarning(
                "Cannot vote: local player is null."
            );

            return;
        }

        Debug.Log(
            localPlayer.playerName +
            " voted to return to lobby."
        );

        hasVotedReturnToLobby = true;

        localPlayer.CmdVoteReturnToLobby();

        UpdateReturnToLobbyButton();
    }

    public void UpdateReturnToLobbyButton()
    {
        if (returnToLobbyButton == null)
            return;

        if (returnToLobbyButtonText == null)
            return;

        if (hasVotedReturnToLobby)
        {
            int votes = 0;

            if (MultiGameManager.Instance != null)
            {
                votes =
                    MultiGameManager.Instance.returnLobbyVotes;
            }

            int totalPlayers =
                NetworkPlayer.Players.Count;

            returnToLobbyButtonText.text =
                waitingOtherPlayerTxt.GetLocalizedString() +
                votes +
                "/" +
                totalPlayers;

            returnToLobbyButton.interactable = false;
        }
        else
        {
            returnToLobbyButtonText.text =
                returnToLobbyTxt.GetLocalizedString();

            returnToLobbyButton.interactable = true;
        }
    }

    private void CheckReturnToLobbyButton()
    {
        if (returnToLobbyButton == null)
            return;

        QuizNetworkManager manager =
            NetworkManager.singleton as QuizNetworkManager;

        if (manager == null)
        {
            returnToLobbyButton.interactable = false;
            return;
        }

        returnToLobbyButton.interactable =
            manager.HostStillExists;
    }

    public void ResetReturnToLobbyVote()
    {
        hasVotedReturnToLobby = false;

        UpdateReturnToLobbyButton();
    }

    void SetupQuizCard()
    {
        if (quizCardButton == null)
        {
            Debug.LogError(
                "MultiQuizUI: Quiz Card Button is not assigned."
            );

            return;
        }

        quizCardButton.onClick.RemoveAllListeners();

        quizCardButton.onClick.AddListener(
            OnQuizCardClicked
        );

        Debug.Log(
            "MultiQuizUI: Quiz Card button successfully connected."
        );
    }

    void OnQuizCardClicked()
    {
        Debug.Log("=== QUIZ CARD BUTTON CLICKED ===");

        if (localPlayer == null)
            return;

        if (!localPlayer.isQuizHolder)
        {
            Debug.Log(
                "QuizCard ignored because player is not the card holder."
            );
            return;
        }

        if (isShowingHint)
            return;

        if (MultiGameManager.Instance == null)
            return;

        if (MultiGameManager.Instance.sharedHintsRemaining <= 0)
        {
            Debug.Log(
                "QuizCard ignored because no shared hints remain."
            );
            return;
        }

        quizCardButton.interactable = false;

        localPlayer.CmdUseHint();
    }
    public void RefreshQuizCard()
    {
        if (localPlayer == null)
            return;

        if (quizCardText == null)
        {
            Debug.LogError(
                "MultiQuizUI: Quiz Card Text is not assigned."
            );

            return;
        }

        if (string.IsNullOrEmpty(localPlayer.questionName))
            return;

        quizCardText.text =
            localPlayer.questionName;

        Debug.Log(
            "QuizCard question updated: " +
            localPlayer.questionName
        );
    }

    public void ShowGameOver()
    {
        Debug.Log("=== SHOWING GAME OVER PANEL ===");

        // =========================================
        // HIDE QUIZ UI
        // =========================================

        AudioManager.Instance.StopTimesUpSfx();
        DisableQuizUI();

        if (cardHolder != null)
            cardHolder.SetActive(false);

        if (guesser != null)
            guesser.SetActive(false);

        if (roundTransition != null)
            roundTransition.SetActive(false);

        // =========================================
        // SHOW GAME OVER PANEL
        // =========================================

        if (gameOverPanel == null)
        {
            Debug.LogError(
                "MultiQuizUI: Game Over Panel is not assigned."
            );

            return;
        }

        gameOverPanel.SetActive(true);

        // =========================================
        // GET GAME MANAGER
        // =========================================

        if (MultiGameManager.Instance == null)
        {
            Debug.LogError(
                "ShowGameOver: MultiGameManager.Instance is null."
            );

            return;
        }

        MultiGameManager game =
            MultiGameManager.Instance;

        // =========================================
        // ROUND
        // =========================================

        if (resultRoundText != null)
        {
            resultRoundText.text =
                game.currentRound.ToString();
        }

        // =========================================
        // SCORE
        // =========================================

        if (resultScoreText != null)
        {
            resultScoreText.text =
                game.sharedScore.ToString();
        }

        // =========================================
        // MISTAKES
        // =========================================

        if (resultMistakesText != null)
        {
            resultMistakesText.text =
                game.totalMistakes.ToString();
        }

        // =========================================
        // HIGHEST FIRST-CHOICE STREAK
        // =========================================

        if (resultFirstChoiceText != null)
        {
            resultFirstChoiceText.text =
                game.highestConsecutiveFirstChoice.ToString();
        }

        // =========================================
        // GAME MODE
        // =========================================

        if (resultGameModeText != null)
        {
            resultGameModeText.text =
                GetDisplayGameMode();
        }

        // =========================================
        // DIFFICULTY
        // =========================================

        resultDifficultyText.text = GetDisplayDifficulty();

        if (resultTimeText != null)
        {
            if (NetworkRoomSettings.Instance != null &&
                NetworkRoomSettings.Instance.timeDuration == 2)
            {
                int totalSeconds = game.gameplayTime;

                int minutes = totalSeconds / 60;
                int seconds = totalSeconds % 60;

                resultTimeText.text =
                    minutes.ToString("00") +
                    ":" +
                    seconds.ToString("00");
            }
            else
            {
                resultTimeText.text =
                    GetDisplayTimeMode();
            }
        }

        // =========================================
        // TIME
        // =========================================

        if (resultTimeText != null)
        {
            resultTimeText.text =
                GetDisplayTimeMode();
        }
        PopulatePlayerResults();
        Debug.Log(
            "Game Over Results Updated: " +
            "Round=" + game.currentRound +
            ", Score=" + game.sharedScore +
            ", Mistakes=" + game.totalMistakes
        );

        
    }
    public void UpdateScoreUI()
    {
        if (MultiGameManager.Instance == null)
            return;

        string score =
            MultiGameManager.Instance.sharedScore.ToString();

        if (scoreText != null)
            scoreText.text = score;

        if (untimedScoreText != null)
            untimedScoreText.text = score;

    }

    public void UpdateStreakUI()
    {
        if (MultiGameManager.Instance == null)
            return;

        string streak =
            MultiGameManager.Instance.currentFirstChoiceStreak.ToString();

        if (streakText != null)
            streakText.text = streak;

        if (untimedStreakText != null)
            untimedStreakText.text = streak;
    }

    void SetupFlashcard()
    {
        if (flashcardButton == null)
        {
            Debug.LogError(
                "MultiQuizUI: Flashcard Button is not assigned."
            );

            return;
        }

        flashcardButton.onClick.RemoveAllListeners();

        flashcardButton.onClick.AddListener(
            OnFlashcardClicked
        );

        Debug.Log(
            "MultiQuizUI: Flashcard button successfully connected."
        );
    }

    void OnFlashcardClicked()
    {
        Debug.Log("=== FLASHCARD BUTTON CLICKED ===");

        if (localPlayer == null)
        {
            Debug.LogWarning(
                "Flashcard clicked, but localPlayer is null."
            );
            return;
        }

        if (!localPlayer.isQuizHolder)
        {
            Debug.Log(
                "Flashcard ignored because player is not the card holder."
            );
            return;
        }

        if (isShowingHint)
        {
            Debug.Log(
                "Flashcard ignored because a hint is already showing."
            );
            return;
        }

        if (MultiGameManager.Instance == null)
        {
            Debug.LogWarning(
                "Flashcard ignored because MultiGameManager is unavailable."
            );

            return;
        }

        if (MultiGameManager.Instance.sharedHintsRemaining <= 0)
        {
            Debug.Log(
                "Flashcard ignored because no shared hints remain."
            );

            return;
        }

        Debug.Log(
            "Flashcard clicked. Requesting hint..."
        );

        if (flashcardButton != null)
            flashcardButton.interactable = false;

        localPlayer.CmdUseHint();
    }

    public void ShowHint(string correctAnswer)
    {
        if (isShowingHint)
            return;

        isShowingHint = true;

        if (IsTextQuizMode())
        {
            StartCoroutine(
                QuizCardHintRoutine(correctAnswer)
            );
        }
        else
        {
            if (correctAnswerText != null)
            {
                correctAnswerText.text = correctAnswer;
            }

            StartCoroutine(
                HintDisplayRoutine()
            );
        }
    }

    bool IsTextQuizMode()
    {
        if (MultiGameManager.Instance == null)
            return false;

        string mode =
            MultiGameManager.Instance.resultGameMode;

        return mode == "SYNONYM" ||
            mode == "ANTONYM";
    }

    private IEnumerator HintDisplayRoutine()
    {
        // -------------------------------------------------
        // LOCK THE FLASHCARD
        // -------------------------------------------------

        if (flashcardButton != null)
            flashcardButton.interactable = false;

        // -------------------------------------------------
        // MAKE SURE WE START WITH THE QUESTION
        // -------------------------------------------------

        if (flashcardFront != null)
            flashcardFront.SetActive(true);

        if (flashcardBack != null)
            flashcardBack.SetActive(false);

        // -------------------------------------------------
        // FRONT -> 90 DEGREES
        // -------------------------------------------------

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

        // Wait until the card reaches approximately 90 degrees.
        yield return new WaitForSeconds(artificialDelay);

        // -------------------------------------------------
        // SWITCH QUESTION -> ANSWER
        // -------------------------------------------------

        if (flashcardFront != null)
            flashcardFront.SetActive(false);

        if (flashcardBack != null)
            flashcardBack.SetActive(true);

        Debug.Log("Flashcard switched QUESTION -> ANSWER.");

        // -------------------------------------------------
        // 90 DEGREES -> 0 DEGREES
        // -------------------------------------------------

        if (flashcardAnimator != null)
        {
            flashcardAnimator.ResetTrigger("Flip90");
            flashcardAnimator.SetTrigger("Flip0");
        }

        // Wait for the card to return to 0 degrees.
        // yield return new WaitForSeconds(artificialDelay);

        // -------------------------------------------------
        // SHOW ANSWER FOR 2 SECONDS
        // -------------------------------------------------

        yield return new WaitForSeconds(2f);

        // -------------------------------------------------
        // ANSWER -> 90 DEGREES
        // -------------------------------------------------

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.CardFlipSfx2();
        }

        if (flashcardAnimator != null)
        {
            flashcardAnimator.ResetTrigger("Flip0");
            flashcardAnimator.SetTrigger("Flip90");
        }

        // Wait until the card reaches 90 degrees again.
        yield return new WaitForSeconds(artificialDelay);

        // -------------------------------------------------
        // SWITCH ANSWER -> QUESTION
        // -------------------------------------------------

        if (flashcardBack != null)
            flashcardBack.SetActive(false);

        if (flashcardFront != null)
            flashcardFront.SetActive(true);

        Debug.Log("Flashcard switched ANSWER -> QUESTION.");

        // -------------------------------------------------
        // 90 DEGREES -> 0 DEGREES
        // -------------------------------------------------

        if (flashcardAnimator != null)
        {
            flashcardAnimator.ResetTrigger("Flip90");
            flashcardAnimator.SetTrigger("Flip0");
        }

        // Wait for the card to return to normal.
        // yield return new WaitForSeconds(artificialDelay);

        // -------------------------------------------------
        // FINISHED
        // -------------------------------------------------

        isShowingHint = false;

        UpdateFlashcardInteractable();

    }

    private IEnumerator QuizCardHintRoutine(
    string correctAnswer)
    {
        Debug.Log(
            "=== QUIZ CARD HINT STARTED ==="
        );

        if (quizCardButton != null)
            quizCardButton.interactable = false;

        if (quizCardText == null)
        {
            Debug.LogError(
                "QuizCard Text is not assigned."
            );

            isShowingHint = false;
            UpdateFlashcardInteractable();

            yield break;
        }

        if (quizCardAnimator != null)
        {
            quizCardAnimator.ResetTrigger("CFlip90");
            quizCardAnimator.ResetTrigger("CFlip0");

            quizCardAnimator.SetTrigger("CFlip90");
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.CardFlipSfx1();
        }

        // Wait until the card reaches approximately 90 degrees.
        yield return new WaitForSeconds(artificialDelay);

        // -----------------------------------------
        // SAVE QUESTION
        // -----------------------------------------

        string originalQuestion =
            quizCardText.text;

        // -----------------------------------------
        // SHOW ANSWER
        // -----------------------------------------

        quizCardText.text =
            correctAnswer;

        Debug.Log(
            "QuizCard showing hint answer: " +
            correctAnswer
        );

        if (quizCardAnimator != null)
        {
            quizCardAnimator.ResetTrigger("CFlip90");
            quizCardAnimator.SetTrigger("CFlip0");
        }

        // -----------------------------------------
        // KEEP ANSWER VISIBLE
        // -----------------------------------------

        yield return new WaitForSeconds(2f);

        // -----------------------------------------
        // RESTORE QUESTION
        // -----------------------------------------

        if (quizCardAnimator != null)
        {
            quizCardAnimator.ResetTrigger("CFlip0");
            quizCardAnimator.SetTrigger("CFlip90");
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.CardFlipSfx2();
        }

        // Wait until the card reaches 90 degrees again.
        yield return new WaitForSeconds(artificialDelay);

        quizCardText.text =
            originalQuestion;

        Debug.Log(
            "QuizCard restored question: " +
            originalQuestion
        );


        if (quizCardAnimator != null)
        {
            quizCardAnimator.ResetTrigger("CFlip90");
            quizCardAnimator.SetTrigger("CFlip0");
        }
        // -----------------------------------------
        // FINISHED
        // -----------------------------------------

        isShowingHint = false;

        UpdateFlashcardInteractable();
    }

    void UpdateFlashcardInteractable()
    {
        if (localPlayer == null)
        {
            if (flashcardButton != null)
                flashcardButton.interactable = false;

            if (quizCardButton != null)
                quizCardButton.interactable = false;

            return;
        }

        bool canUseHint =
            localPlayer.isQuizHolder &&
            !isShowingHint &&
            MultiGameManager.Instance != null &&
            MultiGameManager.Instance.sharedHintsRemaining > 0;

        // -----------------------------------------
        // INDU FLASHCARD
        // -----------------------------------------

        if (flashcardButton != null)
        {
            flashcardButton.interactable =
                !IsTextQuizMode() && canUseHint;
        }

        // -----------------------------------------
        // SYNONYM / ANTONYM QUIZ CARD
        // -----------------------------------------

        if (quizCardButton != null)
        {
            quizCardButton.interactable =
                IsTextQuizMode() && canUseHint;
        }
    }
    public void UpdateHintUI()
    {
        if (MultiGameManager.Instance == null)
            return;

        string hints =
            MultiGameManager.Instance.sharedHintsRemaining.ToString();

        if (hintText != null)
            hintText.text = hints;

        if (untimedHintText != null)
            untimedHintText.text = hints;

        UpdateFlashcardInteractable();
    }
    public void ResetFlashcard()
    {
        isShowingHint = false;

        if (characterImage != null)
        {
            characterImage.sprite = null;
        }

        // Completely reset the animation.
        if (flashcardAnimator != null)
        {
            flashcardAnimator.Rebind();
            flashcardAnimator.Update(0f);
        }

        if (quizCardAnimator != null)
        {
            quizCardAnimator.Rebind();
            quizCardAnimator.Update(0f);
        }


        if (flashcardFront != null)
            flashcardFront.SetActive(true);

        if (flashcardBack != null)
            flashcardBack.SetActive(false);

        UpdateFlashcardInteractable();
    }
    public void RefreshFlashcard()
    {
        if (localPlayer == null)
            return;

        if (string.IsNullOrEmpty(localPlayer.questionName))
            return;

        if (string.IsNullOrEmpty(localPlayer.flashcardResourcePath))
            return;

        string resourcePath =
            localPlayer.flashcardResourcePath +
            localPlayer.questionName;

        Debug.Log(
            "Loading flashcard: " +
            resourcePath
        );

        Sprite image =
            Resources.Load<Sprite>(resourcePath);

        if (image == null)
        {
            Debug.LogError(
                "Could not load flashcard: " +
                resourcePath
            );

            return;
        }

        characterImage.sprite = image;

        Debug.Log(
            "Flashcard successfully loaded: " +
            resourcePath
        );
    }
    void TryFindLocalPlayer()
    {
        if (!NetworkClient.active)
            return;

        if (NetworkClient.localPlayer == null)
            return;

        localPlayer =
            NetworkClient.localPlayer.GetComponent<NetworkPlayer>();

        if (localPlayer != null)
        {
            Debug.Log(
                "MultiQuizUI found local player: "
                + localPlayer.playerName
            );

            // RefreshUI();
        }
    }

    public void RefreshUI()
    {
        if (!NetworkClient.active)
            return;

        if (roundTransition != null &&
            roundTransition.activeSelf)
        {
            return;
        }

        localPlayer = NetworkClient.localPlayer != null
            ? NetworkClient.localPlayer.GetComponent<NetworkPlayer>()
            : null;

        if (localPlayer == null)
            return;


        UpdateHintUI();
        UpdateScoreUI();

        string gameMode =
    MultiGameManager.Instance != null
    ? MultiGameManager.Instance.resultGameMode
    : "";

    if (gameMode == "SYNONYM" ||
        gameMode == "ANTONYM")
    {
        ShowQuizCard();
    }
    else
    {
        ShowFlashcard();
    }

        // Load the flashcard for the current round
        // LoadFlashcard();

        if (localPlayer.isQuizHolder)
        {
            ShowCardHolderUI();
        }
        else
        {
            ShowGuesserUI();
        }
    }

    void ShowQuizCard()
    {
        if (flashcardButton != null)
            flashcardButton.gameObject.SetActive(false);

        if (quizCardButton != null)
            quizCardButton.gameObject.SetActive(true);

        RefreshQuizCard();
    }

    void ShowFlashcard()
    {
        if (quizCardButton != null)
            quizCardButton.gameObject.SetActive(false);

        if (flashcardButton != null)
            flashcardButton.gameObject.SetActive(true);

        RefreshFlashcard();
    }

    void ShowCardHolderUI()
    {
        if (roundTransition != null)
        {
            roundTransition.SetActive(false);
        }
        cardHolder.SetActive(true);
        guesser.SetActive(false);

        Debug.Log(
            "=== CARD HOLDER UI ===\n" +
            "Player: " + localPlayer.playerName + "\n" +
            "Choice 1: " + localPlayer.choice1 + "\n" +
            "Choice 2: " + localPlayer.choice2 + "\n" +
            "Choice 3: " + localPlayer.choice3
        );

        SetChoices(
            cardHolderTexts,
            localPlayer.choice1,
            localPlayer.choice2,
            localPlayer.choice3
        );
        RefreshQuestionCard();
    }

    public void RefreshQuestionCard()
    {
        if (MultiGameManager.Instance == null)
            return;

        string gameMode =
            MultiGameManager.Instance.resultGameMode;

        if (gameMode == "SYNONYM" ||
            gameMode == "ANTONYM")
        {
            RefreshQuizCard();
        }
        else
        {
            RefreshFlashcard();
        }
    }

    void ShowGuesserUI()
    {
        if (roundTransition != null)
        {
            roundTransition.SetActive(false);
        }
        cardHolder.SetActive(false);
        guesser.SetActive(true);

        Debug.Log(
            "=== LOCAL PLAYER UI ===\n" +
            "Player: " + localPlayer.playerName + "\n" +
            "Quiz Holder: " + localPlayer.isQuizHolder + "\n" +
            "Correct Player: " + localPlayer.isCorrectAnswerPlayer + "\n" +
            "Choice 1: " + localPlayer.choice1 + "\n" +
            "Choice 2: " + localPlayer.choice2 + "\n" +
            "Choice 3: " + localPlayer.choice3
        );

        SetChoices(
            guesserTexts,
            localPlayer.choice1,
            localPlayer.choice2,
            localPlayer.choice3
        );
    }

    void SetChoices(
        TMP_Text[] texts,
        string choice1,
        string choice2,
        string choice3)
    {
        if (texts == null || texts.Length < 3)
            return;

        texts[0].text = choice1;
        texts[1].text = choice2;
        texts[2].text = choice3;
    }


    void SetupAnswerButtons()
    {
        if (cardHolderButtons != null)
        {
            for (int i = 0; i < cardHolderButtons.Length; i++)
            {
                int index = i;

                cardHolderButtons[i].onClick.RemoveAllListeners();

                cardHolderButtons[i].onClick.AddListener(() =>
                {
                    SubmitAnswer(
                        cardHolderTexts[index].text,
                        cardHolderButtons[index]
                    );
                });
            }
        }

        if (guesserButtons != null)
        {
            for (int i = 0; i < guesserButtons.Length; i++)
            {
                int index = i;

                guesserButtons[i].onClick.RemoveAllListeners();

                guesserButtons[i].onClick.AddListener(() =>
                {
                    SubmitAnswer(
                        guesserTexts[index].text,
                        guesserButtons[index]
                    );
                });
            }
        }
    }
    private void SubmitAnswer(
        string selectedAnswer,
        Button selectedButton)
    {
        if (localPlayer == null)
            return;

        if (localPlayer.answered)
            return;

        // Prevent selecting the same wrong answer again.
        if (eliminatedChoices.Contains(selectedAnswer))
            return;

        Debug.Log(
            "Submitting answer: " +
            selectedAnswer
        );

        if(MultiGameManager.Instance.correctAnswerStr != selectedAnswer)
        {
            ChoiceButtonAnimation animation =
            selectedButton.GetComponent<ChoiceButtonAnimation>();    
            animation.PlayIncorrect(false);

            selectedButton.image.color = Color.red;

            
        }
        else
        {
            ChoiceButtonAnimation animation =
            selectedButton.GetComponent<ChoiceButtonAnimation>();    

            animation.PlayIncorrect(true);
        }
        selectedButton.GetComponentInChildren<TMP_Text>().color = Color.white;

        // Immediately disable this button locally.
        selectedButton.interactable = false;

        // Remember that this choice has been eliminated.
        eliminatedChoices.Add(selectedAnswer);

        // Send answer to server.
        localPlayer.CmdSubmitAnswer(selectedAnswer);
    }

    private void ResetButtons(Button[] buttons)
    {
        if (buttons == null)
            return;

        foreach (Button button in buttons)
        {
            if (button == null)
                continue;

            button.interactable = true;
            button.image.color = Color.white;

            button.GetComponentInChildren<TMP_Text>().color = Color.black;
        }
    }

    public void ResetForNewRound()
    {
        eliminatedChoices.Clear();

        ResetButtons(cardHolderButtons);
        ResetButtons(guesserButtons);

        // Make sure flashcard can be interacted with again.
        // if (characterImage != null)
        // {
        //     Button flashcardButton =
        //         characterImage.GetComponent<Button>();

        //     if (flashcardButton != null)
        //         flashcardButton.interactable = true;
        // }
        ResetFlashcardAnimation();

        ResetFlashcard();
    }

    public void RoundCompleted(
        int completedRound,
        string correctAnswer)
    {
        Debug.Log(
            "MultiQuizUI: Round " +
            completedRound +
            " completed."
        );

        // Turn the correct answer green.
        HighlightCorrectAnswer(correctAnswer);

        // Disable ALL quiz interaction.
        DisableQuizUI();
    }

    void HighlightCorrectAnswer(
        string correctAnswer)
    {
        if (localPlayer == null)
            return;

        Button[] buttons;
        TMP_Text[] texts;

        if (localPlayer.isQuizHolder)
        {
            buttons = cardHolderButtons;
            texts = cardHolderTexts;
        }
        else
        {
            buttons = guesserButtons;
            texts = guesserTexts;
        }

        if (buttons == null || texts == null)
            return;

        for (int i = 0;
            i < buttons.Length && i < texts.Length;
            i++)
        {
            if (texts[i].text == correctAnswer)
            {
                if (buttons[i].image != null)
                {
                    buttons[i].image.color = Color.green;
                }

                break;
            }
        }
    }

    void DisableQuizUI()
    {
        // Disable card holder buttons.
        if (cardHolderButtons != null)
        {
            foreach (Button button in cardHolderButtons)
            {
                if (button != null)
                    button.interactable = false;
            }
        }

        // Disable guesser buttons.
        if (guesserButtons != null)
        {
            foreach (Button button in guesserButtons)
            {
                if (button != null)
                    button.interactable = false;
            }
        }

        // Disable the flashcard interaction.
        if (characterImage != null)
        {
            Button flashcardButton =
                characterImage.GetComponent<Button>();

            if (flashcardButton != null)
                flashcardButton.interactable = false;
        }
    }

    public void ShowNextRound(int nextRound)
    {
        ResetFlashcard();

        cardHolder.SetActive(false);
        guesser.SetActive(false);

        if (roundTransition == null ||
            roundTransitionText == null)
        {
            Debug.LogWarning(
                "Round transition UI is not assigned."
            );

            return;
        }

        roundTransitionText.text =
            roundText.GetLocalizedString() + " " + nextRound;

        roundTransition.SetActive(true);

        Debug.Log(
            "Showing ROUND " +
            nextRound
        );
    }
    public void StartNewRoundUI()
    {
        ResetReturnToLobbyVote();
        Debug.Log(
            "Starting new round UI."
        );

        // Reset all choice colors and interaction.
        ResetForNewRound();

        // Hide the round transition.
        if (roundTransition != null)
        {
            roundTransition.SetActive(false);
        }

        // Show the appropriate UI again.
        RefreshUI();
        RefreshQuestionCard();
    }

    string GetDisplayGameMode()
    {
        if (MultiGameManager.Instance == null)
            return "UNKNOWN";

        switch (MultiGameManager.Instance.resultGameMode)
        {
            case "INDU":
                return readingText.GetLocalizedString().ToUpper();

            case "SYNONYM":
                return synonymsText.GetLocalizedString().ToUpper();

            case "ANTONYM":
                return antonymsText.GetLocalizedString().ToUpper();

            default:
                return "UNKNOWN";
        }
    }


    string GetDisplayDifficulty()
    {
        if (MultiGameManager.Instance == null)
            return "UNKNOWN";

        MultiGameManager game =
            MultiGameManager.Instance;

        if (game.resultGameMode == "INDU")
        {
            switch (game.resultDifficulty)
            {
                case 0:
                    return easyText.GetLocalizedString().ToUpper();

                case 1:
                    return hardText.GetLocalizedString().ToUpper();

                default:
                    return "UNKNOWN";
            }
        }

        switch (game.resultDifficulty)
        {
            case 0:
                return easyText.GetLocalizedString().ToUpper();

            case 1:
                return mediumText.GetLocalizedString().ToUpper();

            case 2:
                return hardText.GetLocalizedString().ToUpper();

            default:
                return "UNKNOWN";
        }
    }


    string GetDisplayTimeMode()
    {
        if (MultiGameManager.Instance == null)
            return "UNKNOWN";

        MultiGameManager game =
            MultiGameManager.Instance;

        // =========================================
        // UNTIMED
        // =========================================

        if (game.resultTimeDuration == 2)
        {
            int totalSeconds =
                game.elapsedGameTime;

            int minutes =
                totalSeconds / 60;

            int seconds =
                totalSeconds % 60;

            return minutes.ToString("00") +
                ":" +
                seconds.ToString("00");
        }


        // =========================================
        // TIMED
        // =========================================

        switch (game.resultTimeDuration)
        {
            case 0:
                return slowText.GetLocalizedString();

            case 1:
                return fastText.GetLocalizedString();

            default:
                return "UNKNOWN";
        }
    }

    public void EndGameButton()
    {
        Debug.Log("=== END GAME BUTTON PRESSED ===");

        if (MultiGameManager.Instance == null)
        {
            Debug.LogError(
                "EndGameButton: MultiGameManager.Instance is null."
            );

            return;
        }

        NetworkPlayer localPlayer =
            NetworkClient.localPlayer
            ?.GetComponent<NetworkPlayer>();

        if (localPlayer == null)
        {
            Debug.LogError(
                "EndGameButton: Local NetworkPlayer not found."
            );

            return;
        }

        localPlayer.CmdEndGame();
    }
    
        private void PopulatePlayerResults()
    {
        Debug.Log(
            "=== POPULATING PLAYER RESULTS ==="
        );

        if (playerResultsContent == null)
        {
            Debug.LogError(
                "MultiQuizUI: Player Results Content is not assigned."
            );

            return;
        }

        if (playerStatsPrefab == null)
        {
            Debug.LogError(
                "MultiQuizUI: Player Stats Prefab is not assigned."
            );

            return;
        }

        // -----------------------------------------
        // CLEAR OLD ROWS
        // -----------------------------------------

        foreach (GameObject row in playerResultRows)
        {
            if (row != null)
            {
                Destroy(row);
            }
        }

        playerResultRows.Clear();

        // -----------------------------------------
        // MAKE SURE PLAYERS EXIST
        // -----------------------------------------

        if (NetworkPlayer.Players == null ||
            NetworkPlayer.Players.Count == 0)
        {
            Debug.LogWarning(
                "No NetworkPlayers found for result panel."
            );

            return;
        }

        // -----------------------------------------
        // CREATE ONE ROW PER PLAYER
        // -----------------------------------------

        for (int i = 0;
            i < NetworkPlayer.Players.Count;
            i++)
        {
            NetworkPlayer player =
                NetworkPlayer.Players[i];

            if (player == null)
                continue;

            GameObject rowObject =
                Instantiate(
                    playerStatsPrefab,
                    playerResultsContent
                );

            PlayerStatsUI row =
                rowObject.GetComponent<PlayerStatsUI>();

            if (row == null)
            {
                Debug.LogError(
                    "Player Stats Prefab does not contain PlayerStatsUI."
                );

                Destroy(rowObject);
                continue;
            }

            row.SetPlayerData(
                i + 1,
                player
            );

            playerResultRows.Add(rowObject);

            Debug.Log(
                "Result Row Created: " +
                (i + 1) +
                " - " +
                player.playerName
            );
        }
    }

    public void ShowToast(string message)
    {
        if (toastPrefab == null)
        {
            Debug.LogWarning(
                "Toast prefab is not assigned."
            );

            return;
        }

        if (toastContainer == null)
        {
            Debug.LogWarning(
                "Toast container is not assigned."
            );

            return;
        }

        GameObject toastObject =
            Instantiate(
                toastPrefab,
                toastContainer
            );

        ToastUI toast =
            toastObject.GetComponent<ToastUI>();

        if (toast != null)
        {
            toast.Show(message);
        }
        else
        {
            Debug.LogWarning(
                "Toast prefab does not contain ToastUI."
            );
        }
    }

    public void ShowPlayerLeftToast(string playerName)
    {
        string message =
            playerName + " left the room.";


        ShowToast(message);
    }

    public void ShowHintToast(string playername)
    {
        string message;
        message = playername + " used hint.";
        ShowToast(message);
    }
    public void ShowAnswerToast(
    string playerName,
    string selectedAnswer,
    bool isCorrect)
    {
        string message;

        if (isCorrect)
        {
            if(gameMode == "INDU")
                message =  playerName + " answered '" + selectedAnswer + "'";
            else
                message = playerName + " answered '" + selectedAnswer + "'" +" for '" + MultiGameManager.Instance.questionName + "'";
        }
        else
        {
            message =
                "X " +
                playerName +
                " answered '" +
                selectedAnswer +
                "'";
        }

        ShowToast(message);
    }

    public void ShowFirstRound()
    {
        ResetFlashcard();

        cardHolder.SetActive(false);
        guesser.SetActive(false);

        StartCoroutine(DisplayFirstRoundUI());

    }

    IEnumerator DisplayFirstRoundUI()
    {
        startUpMessage.SetActive(true);

        yield return new WaitForSeconds(delayBeforeRound);

        startUpMessage.SetActive(false);

        roundTransitionText.text = roundText.GetLocalizedString() + " 1" ;

        roundTransition.SetActive(true);

    }

    public void ResetFlashcardAnimation()
    {
        if (flashcardAnimator != null)
        {
            flashcardAnimator.Rebind();
            flashcardAnimator.Update(0f);
        }

        if (quizCardAnimator != null)
        {
            quizCardAnimator.Rebind();
            quizCardAnimator.Update(0f);
        }

        if (flashcardFront != null)
            flashcardFront.SetActive(true);

        if (flashcardBack != null)
            flashcardBack.SetActive(false);

        isShowingHint = false;
    }

    public void TriggerFx(bool isCorrect)
    {
        // Stop the previous background effect
        if (backgroundFxCoroutine != null)
        {
            StopCoroutine(backgroundFxCoroutine);
        }

        // Start a fresh effect
        backgroundFxCoroutine =
            StartCoroutine(BackgroundFxRoutine(isCorrect));
    }

    private IEnumerator BackgroundFxRoutine(bool isCorrect)
    {
        if (isCorrect)
        {
            backgroundImage.sprite = correctBg;

            yield return new WaitForSeconds(bgCorrectDelay);
        }
        else
        {
            backgroundImage.sprite = wrongBg;

            yield return new WaitForSeconds(1.5f);
        }

        backgroundImage.sprite = normalBg;

        backgroundFxCoroutine = null;
    }

    public void UpdateEndGameVotes(int votes,int players)
    {
        if (endGameVoteText != null)
        {
            endGameVoteText.text =
                $"{endGameVote.GetLocalizedString()} {votes}/{players}";
        }
    }

    public void VoteEndGame()
    {
        if (NetworkClient.localPlayer == null)
            return;

        NetworkPlayer player =
            NetworkClient.localPlayer
            .GetComponent<NetworkPlayer>();

        if (player == null)
            return;

        endGameButton.interactable = false;

        player.CmdEndGame();
    }

    public void ToggleHiddenBg(bool isActive)
    {
        if(hiddenBg != null)
        {
            hiddenBg.SetActive(isActive);
        }
    }
}