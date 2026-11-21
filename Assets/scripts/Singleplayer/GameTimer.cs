using System.Collections;
using System.Transactions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameTimer : MonoBehaviour
{
    [Header("Timer Colors")]
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    public Image clockImageAnimated;
    public Image clockImageStatic;
    public Animator clockAnimator;
    public Image timerShake;
    public float animationDuration = 2f;
    public TMP_Text digitalClock;
    public static GameTimer Instance;
    public GameObject times_up_textholder;
    public Image timer_linear_image;
    // public Image timer_radial_image;    // For radial type timer
    public float time_remaining;
    public float max_time = 60f;
    public bool timeUp = false;
    public bool isFrozen = false;
    public GameObject timerBar, scoreBar, pauseButton, pausePanel;   // To disable when time's up
    //public GameObject timer_radial_textholder;   //idk
    public Image titleImage;
    public bool isPaused = false;
    bool isTimeClosing = false;
    bool animationPlayed = false;
    int timeData = 0;
    bool isGuessing = false;
    public int elapsedTime = 0;
    bool isTinted = false;
    public TMP_Text moneyReceived;
    public bool isNewScore = false;
    public bool isNewStreak = false;
    public bool isNewRound = false;
    public GameObject highScoreLabel;
    public GameObject highStreakLabel;
    public GameObject highRoundLabel;
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
        elapsedTime = 0;
        clockAnimator.enabled = isTimeClosing;
        timeData = PlayerPrefs.GetInt("TimeDuration", 0);
        switch (timeData)
        {
            case 0:
                max_time = 120f;    // Slow || Long playtime
                break;
            case 1:
                max_time = 60f;     // Fast || Short playtime
                break;
            case 2:
                max_time = 999999f; // Inf ||  Self exp
                HideTimer();
                break;
        }
        time_remaining = max_time;
        digitalClock.text = ConvertTime(Mathf.CeilToInt(time_remaining));
        StartCoroutine(ElapsedTimeCounter());
    }
    // Update is called once per frame
    void Update()
    {
        if (isPaused || timeUp || isFrozen)
        {
            return; // Skip updating the timer if the game is paused or time is up
        }
        if (time_remaining > 0)
        {
            CheckTimeRemaining();
            time_remaining -= Time.deltaTime;
            timer_linear_image.fillAmount = time_remaining / max_time;
            digitalClock.text = ConvertTime(Mathf.CeilToInt(time_remaining));
            if (time_remaining <= 20f)
            {
                float t = 1f - (time_remaining / 20f);
                timer_linear_image.color = Color.Lerp(normalColor, warningColor, t);
            }
            else
            {
                timer_linear_image.color = normalColor;
}
        }
        else
        {
            if (!timeUp) // run only once
            {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();
        SaveMoney();
        if (guessBeh != null) 
        {
                if (!LoadingScreen.isFromTutorial)
                    {
                        RecordGameData();
                        guessBeh.EvaluateResults();
                        guessBeh.isTimerRunning = false;
                        isGuessing = guessBeh.isGuessing;       
                        if(!isGuessing) ShowStats();
                    }
                    else
                    {
                        LoadingScreen.isFromTutorial = false;
                    }
        }
                AddTotalPlayed();
                timeUp = true;
                AudioManager.Instance.TimesUpSfx();
                times_up_textholder.SetActive(!isGuessing);
                scoreBar.SetActive(false);
                timerBar.SetActive(false);
                timerShake.enabled = false;
                pauseButton.SetActive(false);
                pausePanel.SetActive(false);
                titleImage.enabled = false;
                // AnimateClockImage();
                titleImage.transform.position -= new Vector3(0, 250f, 0); // Move title up to make room for "Time's Up" text
                titleImage.transform.localScale -= new Vector3(-3f, -3f, 0); // Enlarge title for emphasis
                if (QuizManager.Instance != null)
                {
                    QuizManager.Instance.HideAllButtons();
                }
                if (SynonymQuizManager.Instance != null)
                {
                    SynonymQuizManager.Instance.HideAllButtons();
                }
                if (AntonymQuizManager.Instance != null)
                {
                    AntonymQuizManager.Instance.HideAllButtons();
                }
                if(timeData == 2)
                    UpdateResultTime();
            }
        }
    }
    public void ShowStats()
    {
        var perf = FindAnyObjectByType<PerfStatDis>();
        if(perf != null)
        {
            perf.DisplayStatsSingle();
        } 
    }
    public void CheckHighest()
    {
        StartCoroutine(DisplayHighestLabel());
    }
    IEnumerator DisplayHighestLabel()
    {
        yield return new WaitForSeconds(0.2f);
        if (isNewRound && highRoundLabel != null)
        {
            highRoundLabel.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            AudioManager.Instance.SlamSfx3();
            yield return new WaitForSeconds(1f);
        }
        if (isNewScore && highScoreLabel != null)
        {
            highScoreLabel.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            AudioManager.Instance.SlamSfx3();
            yield return new WaitForSeconds(1f);
        }
        if (isNewStreak && highStreakLabel != null)
        {
            highStreakLabel.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            AudioManager.Instance.SlamSfx3();       
        }

    }
    IEnumerator ElapsedTimeCounter()
    {
        while (!timeUp)
        {
            yield return new WaitForSeconds(1f);
            elapsedTime++;
        }
    }
    void UpdateResultTime()
    {
        var indu = FindAnyObjectByType<QuizManager>();
        var syno = FindAnyObjectByType<SynonymQuizManager>();
        var anto = FindAnyObjectByType<AntonymQuizManager>();
        if(indu != null)
            indu.PrepareResults();
        if(syno != null)
            syno.PrepareResults();
        if(anto != null)
            anto.PrepareResults();
    }
    public void AddTotalPlayed()
    {
        int gPlayed = PlayerPrefs.GetInt("TotalPlayed", 0);
        PlayerPrefs.SetInt("TotalPlayed", gPlayed + 1);
        PlayerPrefs.Save();
    }
    public void PauseGame()
    {
        isPaused = true; 
    }
    public void ResumeGame()
    {
        isPaused = false;
    }
    void HideTimer()
    {
        int destinationScene = PlayerPrefs.GetInt("DestinationScene", 0);
        if (destinationScene == 1)
        {
            timerBar.SetActive(false);
            timerShake.enabled = false;
            scoreBar.SetActive(true);
        }
        else if (destinationScene == 2 || destinationScene == 3)
        {
            timerBar.SetActive(false);
            timerShake.enabled = false;
            scoreBar.SetActive(true);
        }
    }
    public void HideElements(){
        scoreBar.SetActive(false);
        timerBar.SetActive(false);
        timerShake.enabled = false;
        titleImage.enabled = false;
        pauseButton.SetActive(false);
        if(timeData == 2)
        {
            scoreBar.SetActive(false);
            timerShake.enabled = false;
        }
    }
    public void ShowElements(){
        timerBar.SetActive(true);
        timerShake.enabled = true;
        titleImage.enabled = true;
        pauseButton.SetActive(true);
        if (timeData == 2)
        {
            timerBar.SetActive(!timerBar.activeSelf);
            scoreBar.SetActive(true);
            timerShake.enabled = false;
        }
    }
    public void StopGame()
    {
        time_remaining = 0;
        AudioManager.Instance.StopTimesUpSfx();
    }
    void CheckTimeRemaining()
    {
        if(max_time ==  60f && time_remaining <= 10f && animationPlayed == false)
        {
            TimeClosing();
        }
        else if(max_time == 30f && time_remaining <= 10f && animationPlayed == false)
        {
            TimeClosing();
        }
    }
    void TimeClosing()
    {
        isTimeClosing = true;
        animationPlayed = true;
        AudioManager.Instance.TickingSfx();
        StartCoroutine(TimerShake());
        StartCoroutine(ClockFlicker());
    }
    public void AnimateClockImage()
    {
        StartCoroutine(AnimateClockImageCoroutine());
    }
    IEnumerator AnimateClockImageCoroutine()
    {
        clockImageAnimated.gameObject.SetActive(true);
        clockImageStatic.gameObject.SetActive(false);

        // Wait for the animation to finish (assuming the animation is 1 second long)
        yield return new WaitForSeconds(animationDuration);

        clockImageAnimated.gameObject.SetActive(false);
        clockImageStatic.gameObject.SetActive(true);
    }
    IEnumerator TimerShake()
    {
        clockAnimator.enabled = isTimeClosing;
        yield return new WaitForSeconds(10f);
        clockAnimator.enabled = isTimeClosing;
        
    }
    IEnumerator ClockFlicker()
    {
        while(time_remaining < 10f && timeData != 2)
        {
            isTinted = !isTinted;
            yield return new WaitForSeconds(0.5f);
            if(isTinted)
                digitalClock.color = Color.red;
            else
                digitalClock.color = Color.white;
        }
    }
    void RecordGameData()
    {
        var guessBeh = FindAnyObjectByType<GuessingBehavior>();
        if(guessBeh == null)
            return;
        int gameMode = PlayerPrefs.GetInt("DestinationScene", 0);
        int difficultyLevel = PlayerPrefs.GetInt("DifficultyLevel", 0);
        if(guessBeh.currentRound >= 3)
        {
            if(gameMode == 1)       //Reading
            {
                var readingMode = FindAnyObjectByType<QuizManager>();
                if (readingMode == null)
                    return;
                UpdateHighestRecord("InduHR", guessBeh.currentRound, "round");
                UpdateHighestRecord("InduHScore", readingMode.score, "score");
                UpdateHighestRecord("InduHS", readingMode.highestStreak, "streak"); 
                Debug.Log(
                    $"LONG RUN RECORD | timeData: {timeData} | elapsedTime: {elapsedTime}"
                );
                if(timeData == 2)
                {
                    UpdateHighestRecord("InduLongRun", elapsedTime, "");
                }
                switch (difficultyLevel)
                {
                    case 0:     //Easy
                            IncrementData("InduEasy");
                        break;
                    case 1:     //Medium
                            IncrementData("InduMedium");
                        break;
                    case 2:     //Hard
                            IncrementData("InduHard");
                        break;
                }
            }
            if(gameMode == 2)       //Synonyms
            {
                var modeScript = FindAnyObjectByType<SynonymQuizManager>();
                if (modeScript == null)
                    return;
                UpdateHighestRecord("SynoHR", guessBeh.currentRound, "round");
                UpdateHighestRecord("SynoHScore", modeScript.score, "score");
                UpdateHighestRecord("SynoHS", modeScript.highestStreak, "streak");
                if(timeData == 2)
                {
                    UpdateHighestRecord("SynoLongRun", elapsedTime, "");
                }
                switch (difficultyLevel)
                {
                    case 0:     //Easy
                            IncrementData("SynoEasy");
                        break;
                    case 1:     //Medium
                            IncrementData("SynoMedium");
                        break;
                    case 2:     //Hard
                            IncrementData("SynoHard");
                        break;
                }
            }
            if(gameMode == 3)       //Antonyms
            {
                var modeScript = FindAnyObjectByType<AntonymQuizManager>();
                if (modeScript == null)
                    return;

                UpdateHighestRecord("AntoHR", guessBeh.currentRound, "round");
                UpdateHighestRecord("AntoHScore", modeScript.score, "score");
                UpdateHighestRecord("AntoHS", modeScript.highestStreak, "streak");
                if(timeData == 2)
                {
                    UpdateHighestRecord("AntoLongRun", elapsedTime, "");
                }
                switch (difficultyLevel)
                {
                    case 0:     //Easy
                            IncrementData("AntoEasy");
                        break;
                    case 1:     //Medium
                            IncrementData("AntoMedium");
                        break;
                    case 2:     //Hard
                            IncrementData("AntoHard");
                        break;
                }
            }
            PlayerPrefs.Save();
        }
    }
    void IncrementData(string key)
    {
        int currentVal = PlayerPrefs.GetInt(key, 0);
        PlayerPrefs.SetInt(key, currentVal + 1);
    }
    void UpdateHighestRecord(string key, int newValue, string targetState)
    {
        int currentVal = PlayerPrefs.GetInt(key, 0);
        if(newValue > currentVal)
        {
            PlayerPrefs.SetInt(key, newValue);
            if (targetState == "score") isNewScore = true;
            if(targetState == "streak") isNewStreak = true;
            if(targetState == "round") isNewRound = true;
        }
    }
    string ConvertTime(int timeVal)
    {
        int minutes = timeVal / 60;
        int seconds = timeVal % 60;
        string formattedTime = string.Format("{0:D2}:{1:D2}", minutes, seconds);
        return formattedTime;
    }
    void SaveMoney()
    {
        var indu = FindAnyObjectByType<QuizManager>();
        var syno = FindAnyObjectByType<SynonymQuizManager>();
        var anto = FindAnyObjectByType<AntonymQuizManager>();
        int score = 0;
        int streak = 0;
        int wrong = 0;
        int totalValue = 0;
        if(indu != null)
        {
            score = indu.score;
            streak = indu.highestStreak;
            wrong = indu.wrongCount;
        }
        if(syno != null)
        {
            score = syno.score;
            streak = syno.highestStreak;
            wrong = syno.wrongCount;
        }
        if(anto != null)
        {
            score = anto.score;
            streak = anto.highestStreak;
            wrong = anto.wrongCount;
        }
        totalValue = (int)((score * 0.3) - (wrong * 10) + (streak * 4));
        int currentCash = PlayerPrefs.GetInt("Money", 0);
        if(totalValue <= 0)
        {
            totalValue = 0;
        } 
        PlayerPrefs.SetInt("Money", totalValue + currentCash);
        PlayerPrefs.Save();
        if(moneyReceived != null)
            moneyReceived.text = totalValue.ToString("N0");
    }
}