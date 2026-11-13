using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

public class GuessingBehavior : MonoBehaviour
{
    [Header("Display UI")]
    public GameObject statsPanel;
    public GameObject warningPanel;
    public GameObject gameOverPanel;

    public TMP_Text stamp1;
    public TMP_Text stamp2;
    public TMP_Text stamp3;
    public TMP_Text timerDisplay;
    public TMP_Text attemptsDisplay;
    public TMP_Text hintDisplay;

    public TMP_Text roundDisplay;
    public TMP_Text scoreDisplay;
    public TMP_Text lowProbDisplay;
    public TMP_Text medProbDisplay;
    public TMP_Text highProbDisplay;
    public TMP_Text timeBasisDisplay;
    public TMP_Text transitionRoundDisplay;
    public TMP_Text transitionHintDisplay; 
    public TMP_Text highestProbDisplay;

    public TMP_Text averageSpeedDisplay;
    public TMP_Text baseSpeedDisplay;
    public TMP_Text roundReqDisplay;
    public TMP_Text ignoreTimeDisplay;
    public TMP_Text isAdaptiveDisplay;


    public TMP_Text hasMinReqDisplay;           //=======================||||||||||||||||
    public TMP_Text gamesPlayedDisplay;         //=======================||||||||||||||||
    public TMP_Text validPlayedDisplay;         //=======================||||||||||||||||
    public TMP_Text requiredDisplay;
    public TMP_Text tendencyDisplay;


    [Header("Game Timer")]
    public float timer = 0f;
    public bool isTimerRunning = false;
    

    public float timeStamp1 = 0f;
    public float timeStamp2 = 0f;
    public float timeStamp3 = 0f;


    [Header("Game Activity")]
    public int currentRound = 0;
    public List<float> averageStampSpeed = new List<float>();        
    public float baseAverageSpeed = 2.36f;          // <<< feed the data here
    public bool hintUsed = false;
    public int attempts = 0;
    public int totalPlayed = 0;             //=======================||||||||||||||||
    public int validPlayed = 0;             //=======================||||||||||||||||
    public float guessingTendency = 0f;


    [Header("Behavior Settings")]
    public float timeBasis = 2.36f;                // Overall time t'ill correct
    public float transitionRoundDelay = 1.6f;     
    public float transitionHintDelay = 3.6f;
    public bool displayPanel = false;
    public int roundReq = 8;
    public float maxIgnoreTime = 30f;
    public bool isAdaptive = false;
    public bool hasRequiredPlayAmount = false;      //=======================||||||||||||||||
    public int requiredAmount = 8;


    [Header("Behavior Stats")]
    public int scoreBehavior = 0;
    public int lowProbability = 0;
    public int mediumProbability = 0;
    public int highProbability = 0;

    [Header("Final Judgement")]
    public bool isGuessing = false;

    void Start()
    {

        hasRequiredPlayAmount = PlayerPrefs.GetInt("HasRequire", 0) == 1;
        guessingTendency = PlayerPrefs.GetFloat("GuessingTendency", 0f);
        validPlayed = PlayerPrefs.GetInt("ValidPlayed", 0);
        totalPlayed = PlayerPrefs.GetInt("TotalPlayed", 0);

        if(PlayerPrefs.GetInt("AdaptiveResponse", 0) == 1)
        {
            isAdaptive = true;
            baseAverageSpeed = PlayerPrefs.GetFloat("AverageSpeed", timeBasis);     
            averageStampSpeed.Add(baseAverageSpeed);   
            timeBasis = baseAverageSpeed;            
        }


        isGuessing = false;


        warningPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        displayPanel = false;
        // isTimerRunning = true;
        timer = 0f;
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
        }
        UpdateDisplayUI();
    }

    void UpdateDisplayUI()
    {
        if(displayPanel == false)
        {
            if(statsPanel != null)
                statsPanel.SetActive(false);
            return;
        }

        if(statsPanel != null)
            statsPanel.SetActive(displayPanel);
        if(timerDisplay != null)
            timerDisplay.text = "Time: " + timer.ToString("F2") + "s";
        if(attemptsDisplay != null)
            attemptsDisplay.text = "Attempts: " + attempts.ToString();
        if(hintDisplay != null)
            hintDisplay.text = "Hint Used:<color=\"lightblue\"> " + hintUsed.ToString();
        if(stamp1 != null)
            stamp1.text = "Stamp 1:<color=\"yellow\"> " + timeStamp1.ToString("F2") + "s";
        if(stamp2 != null)
            stamp2.text = "Stamp 2:<color=\"yellow\"> " + timeStamp2.ToString("F2") + "s";
        if(stamp3 != null)
            stamp3.text = "Stamp 3:<color=\"yellow\"> " + timeStamp3.ToString("F2") + "s";


        if(roundDisplay != null)
            roundDisplay.text = "Round:<color=\"yellow\"> " + currentRound.ToString();
        if(scoreDisplay != null)
            scoreDisplay.text = "Score:<color=\"yellow\"> " + scoreBehavior.ToString();
        if(lowProbDisplay != null)
            lowProbDisplay.text = "Low Probability:<color=\"yellow\"> " + lowProbability.ToString();
        if(medProbDisplay != null)
            medProbDisplay.text = "Medium Probability:<color=\"yellow\"> " + mediumProbability.ToString();
        if(highProbDisplay != null)
            highProbDisplay.text = "High Probability:<color=\"yellow\"> " + highProbability.ToString();
        if(timeBasisDisplay != null)
            timeBasisDisplay.text = "Max Time Basis:<color=\"yellow\"> " + timeBasis.ToString("F2") + "s";
        if(transitionRoundDisplay != null)
            transitionRoundDisplay.text = "Round Transition Delay:<color=\"yellow\"> " + transitionRoundDelay.ToString("F2") + "s";
        if(transitionHintDisplay != null)
            transitionHintDisplay.text = "Hint Transition Delay:<color=\"yellow\"> " + transitionHintDelay.ToString("F2") + "s";

        if(averageSpeedDisplay != null)
            averageSpeedDisplay.text = "Average response:<color=\"yellow\"> " + (averageStampSpeed.Sum() / averageStampSpeed.Count).ToString("F2") + "s";
        if(baseSpeedDisplay != null)
            baseSpeedDisplay.text = "Base response spd:<color=\"yellow\"> " + baseAverageSpeed.ToString("F2") + "s";
        if(roundReqDisplay != null)
            roundReqDisplay.text = "Required round for update:<color=\"yellow\"> " + roundReq.ToString();
        if(ignoreTimeDisplay != null)
            ignoreTimeDisplay.text = "Ignore time:<color=\"yellow\"> " + maxIgnoreTime.ToString("F2") + "s";
        if(isAdaptiveDisplay != null)
            isAdaptiveDisplay.text = "Response speed adaptation: <color=\"yellow\">" + isAdaptive.ToString();

        if(hasMinReqDisplay != null)
            hasMinReqDisplay.text = "Has Min required round: <color=\"yellow\">" + hasRequiredPlayAmount.ToString();
        if(gamesPlayedDisplay != null)
            gamesPlayedDisplay.text = "Total games played: <color=\"yellow\">" + totalPlayed.ToString();   
        if(validPlayedDisplay != null)
            validPlayedDisplay.text = "Total valid games played: <color=\"yellow\">" + validPlayed.ToString();
        if(requiredDisplay != null)
            requiredDisplay.text = "Min required to trigger detection: <color=\"yellow\">" + requiredAmount.ToString();
        if(tendencyDisplay != null)
            tendencyDisplay.text = "Current Guessing Tendency: <color=\"yellow\">" + guessingTendency.ToString("F2") + "%";
    }

    public void NewRound()
    {
        isTimerRunning = false;
        scoreBehavior = 0;
        timer = 0f;
        attempts = 0;
        hintUsed = false;
        timeStamp1 = 0f;
        timeStamp2 = 0f;
        timeStamp3 = 0f;
    }

    public void RecordStamp()     
    {
        // if (hintUsed)
        // {
        //     timer += transitionHintDelay;
        //     hintUsed = false;
        // }

        if(attempts == 1)
        {
            timeStamp1 = timer;
        }
        else if(attempts == 2)
        {
            timeStamp2 = timer;
        }
        else if(attempts == 3)
        {
            timeStamp3 = timer;
        }
        // timer += transitionRoundDelay;
    }


    public void CheckAnswer(bool isCorrect)
    {
        attempts++;
        RecordStamp();

        if (isCorrect)
        {
            isTimerRunning = false;
            if(timer < timeBasis)
            {
                scoreBehavior++;
            }

            if(attempts == 3)
            {
                scoreBehavior++;
            }

            ProcessStats();          
        }
        else
        {   
            if(attempts == 1)
            {
                scoreBehavior++;
            }
            if(!hintUsed && attempts == 1)
            {
                scoreBehavior++;
            }
           
        }

    }

    public void ProcessStats()
    {
        CalculateResponseTime();

        if(scoreBehavior <= 1)
        {
            lowProbability++;
        }
        else if(scoreBehavior == 2)
        {
            mediumProbability++;
        }
        else if(scoreBehavior >= 3)
        {
            highProbability++;
        }
    }

    public void CalculateResponseTime()
    {
        currentRound++;

        if(timer <= maxIgnoreTime)
        {
            averageStampSpeed.Add(timer);            
        }

    }

    public void EvaluateResults()
    {
        CalculateGuessingTendency();    // Trigger no matter what

        if(currentRound < roundReq)
        {
             return;
        }   
        if(averageStampSpeed.Count >= roundReq)
        {
            baseAverageSpeed = averageStampSpeed.Sum() / averageStampSpeed.Count; 
            if(isAdaptive)
            {
                PlayerPrefs.SetFloat("AverageSpeed", baseAverageSpeed); 
                PlayerPrefs.Save();
            }
                AddValidPlayed();
        }

        if (hasRequiredPlayAmount)
        {
            if(validPlayed <= requiredAmount)
            {
                highestProbDisplay.text = "Threshold not reached yet";
                return;
            }
        }

        if(highProbability + mediumProbability + lowProbability == 0)   // Don't evaluate if no recorded data || 
        {
            isGuessing = false;
            return;
        }

        if (highProbability >= mediumProbability &&     // If high probability is greater than or equal to medium and low, then it's HIGH
            highProbability >= lowProbability)
        {
            // HIGH

            if(highestProbDisplay != null)
                highestProbDisplay.text = "Probability Level: <color=\"green\"> HIGH";


            isGuessing = true;          // Show the result
            warningPanel.SetActive(true);
            gameOverPanel.SetActive(false);


        }
        else if (mediumProbability >= lowProbability)   // If medium probability is greater than or equal to low, then it's MEDIUM
        {
            // MEDIUM
            if(highestProbDisplay != null)
                highestProbDisplay.text = "Probability Level: <color=\"orange\"> MEDIUM";
        }
        else
        {
            // LOW
            if(highestProbDisplay != null)
                highestProbDisplay.text = "Probability Level: <color=\"white\"> LOW";
        }
    }
    public void AddValidPlayed()
    {
        validPlayed++;
        PlayerPrefs.SetInt("ValidPlayed", validPlayed);
        PlayerPrefs.Save();
    }

    public void CalculateGuessingTendency()
    {
        int totalRound = PlayerPrefs.GetInt("TotalClassifiedRounds", 0);
        int totalHigh = PlayerPrefs.GetInt("TotalHighCounts", 0);

        totalRound += currentRound;
        totalHigh += highProbability;

        if (totalRound > 0)
        {
            guessingTendency = (totalHigh / (float)totalRound) * 100f;
            PlayerPrefs.SetFloat("GuessingTendency", guessingTendency);      // To Display
        }

        PlayerPrefs.SetInt("TotalClassifiedRounds", totalRound);
        PlayerPrefs.SetInt("TotalHighCounts", totalHigh);
        PlayerPrefs.Save();
    }
    
}
