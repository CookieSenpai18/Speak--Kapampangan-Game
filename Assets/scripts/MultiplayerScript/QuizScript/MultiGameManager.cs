using System.Collections.Generic;
using Mirror;
using UnityEngine;
using System.Collections;
using TMPro;

public class MultiGameManager : NetworkBehaviour
{
    public static MultiGameManager Instance;

    public bool timerPaused = false;

    [Header("Game State")]
    [SyncVar]
    public int currentRound = 0;

    [SyncVar(hook = nameof(OnSharedScoreChanged))]
    public int sharedScore = 0;

    [SyncVar]
    public bool gameStarted = false;

    [Header("Game Timer")]

    [SyncVar(hook = nameof(OnTimeRemainingChanged))]
    public int timeRemaining;

    [SyncVar(hook = nameof(OnElapsedGameTimeChanged))]
    public int elapsedGameTime = 0;

    [SyncVar]
    public bool gameTimeUp = false;

    [SyncVar]
    public int gameplayTime = 0;

    [Header("Round State")]
    [SyncVar]
    public int cardHolderIndex = 0;

    [SyncVar]
    public int correctAnswerPlayerIndex = -1;

    [SyncVar]
    public bool roundEnding = false;

 


    [Header("Hint Settings")]

    [SerializeField]
    private int startingHints = 3;

    [SyncVar(hook = nameof(OnSharedHintsChanged))]
    public int sharedHintsRemaining;

    private List<VocabularyItem> vocabulary;
    private Dictionary<string, string> questionFlashcardFolders =
    new Dictionary<string, string>();

    private VocabularyItem currentQuestion;
    private List<VocabularyItem> shuffledVocabulary;
    private int currentQuestionIndex = 0;

    private NetworkRoomSettings roomSettings;

    [Header("Round Scoring")]
    private int currentRoundMistakes = 0;

    [SyncVar]
    public int totalMistakes = 0;

    [SyncVar]
    public int highestConsecutiveFirstChoice = 0;

    [SyncVar(hook = nameof(OnStreakChanged))]
    public int currentFirstChoiceStreak = 0;

    [SyncVar(hook = nameof(OnGameModeChanged))]
    public string gameMd;

    [SyncVar]
    public bool gameOver = false;

    [SyncVar]
    public string resultGameMode;

    [SyncVar]
    public int resultDifficulty;

    [SyncVar]
    public int resultTimeDuration;

    [SyncVar]
    public string questionName;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    [SyncVar(hook = nameof(OnReturnLobbyVotesChanged))]
    public int returnLobbyVotes = 0;

    
    private HashSet<NetworkPlayer> returnLobbyVoters =
        new HashSet<NetworkPlayer>();

    private HashSet<NetworkPlayer> endGameVoters =
    new HashSet<NetworkPlayer>();

    private int endGameVotes = 0;

    [SyncVar(hook = nameof(OnCorrectAnswerChanged))]
    public string correctAnswerStr;
    [SyncVar(hook = nameof(OnFrozenChanged))]
    public bool isFrozen = false;
    [SyncVar(hook = nameof(OnBgActiveChanged))]
    public bool bgActive = false;

    [ServerCallback]
    private void Update()
    {
        if (gameStarted)
            return;

        if (NetworkPlayer.Players.Count >= 2)
        {
            gameStarted = true;

            Debug.Log(
                "Players detected. Starting first round."
            );

            StartFirstRound();
        }
    }

    private void OnReturnLobbyVotesChanged(
    int oldValue,
    int newValue)
    {
        MultiQuizUI.Instance?.UpdateReturnToLobbyButton();
    }

    private void OnCorrectAnswerChanged(string oldValue, string newValue)
    {
        
    }

    private void OnGameModeChanged(string oldValue, string newValue)
    {
        if(MultiQuizUI.Instance != null)
        {
            MultiQuizUI.Instance.gameMode = resultGameMode;
        }
    }

    private void OnFrozenChanged(bool oldValue, bool newValue)
    {
        
    }

    private void OnBgActiveChanged(bool oldValue, bool newValue)
    {
        MultiQuizUI.Instance.ToggleHiddenBg(bgActive);
    }
    [Server]
    public void ServerRegisterLobbyVote(NetworkPlayer player)
    {
        if (player == null)
            return;

        // Prevent the same player from voting twice.
        if (returnLobbyVoters.Contains(player))
            return;

        returnLobbyVoters.Add(player);

        returnLobbyVotes =
            returnLobbyVoters.Count;

        Debug.Log(
            "Return to Lobby vote: " +
            returnLobbyVotes +
            "/" +
            NetworkPlayer.Players.Count
        );

        int connectedPlayers =
        NetworkServer.connections.Count;

        // Everyone currently connected must vote.
        if (returnLobbyVotes >= connectedPlayers)
        {
            Debug.Log(
                "=== ALL PLAYERS VOTED TO RETURN TO LOBBY ==="
            );


        // Tell every client to switch to Main Menu music
        foreach (NetworkPlayer p in NetworkPlayer.Players)
        {
            if (p != null)
            {
                p.RpcPlayMainMenuMusic();
            }
        }


            ResetReturnLobbyVotes();


            LoadingScreen.destinationScene = "Multi_Host";
            LoadingScreen.isMultiplayer = true;

            NetworkManager.singleton.ServerChangeScene(
                "LoadingScreen"
            );
        }
    }

    [Server]
    public void ResetReturnLobbyVotes()
    {
        returnLobbyVoters.Clear();
        returnLobbyVotes = 0;
    }

    public override void OnStartServer()
    {
    base.OnStartServer();


    Debug.Log("=== MULTI GAME MANAGER STARTED ===");

    roomSettings =
    FindAnyObjectByType<NetworkRoomSettings>();

    if (roomSettings == null)
    {
        Debug.LogError(
            "MultiGameManager could not find NetworkRoomSettings!"
        );

        return;
    }

    Debug.Log(
        "Room settings successfully found."
    );

        Debug.Log(
            "Game Mode: " + roomSettings.GetGameMode()
        );

        Debug.Log(
            "Difficulty: " + roomSettings.GetDifficulty()
        );

        Debug.Log(
            "Time Mode: " + roomSettings.GetTimeMode()
        );

        LoadVocabulary();

    }

    private void OnSharedHintsChanged(int oldValue, int newValue)
    {
        if (MultiQuizUI.Instance != null)
        {
            MultiQuizUI.Instance.UpdateHintUI();
        }
    }


    // =========================================================
    // VOCABULARY
    // =========================================================

    [Server]
    void LoadVocabulary()
    {
        string gameMode = roomSettings.GetGameMode();
        int difficulty = roomSettings.difficultyLevel;

        vocabulary = new List<VocabularyItem>();

        questionFlashcardFolders =
            new Dictionary<string, string>();

        // =========================================
        // SCRIPT / INDU
        // =========================================

        if (gameMode == "INDU")
        {
            // EASY
            if (difficulty == 0)
            {
                LoadVocabularyFile(
                    "pakamatesiuala",
                    "Pakamate_Siuala/"
                );

                LoadVocabularyFile(
                    "indungsulat",
                    "Indung_Sulat/"
                );
            }

            // MEDIUM / HARD
            else if (difficulty == 1)
            {
                LoadVocabularyFile(
                    "anaksulat",
                    "Anak_Sulat/"
                );
            }

            else
            {
                Debug.LogError(
                    "Invalid INDU difficulty: " +
                    difficulty
                );

                return;
            }
        }

        // =========================================
        // SYNONYM
        // =========================================

        else if (gameMode == "SYNONYM")
        {
            switch (difficulty)
            {
                case 0:
                    LoadVocabularyFile(
                        "SynonymsEasy",
                        "Synonym/"
                    );
                    break;

                case 1:
                    LoadVocabularyFile(
                        "SynonymsMedium",
                        "Synonym/"
                    );
                    break;

                case 2:
                    LoadVocabularyFile(
                        "SynonymsHard",
                        "Synonym/"
                    );
                    break;
            }
        }

        // =========================================
        // ANTONYM
        // =========================================

        else if (gameMode == "ANTONYM")
        {
            switch (difficulty)
            {
                case 0:
                    LoadVocabularyFile(
                        "AntonymsEasy",
                        "Antonym/"
                    );
                    break;

                case 1:
                    LoadVocabularyFile(
                        "AntonymsMedium",
                        "Antonym/"
                    );
                    break;

                case 2:
                    LoadVocabularyFile(
                        "AntonymsHard",
                        "Antonym/"
                    );
                    break;
            }
        }

        else
        {
            Debug.LogError(
                "Unknown game mode: " +
                gameMode
            );

            return;
        }

        Debug.Log(
            "Total vocabulary loaded: " +
            vocabulary.Count
        );

        shuffledVocabulary = new List<VocabularyItem>(vocabulary);

        ShuffleQuestions();

        currentQuestionIndex = 0;
    }

    [Server]
    void ShuffleQuestions()
    {
        for (int i = shuffledVocabulary.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            VocabularyItem temp = shuffledVocabulary[i];

            shuffledVocabulary[i] =
                shuffledVocabulary[randomIndex];

            shuffledVocabulary[randomIndex] =
                temp;
        }

        // Prevent the first question of the new cycle
        // from being the same as the previous question.
        if (shuffledVocabulary.Count > 1 &&
            currentQuestion != null &&
            shuffledVocabulary[0].name == currentQuestion.name)
        {
            int swapIndex =
                Random.Range(1, shuffledVocabulary.Count);

            VocabularyItem temp =
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

[Server]
void LoadVocabularyFile(
    string fileName,
    string flashcardFolder)
{
    Debug.Log(
        "Loading vocabulary file: " +
        fileName
    );

    TextAsset jsonFile =
        Resources.Load<TextAsset>(fileName);

    if (jsonFile == null)
    {
        Debug.LogError(
            "Could not find JSON file: " +
            fileName
        );

        return;
    }

    VocabularyDatabase database =
        JsonUtility.FromJson<VocabularyDatabase>(
            jsonFile.text
        );

    if (database == null ||
        database.items == null)
    {
        Debug.LogError(
            "Failed to read vocabulary database: " +
            fileName
        );

        return;
    }

    foreach (VocabularyItem item in database.items)
    {
        vocabulary.Add(item);

        questionFlashcardFolders[item.name] =
            flashcardFolder;
    }

    Debug.Log(
        "Loaded " +
        database.items.Length +
        " items from " +
        fileName +
        " -> " +
        flashcardFolder
    );
}


    [Server]
    string GetJsonFileName()
    {
        string gameMode =
            roomSettings.GetGameMode();

        int difficulty =
            roomSettings.difficultyLevel;


        // =========================================
        // SCRIPT / INDU
        // =========================================

        if (gameMode == "INDU")
        {
            switch (difficulty)
            {
                case 0:
                    return "pakamatesiuala";

                case 1:
                    // Temporary.
                    // We will combine Medium + Hard later.
                    return "indungsulat";

                default:
                    Debug.LogError(
                        "Invalid INDU difficulty: " +
                        difficulty
                    );

                    return "pakamatesiuala";
            }
        }


        // =========================================
        // SYNONYM
        // =========================================

        if (gameMode == "SYNONYM")
        {
            switch (difficulty)
            {
                case 0:
                    return "SynonymsEasy";

                case 1:
                    return "SynonymsMedium";

                case 2:
                    return "SynonymsHard";

                default:
                    Debug.LogError(
                        "Invalid SYNONYM difficulty: " +
                        difficulty
                    );

                    return "SynonymsEasy";
            }
        }



        // =========================================
        // ANTONYM
        // =========================================

        if (gameMode == "ANTONYM")
        {
            switch (difficulty)
            {
                case 0:
                    return "AntonymsEasy";

                case 1:
                    return "AntonymsMedium";

                case 2:
                    return "AntonymsHard";

                default:
                    Debug.LogError(
                        "Invalid ANTONYM difficulty: " +
                        difficulty
                    );

                    return "AntonymsEasy";
            }
        }


        Debug.LogError(
            "Unknown game mode or difficulty."
        );

        return "";
    }


    // =========================================================
    // ROUND
    // =========================================================

    [Server]
    void StartFirstRound()
    {

        foreach (NetworkPlayer player in NetworkPlayer.Players)
        {
            if (player == null)
                continue;

            player.totalCorrectAnswers = 0;
            player.totalMistakes = 0;
            player.totalHintsUsed = 0;

            player.score = 0;

            player.currentFirstChoiceStreak = 0;
            player.highestFirstChoiceStreak = 0;
        }
        
        resultGameMode = roomSettings.GetGameMode(); 
        resultDifficulty = roomSettings.difficultyLevel;
        resultTimeDuration = roomSettings.timeDuration;

        elapsedGameTime = 0;

        gameplayTime = 0;

        currentRound = 1;

        sharedScore = 0;

        totalMistakes = 0;

        currentFirstChoiceStreak = 0;

        highestConsecutiveFirstChoice = 0;

        cardHolderIndex = 0;

        sharedHintsRemaining = startingHints;

        gameStarted = true;

        gameTimeUp = false;

        switch (roomSettings.timeDuration)
        {
            case 0:
                timeRemaining = 120;
                break;

            case 1:
                timeRemaining = 60;
                break;

            case 2:
                timeRemaining = -1;
                break;

            default:
                timeRemaining = 120;
                break;
        }

        // StartCoroutine(GameTimerRoutine());

        StartCoroutine(StartFirstRoundRoutine());

        // GenerateRound();

        // RpcShowFirstRound();

        RpcPlayBgm();

    }

    [Server]
    IEnumerator StartFirstRoundRoutine()
    {
        yield return new WaitForSeconds(1f);
        // Tell all clients to show "ROUND 1".
        RpcShowFirstRound();

        // Keep the transition visible for few seconds.
        yield return new WaitForSeconds(5.5f);

        // Generate Round 1 on the server.
        GenerateRound();

        // Tell all clients to hide the transition
        // and show their appropriate quiz UI.
        RpcStartNewRoundUI();

        gameMd = roomSettings.GetGameMode();

        // Start the actual game timer AFTER the transition.
        StartCoroutine(GameTimerRoutine());

        
    }

    [ClientRpc]
    void RpcShowFirstRound()
    {
        if (MultiQuizUI.Instance != null)
        {
            MultiQuizUI.Instance.ShowFirstRound();
        }
        if (MultiTimer.Instance != null)
        {
            MultiTimer.Instance.HideScoreBoard();
        }
    }

    [Server]
    IEnumerator GameTimerRoutine()
    {
        Debug.Log("=== MULTIPLAYER GAME TIMER STARTED ===");

        // =========================================
        // UNTIMED MODE
        // =========================================

        if (timeRemaining == -1)
        {
            Debug.Log(
                "=== MULTIPLAYER UNTIMED MODE ==="
            );

            elapsedGameTime = 0;

            while (!gameOver)
            {
                yield return new WaitForSeconds(1f);

                if (gameOver || isFrozen)
                    break;

                elapsedGameTime++;
            }

            Debug.Log(
                "=== MULTIPLAYER UNTIMED TIMER STOPPED ==="
            );

            yield break;
        }


        // =========================================
        // TIMED MODE
        // =========================================

        while (timeRemaining > 0 && !gameTimeUp)
        {
            yield return new WaitForSeconds(1f);

            if (roundEnding)
                continue;

            
            if (timerPaused)
                continue;

            if (gameTimeUp)
                break;

            timeRemaining--;
        }


        // =========================================
        // TIME UP
        // =========================================

        if (!gameTimeUp)
        {
            gameTimeUp = true;

            Debug.Log(
                "=== MULTIPLAYER GAME TIME UP ==="
            );
            EndMultiplayerGame();
        }
    }

    [Server]
    void EndMultiplayerGame()
    {
        Debug.Log("=== MULTIPLAYER GAME OVER ===");

        roundEnding = true;

        RpcGameOver();
    }

    [ClientRpc]
    void RpcGameOver()
    {
        Debug.Log("GAME OVER received from server.");

        MultiTimer.Instance.HideObjects();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TimesUpSfx();
        }

        if (MultiQuizUI.Instance != null)
        {
            MultiQuizUI.Instance.ShowGameOver();
        }
        
        var statsRecorder =
            FindAnyObjectByType<MultiplayerStatsRecorder>();

        if (statsRecorder == null)
        {
            Debug.LogError(
                "=== MULTIPLAYER STATS RECORDER NOT FOUND ==="
            );

            return;
        }

        Debug.Log(
            "=== MULTIPLAYER STATS RECORDER FOUND ==="
        );

        statsRecorder.RecordGameData();
    }

    [Server]
    void GenerateRound()
    {
        MultiQuizUI.Instance.UpdateScoreUI();
        currentRoundMistakes = 0;
        if (vocabulary == null ||
            vocabulary.Count == 0)
        {
            Debug.LogError(
                "Cannot generate round. " +
                "Vocabulary is empty."
            );

            return;
        }


        if (NetworkPlayer.Players.Count == 0)
        {
            Debug.LogError(
                "Cannot generate round. " +
                "No players connected."
            );

            return;
        }


        // =========================================
        // SELECT QUESTION
        // =========================================

        if (shuffledVocabulary == null ||
            shuffledVocabulary.Count == 0)
        {
            Debug.LogError(
                "Cannot generate round. " +
                "Shuffled vocabulary is empty."
            );

            return;
        }

        currentQuestion =
            shuffledVocabulary[currentQuestionIndex];

        // Debug.Log(
        //     "Question " +
        //     (currentQuestionIndex + 1) +
        //     "/" +
        //     shuffledVocabulary.Count +
        //     ": " +
        //     currentQuestion.name
        // );

        questionName = currentQuestion.name;
        currentQuestionIndex++;

        if (currentQuestionIndex >= shuffledVocabulary.Count)
        {
            currentQuestionIndex = 0;

            ShuffleQuestions();

            Debug.Log(
                "All questions used. " +
                "Starting a new shuffled cycle."
            );
        }

        // string flashcardFolder = GetFlashcardFolder();

        // =========================================
        // CARD HOLDER
        // =========================================

        if (cardHolderIndex >=
            NetworkPlayer.Players.Count)
        {
            cardHolderIndex = 0;
        }


        NetworkPlayer cardHolder =
            NetworkPlayer.Players[
                cardHolderIndex
            ];

        // sharedHintsRemaining = startingHints;   

        // =========================================
        // CORRECT ANSWER PLAYER
        // =========================================

        correctAnswerPlayerIndex =
            Random.Range(
                0,
                NetworkPlayer.Players.Count
            );


        NetworkPlayer correctPlayer =
            NetworkPlayer.Players[
                correctAnswerPlayerIndex
            ];


        // =========================================
        // DEBUG
        // =========================================

        Debug.Log(
            "================================"
        );

        Debug.Log(
            "MULTIPLAYER ROUND " +
            currentRound
        );

        Debug.Log(
            "Question: " +
            currentQuestion.name
        );

        Debug.Log(
            "Correct Answer: " +
            currentQuestion.answer
        );

        Debug.Log(
            "Card Holder: " +
            cardHolder.playerName
        );

        Debug.Log(
            "Correct Answer Player: " +
            correctPlayer.playerName
        );

        Debug.Log(
            "================================"
        );


        // =========================================
        // SET PLAYER STATES
        // =========================================

    for (int i = 0;
        i < NetworkPlayer.Players.Count;
        i++)
    {
        NetworkPlayer player =
            NetworkPlayer.Players[i];

        player.isQuizHolder =
            (i == cardHolderIndex);

        player.answered = false;
    }


        // =========================================
        // GENERATE CHOICES
        // =========================================

        GeneratePlayerChoices();
    }

    private void OnSharedScoreChanged(
    int oldValue,
    int newValue)
    {
        if (MultiQuizUI.Instance != null)
        {
            MultiQuizUI.Instance.UpdateScoreUI();
        }
    }

    [Server]
    string GetFlashcardFolder()
    {
        string gameMode =
            roomSettings.GetGameMode();

        int difficulty =
            roomSettings.difficultyLevel;

        if (gameMode == "INDU")
        {
            switch (difficulty)
            {
                case 0:
                    return "Pakamate_Siuala/";

                case 1:
                    return "Indung_Sulat/";

                case 2:
                    return "Anak_Sulat/";
            }
        }

        if (gameMode == "SYNONYM")
        {
            return "Synonym/";
        }

        if (gameMode == "ANTONYM")
        {
            return "Antonym/";
        }

        Debug.LogError(
            "Could not determine flashcard folder."
        );

        return "";
    }

    // =========================================================
    // CHOICE GENERATION
    // =========================================================
 
    [Server]
    void GeneratePlayerChoices()
    {
        Debug.Log("Generating player choices...");

        // -----------------------------------------
        // Build a list of unique answers
        // -----------------------------------------

        List<string> allAnswers = new List<string>();

        foreach (VocabularyItem item in vocabulary)
        {
            if (!allAnswers.Contains(item.answer))
            {
                allAnswers.Add(item.answer);
            }
        }

        // We need at least 3 unique answers
        if (allAnswers.Count < 3)
        {
            Debug.LogError(
                "Not enough unique answers to generate choices."
            );

            return;
        }

        // -----------------------------------------
        // Create wrong-answer pool
        // -----------------------------------------

        List<string> wrongAnswers =
            new List<string>(allAnswers);

        wrongAnswers.Remove(currentQuestion.answer);

        if (wrongAnswers.Count < 2)
        {
            Debug.LogError(
                "Not enough wrong answers for 3 choices."
            );

            return;
        }

        // -----------------------------------------
        // Generate choices for every player
        // -----------------------------------------

        for (
            int playerIndex = 0;
            playerIndex < NetworkPlayer.Players.Count;
            playerIndex++
        )
        {
            NetworkPlayer player =
                NetworkPlayer.Players[playerIndex];

            List<string> choices =
                new List<string>();

            // -------------------------------------
            // Correct-answer player
            // -------------------------------------

            if (playerIndex == correctAnswerPlayerIndex)
            {
                choices.Add(currentQuestion.answer);
            }

            player.questionName = currentQuestion.name;

            // -------------------------------------
            // Fill remaining choices
            // -------------------------------------

            while (choices.Count < 3)
            {
                string randomAnswer =
                    wrongAnswers[
                        Random.Range(
                            0,
                            wrongAnswers.Count
                        )
                    ];

                if (!choices.Contains(randomAnswer))
                {
                    choices.Add(randomAnswer);
                }
            }

            // -------------------------------------
            // Shuffle this player's choices
            // -------------------------------------

            for (int i = 0; i < choices.Count; i++)
            {
                int randomIndex =
                    Random.Range(
                        i,
                        choices.Count
                    );

                string temp = choices[i];

                choices[i] =
                    choices[randomIndex];

                choices[randomIndex] =
                    temp;
            }

            // -------------------------------------
            // Assign SyncVars
            // -------------------------------------

            player.choice1 = choices[0];
            player.choice2 = choices[1];
            player.choice3 = choices[2];


            player.quizRound = currentRound;

            player.questionName =
                currentQuestion.name;

            if (questionFlashcardFolders.TryGetValue(
                currentQuestion.name,
                out string folder))
            {
                player.flashcardResourcePath = folder;
            }
            else
            {
                Debug.LogError(
                    "No flashcard folder found for question: " +
                    currentQuestion.name
                );

                player.flashcardResourcePath = "";
            }


            player.isCorrectAnswerPlayer =
                (playerIndex == correctAnswerPlayerIndex);

            player.answered = false;

            // -------------------------------------
            // Debug
            // -------------------------------------

            Debug.Log(
                "Player " +
                (playerIndex + 1) +
                " (" +
                player.playerName +
                ")"
            );

            Debug.Log(
                "  Correct Player: " +
                player.isCorrectAnswerPlayer
            );

            Debug.Log(
                "  Choice 1: " +
                player.choice1
            );

            Debug.Log(
                "  Choice 2: " +
                player.choice2
            );

            Debug.Log(
                "  Choice 3: " +
                player.choice3
            );
        }
        correctAnswerStr = currentQuestion.answer;
    }


    // =========================================================
    // ANSWER PROCESSING
    // =========================================================

    [Server]
    public void ServerProcessAnswer(
        NetworkPlayer player,
        string selectedAnswer)
    {
        MultiQuizUI.Instance.UpdateScoreUI();

        if (player == null)
        {
            Debug.LogError(
                "ServerProcessAnswer: Player is null."
            );

            return;
        }

        // Round is already ending.
        if (roundEnding)
        {
            return;
        }

        // Player already found the correct answer.
        if (player.answered)
        {
            Debug.Log(
                player.playerName +
                " already found the correct answer."
            );

            return;
        }

        // Make sure the submitted answer is actually
        // one of this player's three choices.
        bool isOneOfPlayersChoices =
            selectedAnswer == player.choice1 ||
            selectedAnswer == player.choice2 ||
            selectedAnswer == player.choice3;

        if (!isOneOfPlayersChoices)
        {
            Debug.LogWarning(
                player.playerName +
                " submitted an invalid choice: " +
                selectedAnswer
            );

            return;
        }

        bool isCorrect =
            selectedAnswer == currentQuestion.answer;

        RpcPlayChoiceSfx(isCorrect);

            RpcShowAnswerToast(
                player.playerName,
                selectedAnswer,
                isCorrect
            );

        Debug.Log(
            "Answer received from " +
            player.playerName +
            ": " +
            selectedAnswer
        );

        // ==========================================
        // CORRECT ANSWER
        // ==========================================

        if (isCorrect)
        {
            player.answered = true;

            player.totalCorrectAnswers++;

            timerPaused = true;


            int roundScore;

if (currentRoundMistakes == 0)
{
    // Solved on the first try.
    roundScore = 15;

    // Increase first-choice streak.
    currentFirstChoiceStreak++;

    if(currentFirstChoiceStreak % 3 == 0)
    {
        sharedHintsRemaining++;
        Debug.Log("Added 1 hint for 3 consecutive streak");
    }

    // Save highest streak.
    if (currentFirstChoiceStreak > highestConsecutiveFirstChoice)
    {
        highestConsecutiveFirstChoice =
            currentFirstChoiceStreak;
    }
    }
    else if (currentRoundMistakes == 1)
    {
        // Solved with exactly one mistake.
        roundScore = 10;

        // First-choice streak is broken.
        currentFirstChoiceStreak = 0;
    }
    else
    {
        // Two or more mistakes.
        roundScore = Mathf.Max(
            10 - currentRoundMistakes,
            5
        );

        // First-choice streak is broken.
        currentFirstChoiceStreak = 0;
    }


            sharedScore += roundScore;

            Debug.Log(
                player.playerName +
                " answered CORRECTLY!"
            );

            Debug.Log(
                "Mistakes this round: " +
                currentRoundMistakes
            );

            Debug.Log(
                "Points earned this round: " +
                roundScore
            );

            Debug.Log(
                "Total score: " +
                player.score
            );

            roundEnding = true;

            RpcRoundCompleted(
                currentRound,
                currentQuestion.answer
            );

            StartCoroutine(
                StartNextRoundAfterDelay()
            );

            return;
        }

        // ==========================================
        // WRONG ANSWER
        // ==========================================

        Debug.Log(
            player.playerName +
            " answered WRONG."
        );

        // Count this mistake for the current round.
        player.totalMistakes++;
        currentRoundMistakes++;
        totalMistakes++;
        currentFirstChoiceStreak = 0;


        // IMPORTANT:
        //
        // Do NOT set player.answered = true.
        //
        // The player can continue selecting
        // the remaining choices.
    }

    [ClientRpc]
    void RpcPlayChoiceSfx(bool isCorrect)
    {
        MultiQuizUI.Instance.TriggerFx(isCorrect);
        if (isCorrect)
        {
            AudioManager.Instance.CorrectSfx();
        }
        else
        {
            AudioManager.Instance.WrongSfx();
        }
    }

    [ClientRpc]
    void RpcShowAnswerToast(
        string playerName,
        string selectedAnswer,
        bool isCorrect)
    {
        if (MultiQuizUI.Instance == null)
            return;

        MultiQuizUI.Instance.ShowAnswerToast(
            playerName,
            selectedAnswer,
            isCorrect
        );
    }

    [ClientRpc]
    void RpcShowHint(string playerName)
    {
        MultiQuizUI.Instance.ShowHintToast(playerName);
    }

    // =========================================================
    // ROUND COMPLETION
    // =========================================================

    [ClientRpc]
    void RpcRoundCompleted(
        int completedRound,
        string correctAnswer)
    {
        if (MultiQuizUI.Instance == null)
            return;

        MultiQuizUI.Instance.RoundCompleted(
            completedRound,
            correctAnswer
        );
    }


    // =========================================================
    // NEXT ROUND
    // =========================================================

    [Server]
    IEnumerator StartNextRoundAfterDelay()
    {
        // ==========================================
        // WAIT 2 SECONDS
        // ==========================================

        yield return new WaitForSeconds(2f);

        // ==========================================
        // SHOW ROUND TRANSITION
        // ==========================================

        int nextRound = currentRound + 1;

        RpcShowNextRound(nextRound);

        // ==========================================
        // WAIT 2 SECONDS
        // ==========================================

        yield return new WaitForSeconds(2f);

        // ==========================================
        // START NEXT ROUND
        // ==========================================

        currentRound++;

        // Rotate card holder.
        cardHolderIndex++;

        if (cardHolderIndex >=
            NetworkPlayer.Players.Count)
        {
            cardHolderIndex = 0;
        }

        roundEnding = false;

        timerPaused = false;

        // Generate the new question and choices.
        GenerateRound();

        // Tell clients that the new round is ready.
        RpcStartNewRoundUI();
    }


    // =========================================================
    // ROUND TRANSITION RPC
    // =========================================================

    [ClientRpc]
    void RpcShowNextRound(int nextRound)
    {

        if (MultiQuizUI.Instance != null)
        {
            MultiQuizUI.Instance.ShowNextRound(nextRound);
        }

        if (MultiTimer.Instance != null)
        {
            MultiTimer.Instance.HideScoreBoard();
        }
    }


    // =========================================================
    // START NEW ROUND UI RPC
    // =========================================================

    [ClientRpc]
    void RpcStartNewRoundUI()
    {
        MultiTimer.Instance.UpdateTimerModeUI();
        if (MultiQuizUI.Instance != null)
        {
            MultiQuizUI.Instance.StartNewRoundUI();
        }
    }



    [Server]
    public void ServerProcessHint(NetworkPlayer player)
    {
        if (player == null)
        {
            Debug.LogError(
                "ServerProcessHint: Player is null."
            );

            return;
        }

        Debug.Log(
            "HINT REQUEST RECEIVED FROM: " +
            player.playerName
        );

        // Only the card holder can use the flashcard.
        if (!player.isQuizHolder)
        {
            Debug.LogWarning(
                player.playerName +
                " is not the card holder."
            );

            return;
        }

        // Don't allow hints after the round has ended.
        if (roundEnding)
        {
            Debug.LogWarning(
                "Hint rejected because round is ending."
            );

            return;
        }

        // No shared hints remaining.
        if (sharedHintsRemaining <= 0)
        {
            Debug.LogWarning(
                "No shared hints remaining."
            );

            return;
        }

        // =========================================
        // CONSUME SHARED HINT
        // =========================================

        sharedHintsRemaining--;

        player.totalHintsUsed++;

        RpcShowHint(player.playerName);

        Debug.Log(
            "HINT CONSUMED!"
        );

        Debug.Log(
            player.playerName +
            " used a hint. Shared hints remaining: " +
            sharedHintsRemaining
        );

        // =========================================
        // SEND CORRECT ANSWER TO CARD HOLDER
        // =========================================

        TargetShowHint(
            player.connectionToClient,
            currentQuestion.answer
        );
    }
    [TargetRpc]
    void TargetShowHint(
        NetworkConnection target,
        string correctAnswer)
    {
        if (MultiQuizUI.Instance != null)
        {
            MultiQuizUI.Instance.ShowHint(correctAnswer);
        }
    }

    private void OnTimeRemainingChanged(
    int oldValue,
    int newValue)
    {
        
    }

    [Server]
    public void ServerEndGame()
    {
        if (gameOver)
            return;

        gameOver = true;
        gameTimeUp = true;
        roundEnding = true;

        Debug.Log(
            "=== GAME ENDED BY PLAYER ==="
        );

        RpcGameOver();
    }

    private void OnElapsedGameTimeChanged(
    int oldValue,
    int newValue)
    {

    }

    private void OnStreakChanged(
    int oldValue, int newValue)
    {
        MultiQuizUI.Instance.UpdateStreakUI();
    }
    [ClientRpc]
    void RpcPlayBgm()
    {
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
            Debug.Log("Playing Game Music");
        }
    }

    [ClientRpc]
    public void RpcShowPlayerLeftToast(string playerName)
    {
        if (MultiQuizUI.Instance == null)
            return;

        MultiQuizUI.Instance.ShowPlayerLeftToast(
            playerName
        );
    }

    [Server]
public void CheckPlayersAfterDisconnect()
{
    // =========================================
    // GAME IS ALREADY OVER
    // =========================================

    if (gameOver)
        return;

    // =========================================
    // GAME HAS NOT STARTED
    // =========================================

    if (!gameStarted)
        return;

    // =========================================
    // CHECK REMAINING PLAYERS
    // =========================================

    int remainingPlayers =
        NetworkPlayer.Players.Count;

    Debug.Log(
        "Players remaining after disconnect: " +
        remainingPlayers
    );

    // =========================================
    // ONLY ONE PLAYER REMAINS
    // =========================================

    if (remainingPlayers == 1)
    {
        Debug.Log(
            "=== ONLY ONE PLAYER REMAINS ==="
        );

        Debug.Log(
            "Remaining player: " +
            NetworkPlayer.Players[0].playerName
        );

        gameOver = true;
        gameTimeUp = true;
        roundEnding = true;

        RpcGameOver();
    }
}

[Server]
public void ServerRegisterEndGameVote(NetworkPlayer player)
{
    if (player == null)
        return;

    // Prevent duplicate votes
    if (endGameVoters.Contains(player))
        return;

    endGameVoters.Add(player);

    endGameVotes = endGameVoters.Count;

    int connectedPlayers =
        NetworkServer.connections.Count;

    Debug.Log(
        $"End the Game {endGameVotes}/{connectedPlayers}"
    );

    // Tell everyone about the vote
    RpcShowEndGameVoteToast(
        player.playerName,
        endGameVotes,
        connectedPlayers
    );

    // Tell everyone the updated vote count
    RpcUpdateEndGameVotes(
        endGameVotes,
        connectedPlayers
    );

    // Check if enough players voted
    CheckEndGameVotes();
}

    [ClientRpc]
    private void RpcShowEndGameVoteToast(
        string playerName,
        int votes,
        int totalPlayers)
    {
        string message =
            playerName +
            " voted to end the game (" +
            votes +
            "/" +
            totalPlayers +
            ")";

        Debug.Log(message);
        MultiQuizUI.Instance.ShowToast(message);
    }

    [Server]
    private void ResetEndGameVotes()
    {
        endGameVoters.Clear();
        endGameVotes = 0;
    }

    [Server]
    private void CheckEndGameVotes()
    {
        int connectedPlayers =
            NetworkServer.connections.Count;

        if (connectedPlayers <= 0)
            return;

        if (endGameVotes >= connectedPlayers)
        {
            Debug.Log(
                "=== ALL PLAYERS VOTED TO END GAME ==="
            );

            endGameVoters.Clear();
            endGameVotes = 0;

            // Your existing end-game logic here
            ServerEndGame();
        }
    }

    [ClientRpc]
    private void RpcUpdateEndGameVotes(
        int votes,
        int players)
    {
        Debug.Log(
            $"End game votes {votes}/{players}"
        );

        // Call your UI method here
        MultiQuizUI.Instance?.UpdateEndGameVotes(
            votes,
            players
        );
    }


    [Server]
    public void ServerPlayerDisconnected(
        NetworkPlayer player)
    {
        if (player == null)
            return;

        // Remove their vote if they had voted
        endGameVoters.Remove(player);

        endGameVotes =
            endGameVoters.Count;

        int connectedPlayers =
            NetworkServer.connections.Count;

        Debug.Log(
            $"Player disconnected. End game votes: " +
            $"{endGameVotes}/{connectedPlayers}"
        );

        RpcUpdateEndGameVotes(
            endGameVotes,
            connectedPlayers
        );

        CheckEndGameVotes();
    }
}

