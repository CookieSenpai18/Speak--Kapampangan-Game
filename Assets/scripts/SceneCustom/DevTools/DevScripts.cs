using UnityEngine;
using TMPro;

public class DevScripts : MonoBehaviour
{

    public TMP_Text displayLog;

    public GameObject generalPanel;
    public GameObject singlePlayerMode;
    public GameObject multiplayerMode;
    public GameObject guessingMechanics;

    public void SwitchPanel(int index)
    {
        generalPanel.SetActive(false);
        singlePlayerMode.SetActive(false);
        multiplayerMode.SetActive(false);
        guessingMechanics.SetActive(false);
        switch (index)
        {
            case 0: generalPanel.SetActive(true);
                break;
            case 1: singlePlayerMode.SetActive(true);
                break;
            case 2: multiplayerMode.SetActive(true);
                break;
            case 3: guessingMechanics.SetActive(true);
                break;
            default: displayLog.text = "Invalid script";
                break;
        }
    }

// FOR GUESSING BEHAVIOR TESTING
    public void TimeStamp(bool isCorrect)
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";
        guessBeh.CheckAnswer(isCorrect);
    }

    public void StartRound()
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";
        guessBeh.NewRound();
    }

    public void ToggleTimer()
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";
        guessBeh.isTimerRunning = !guessBeh.isTimerRunning;
    }

    public void ToggleHint()
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";
        guessBeh.hintUsed = !guessBeh.hintUsed;
    }

    public void ModifyRoundTransition(TMP_InputField newDelayInput)
    {
        string newDelay = newDelayInput.text;
        if (!float.TryParse(newDelay, out float delay))
        {
            displayLog.text = $"Invalid round transition delay: '{newDelay}'";

            displayLog.text =
                "Invalid delay: " + newDelay;

            return;
        }
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";
        guessBeh.transitionRoundDelay = delay;
    }

    public void ModifyHintTransition(TMP_InputField newDelayInput)
    {
        string newDelay = newDelayInput.text;
        if (!float.TryParse(newDelay, out float delay))
        {
            displayLog.text = $"Invalid hint transition delay: '{newDelay}'";

            displayLog.text =
                "Invalid delay: " + newDelay;

            return;
        }

        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";
        guessBeh.transitionHintDelay = delay;
    }

    public void ModifyTimeBasis(TMP_InputField newTimeBasisInput)
    {
        string newTimeBasis = newTimeBasisInput.text;

        if (!float.TryParse(newTimeBasis, out float delay))
        {
            displayLog.text = $"Invalid max time basis: '{newTimeBasis}'";
            return;
        }
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";
        guessBeh.timeBasis = delay;
    }

    public void ModifyIgnoreTime(TMP_InputField newIgnoreTime)
    {
        string newTime = newIgnoreTime.text;

        if (!float.TryParse(newTime, out float setTime))
        {
            displayLog.text = $"Invalid value: '{newTime}'";
            return;
        }
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";
        guessBeh.maxIgnoreTime = setTime;
    }

    public void ModifyReqLen(TMP_InputField newLen)
    {
        string newLength = newLen.text;

        if (!int.TryParse(newLength, out int setReq))
        {
            displayLog.text = $"Invalid value: '{newLength}'";
            return;
        }
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";
        guessBeh.maxIgnoreTime = setReq;
    }

    public void EvaluateResult()
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";
        guessBeh.EvaluateResults();       
    }

    public void ResetStats()
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";
        guessBeh.NewRound();        
        guessBeh.currentRound = 0;
        guessBeh.lowProbability = 0;
        guessBeh.mediumProbability = 0;
        guessBeh.highProbability = 0;
        guessBeh.highestProbDisplay.text = "Probability Level: <color=\"white\"> Null";
        guessBeh.warningPanel.SetActive(false);
        guessBeh.averageStampSpeed.Clear();
    }

    public void TogglePanel()
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";
        guessBeh.displayPanel = !guessBeh.displayPanel;
    }

    public void IncreaseRound(bool isIncrement)
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";
        if (isIncrement)
        {
            guessBeh.currentRound++;
        }
        else
        {
            guessBeh.currentRound = Mathf.Max(0, guessBeh.currentRound - 1);
        }
    }

    public void IncreaseLow(bool isIncrement)
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";
        if (isIncrement)
        {
            guessBeh.lowProbability++;
        }
        else
        {
            guessBeh.lowProbability = Mathf.Max(0, guessBeh.lowProbability - 1);
        }
        
    }

    public void IncreaseMedium(bool isIncrement)
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";

        if (isIncrement)
        {
            guessBeh.mediumProbability++;
        }
        else
        {
            guessBeh.mediumProbability = Mathf.Max(0, guessBeh.mediumProbability - 1);
        }
        
    }

    public void IncreaseHigh(bool isIncrement)
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";
        if (isIncrement)
        {
            guessBeh.highProbability++;
        }
        else
        {
            guessBeh.highProbability = Mathf.Max(0, guessBeh.highProbability - 1);
        }
    }

    public void DecreaseAttempt()
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();

        if (guessBeh == null)
        {
            Debug.LogWarning("GuessingBehavior not loaded.");
            displayLog.text = "GuessingBehavior not loaded.";
            return;
        }
        displayLog.text = "";
        int attempts = guessBeh.attempts;

        if (attempts == 1)
        {
            guessBeh.timeStamp1 = 0f;
            guessBeh.attempts = 0;
        }        
        else if(attempts == 2)
        {
            guessBeh.timeStamp2 = 0f;
            guessBeh.attempts = 1;
        }
        else if(attempts == 3)
        {
            guessBeh.timeStamp3 = 0f;
            guessBeh.attempts = 2;
        }
        else
        {
            guessBeh.attempts = 0;
        }
    }

    public void ToggleAdaptivity(bool isAdaptive)
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();
        if (isAdaptive)
        {
            PlayerPrefs.SetInt("AdaptiveResponse", 1);
            PlayerPrefs.Save();
            displayLog.text = "User average response will now adapt at the end of each sessions";
            if (guessBeh != null)
                guessBeh.isAdaptive = true;
        }
        else
        {
            PlayerPrefs.SetInt("AdaptiveResponse", 0);
            PlayerPrefs.Save();
            displayLog.text = "Response adaptivity disabled.";
            if (guessBeh != null)
                guessBeh.isAdaptive = false;            
        }
    }

    public void ToggleHasMinRequirement(bool val)
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();
        if (val)
        {
            PlayerPrefs.SetInt("HasRequire", 1);
            PlayerPrefs.Save();
            displayLog.text = "Detecting will no longer trigger until minimum played games reached.";
            if (guessBeh != null)
                guessBeh.hasRequiredPlayAmount = true;
        }
        else
        {
            PlayerPrefs.SetInt("HasRequire", 0);
            PlayerPrefs.Save();
            displayLog.text = "Detecting will trigger every session";
            if (guessBeh != null)
                guessBeh.hasRequiredPlayAmount = false;            
        }
    }

    public void ChangeRequiredPlayed(string val)
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();
        if (guessBeh == null)
        {
            displayLog.text = "Guessing not loaded yet";
            return;
        }
        if (!int.TryParse(val, out int setReq))
            guessBeh.requiredAmount = setReq;
        displayLog.text = "Updated minimum game requirement";        
    }


    // Single Player
    public void ToggleExtraBackground()
    {
        var sceneMana = FindAnyObjectByType<SceneManagerScript>();

        if(sceneMana != null)
        {
            if(sceneMana.secretBaackground != null)
            {
                sceneMana.secretBaackground.SetActive(!sceneMana.secretBaackground.activeSelf);
                displayLog.text = "Background switched.";
            }
            else
            displayLog.text = "Background not detected.";
        }
        else
        displayLog.text = "Scene manager not found";
    }

    public void AddHint()
    {
        var indu = FindAnyObjectByType<QuizManager>();
        var syno = FindAnyObjectByType<SynonymQuizManager>();
        var anto = FindAnyObjectByType<AntonymQuizManager>();


        if(indu != null)
        {
            indu.hintRemaining++;
            indu.UpdateHint();
        }
        if(syno != null)
        {
            syno.hintRemaining++;
            syno.UpdateHint();
        }
        if(anto != null)
        {
            anto.hintRemaining++;
            anto.UpdateHint();
        }
    }

    public void AddStreak()
    {
        var indu = FindAnyObjectByType<QuizManager>();
        var syno = FindAnyObjectByType<SynonymQuizManager>();
        var anto = FindAnyObjectByType<AntonymQuizManager>();


        if(indu != null)
        {
            indu.currentStreak++;
            indu.UpdateStreakUI();
        }
        if(syno != null)
        {
            syno.currentStreak++;
            syno.UpdateStreakUI();
        }
        if(anto != null)
        {
            anto.currentStreak++;
            anto.UpdateStreakUI();
        }        
    }

    public void ToggleGameClock()
    {
        var timeSet = FindAnyObjectByType<GameTimer>();

        if(timeSet == null)
        {
            displayLog.text = "Game timer not found.";
            return;
        }
        timeSet.isFrozen = !timeSet.isFrozen;
    }

    public void HintDisplayDuration(TMP_InputField newVal)
    {
        string newValue = newVal.text;

        if (!float.TryParse(newValue, out float setValue))
        {
            displayLog.text = $"Invalid value: '{newValue}'";
            return;
        }
        var indu = FindAnyObjectByType<QuizManager>();
        var syno = FindAnyObjectByType<SynonymQuizManager>();
        var anto = FindAnyObjectByType<AntonymQuizManager>();


        if(indu != null)
        {
            indu.flashAnswerDuration = setValue;
            displayLog.text = "Hint display duration updated";
            return;
        }
        if(syno != null)
        {
            syno.flashAnswerDuration = setValue;
            displayLog.text = "Hint display duration updated";
            return;
        }
        if(anto != null)
        {
            anto.flashAnswerDuration = setValue;
            displayLog.text = "Hint display duration updated";
            return;
        }  
        displayLog.text = "Couldn't find any game session";
    }
    public void HintTransitionSpeed(TMP_InputField newVal)
    {
        string newValue = newVal.text;

        if (!float.TryParse(newValue, out float setValue))
        {
            displayLog.text = $"Invalid value: '{newValue}'";
            return;
        }
        var indu = FindAnyObjectByType<QuizManager>();
        var syno = FindAnyObjectByType<SynonymQuizManager>();
        var anto = FindAnyObjectByType<AntonymQuizManager>();


        if(indu != null)
        {
            indu.artificialDelay = setValue;
            displayLog.text = "Hint switching delay updated";
            return;
        }
        if(syno != null)
        {
            syno.artificialDelay = setValue;
            displayLog.text = "Hint switching delay updated";
            return;
        }
        if(anto != null)
        {
            anto.artificialDelay = setValue;
            displayLog.text = "Hint switching delay updated";
            return;
        }  
        displayLog.text = "Couldn't find any game session";
    }

    public void AnswersAnimationSpeed(TMP_InputField newVal)
    {
        string newValue = newVal.text;

        if (!float.TryParse(newValue, out float setValue))
        {
            displayLog.text = $"Invalid value: '{newValue}'";
            return;
        }
        var indu = FindAnyObjectByType<QuizManager>();
        var syno = FindAnyObjectByType<SynonymQuizManager>();
        var anto = FindAnyObjectByType<AntonymQuizManager>();


        if(indu != null)
        {
            indu.flashcardMoveDuration = setValue;
            displayLog.text = "Animation transition speed updated";
            return;
        }
        if(syno != null)
        {
            syno.flashcardMoveDuration = setValue;
            displayLog.text = "Animation transition speed updated";
            return;
        }
        if(anto != null)
        {
            anto.flashcardMoveDuration = setValue;
            displayLog.text = "Animation transition speed updated";
            return;
        }  
        displayLog.text = "Couldn't find any game session";
    }

    public void CorrectDisplayDuration(TMP_InputField newVal)
    {
        string newValue = newVal.text;

        if (!float.TryParse(newValue, out float setValue))
        {
            displayLog.text = $"Invalid value: '{newValue}'";
            return;
        }
        var indu = FindAnyObjectByType<QuizManager>();
        var syno = FindAnyObjectByType<SynonymQuizManager>();
        var anto = FindAnyObjectByType<AntonymQuizManager>();


        if(indu != null)
        {
            indu.correctDuration = setValue;
            displayLog.text = "Correct display duration updated";
            return;
        }
        if(syno != null)
        {
            syno.correctDuration = setValue;
            displayLog.text = "Correct display duration updated";
            return;
        }
        if(anto != null)
        {
            anto.correctDuration = setValue;
            displayLog.text = "Correct display duration updated";
            return;
        }  
        displayLog.text = "Couldn't find any game session";
    }
    public void WrongDisplayDuration(TMP_InputField newVal)
    {
        string newValue = newVal.text;

        if (!float.TryParse(newValue, out float setValue))
        {
            displayLog.text = $"Invalid value: '{newValue}'";
            return;
        }
        var indu = FindAnyObjectByType<QuizManager>();
        var syno = FindAnyObjectByType<SynonymQuizManager>();
        var anto = FindAnyObjectByType<AntonymQuizManager>();


        if(indu != null)
        {
            indu.wrongDuration = setValue;
            displayLog.text = "Incorrect display duration updated";
            return;
        }
        if(syno != null)
        {
            syno.wrongDuration = setValue;
            displayLog.text = "Incorrect display duration updated";
            return;
        }
        if(anto != null)
        {
            anto.wrongDuration = setValue;
            displayLog.text = "Incorrect display duration updated";
            return;
        }  
        displayLog.text = "Couldn't find any game session";
    }


    // General
    public void OverrideLoadinDuration(TMP_InputField newVal)
    {
        string newValue = newVal.text;

        if (!float.TryParse(newValue, out float setValue))
        {
            displayLog.text = $"Invalid value: '{newValue}'";
            return;
        }

        PlayerPrefs.SetFloat("LoadingDuration", setValue);
        PlayerPrefs.Save();
        var varObj = FindAnyObjectByType<LoadingScreen>();

        if(varObj == null)
        {
            displayLog.text = "You can't modify loading duration right now.";
            return;
        }
        
        LoadingScreen.overrideLoadingDuration = setValue;
        displayLog.text = "Updated loading duration.";
    }

    public void ModifyBrushSize(TMP_InputField newVal)
    {
        string newValue = newVal.text;

        if (!float.TryParse(newValue, out float setValue))
        {
            displayLog.text = $"Invalid value: '{newValue}'";
            return;
        }
        var varObj = FindAnyObjectByType<CharacterTracingController>();

        if(varObj == null)
        {
            displayLog.text = "Open the tracing character page first.";
            return;
        }
        if(setValue > 10)
        {
            displayLog.text = "Value cannot exceed 10";
            return;
        }
        
        varObj.ChangeBrushSize(setValue);
        displayLog.text = "Brush Size Updated";
    }

    public void ModifyGrayTolerance(TMP_InputField newVal)
    {
        string newValue = newVal.text;

        if (!float.TryParse(newValue, out float setValue))
        {
            displayLog.text = $"Invalid value: '{newValue}'";
            return;
        }
        var varObj = FindAnyObjectByType<CharacterTracingController>();

        if(varObj == null)
        {
            displayLog.text = "Open the tracing character page first.";
            return;
        }
        if(setValue > 1f)
        {
            displayLog.text = "Value cannot exceed 1";
            return;
        }
        
        varObj.grayTolerance = setValue;
        displayLog.text = "Gray Tolerance Updated";
    }

    public void ModifyCompThreshold(TMP_InputField newVal)
    {
        string newValue = newVal.text;

        if (!float.TryParse(newValue, out float setValue))
        {
            displayLog.text = $"Invalid value: '{newValue}'";
            return;
        }
        var varObj = FindAnyObjectByType<CharacterTracingController>();

        if(varObj == null)
        {
            displayLog.text = "Open the tracing character page first.";
            return;
        }
        if(setValue > 1f)
        {
            displayLog.text = "Value cannot exceed 1";
            return;
        }
        
        varObj.completionThreshold = setValue;
        displayLog.text = "Completion Threshold Updated";
    }


    public void TriggerFirstTimeUser()
    {
        PlayerPrefs.SetInt("IsFirstTime", 0);
        PlayerPrefs.SetInt("FirstTimeMultiplayer", 1);
        PlayerPrefs.Save();
        displayLog.text = "Introduction scene will load next page";
    }

    public void TogglePlayerProfileStats()
    {
        var varObj = FindAnyObjectByType<UserStatsDisplay>();

        if (varObj == null)
        {
            displayLog.text = "Player profile stats not available.";
            return;
        }
        displayLog.text = "";
        varObj.statsPanel.SetActive(!varObj.statsPanel.activeSelf);
    }


    public void ToggleHiddenStuffTemporary()
    {
        var varObj = FindAnyObjectByType<HiddenShits>();

        if (varObj == null)
        {
            displayLog.text = "No hidden stuffs in here";
            return;
        }
        displayLog.text = "Hidden stuffs are being displayed temporarily";
        varObj.UpdateUI(true);
    }

    public void ToggleHiddenStuffPerm()
    {
        var varObj = FindAnyObjectByType<HiddenShits>();

        if (varObj == null)
        {
            displayLog.text = "No hidden stuffs in here";
            return;
        }
        displayLog.text = "Hidden stuffs are being displayed temporarily";

        PlayerPrefs.SetInt("ShowHidden", PlayerPrefs.GetInt("ShowHidden", 0) == 0 ? 1 : 0);
        varObj.UpdateUI(PlayerPrefs.GetInt("ShowHidden", 0) == 1);
    }


    public void AddCash()
    {
        var varObj = FindAnyObjectByType<ProcessShopItems>();
        int currentCash = PlayerPrefs.GetInt("Money", 0);
        PlayerPrefs.SetInt("Money", currentCash + 100);
        PlayerPrefs.Save();

        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.BuySfx();
        }

        if (varObj != null)
        {
            displayLog.text = "Added 100 pesos LoL.";
            varObj.UpdateMoneyDisplay();
            return;
        }
        displayLog.text = "Added 100 pesos LoL.";
        varObj.UpdateMoneyDisplay();
    }

    public void SwitchBackground(int index)
    {
        var varObj = FindAnyObjectByType<CustomBackground>();

        if(varObj == null)
        {
            displayLog.text = "Custom Background not found.";
            return;
        }

        varObj.UpdateBackground(index);
        displayLog.text = "Switched background successfully. index number = " + index;
    }

    public void ClearShop()
    {

        var varObj = FindAnyObjectByType<ProcessShopItems>();
         var varObj1 = FindAnyObjectByType<CustomBackground>();

        PlayerPrefs.SetInt("Index1", 0);
        PlayerPrefs.SetInt("Index2", 0);
        PlayerPrefs.SetInt("Index3", 0);
        PlayerPrefs.SetInt("Index4", 0);
        PlayerPrefs.SetInt("Index5", 0);
        PlayerPrefs.SetInt("Index6", 0);
        PlayerPrefs.SetInt("Money", 0);
        PlayerPrefs.Save();

        if (varObj != null)
        {
            displayLog.text = "Shop items and cash been cleared.";
            varObj.UpdateMoneyDisplay();
        }
        if(varObj1 != null)
        {
            varObj1.UpdateBackground(0);
        }
    }

    // Multiplayer 

    public void AddMultiHint()
    {
        var varObj = FindAnyObjectByType<MultiGameManager>();

        if (varObj == null)
        {
            Debug.LogWarning("Multiplayer connection is not active.");
            displayLog.text = "Multiplayer connection is not active.";
            return;
        }
        displayLog.text = "Added 1 hint";
        varObj.sharedHintsRemaining++; 
    }

    public void AddMultiScore()
    {
        var varObj = FindAnyObjectByType<MultiGameManager>();

        if (varObj == null)
        {
            Debug.LogWarning("Multiplayer connection is not active.");
            displayLog.text = "Multiplayer connection is not active.";
            return;
        }
        displayLog.text = "Added 1 score";
        varObj.sharedScore++; 
    }

    public void AddMultiStreak()
    {
        var varObj = FindAnyObjectByType<MultiGameManager>();

        if (varObj == null)
        {
            Debug.LogWarning("Multiplayer connection is not active.");
            displayLog.text = "Multiplayer connection is not active.";
            return;
        }
        displayLog.text = "Added 1 streak";
        varObj.currentFirstChoiceStreak++; 
    }

    public void ToggleMultiTimer()
    {
        var varObj = FindAnyObjectByType<MultiGameManager>();

        if (varObj == null)
        {
            Debug.LogWarning("Multiplayer connection is not active.");
            displayLog.text = "Multiplayer connection is not active.";
            return;
        }
        displayLog.text = "Toggled Timer";
        varObj.isFrozen = !varObj.isFrozen; 
    }
    public void GetMultiAnswer()
    {
        var varObj = FindAnyObjectByType<MultiGameManager>();

        if (varObj == null)
        {
            Debug.LogWarning("Multiplayer connection is not active.");
            displayLog.text = "Multiplayer connection is not active.";
            return;
        }
        displayLog.text = "Answer: " + varObj.correctAnswerStr;
    }

    public void ToggleHiddenMultiBg()
    {
        var varObj = FindAnyObjectByType<MultiGameManager>();

        if (varObj == null)
        {
            Debug.LogWarning("Multiplayer connection is not active.");
            displayLog.text = "Multiplayer connection is not active.";
            return;
        }
        displayLog.text = "";
        varObj.bgActive = !varObj.bgActive; 
    }
}
