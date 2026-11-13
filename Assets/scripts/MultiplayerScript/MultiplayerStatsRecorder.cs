using Mirror;
using TMPro;
using UnityEngine;

public class MultiplayerStatsRecorder : MonoBehaviour
{
    public TMP_Text moneyDisplay;
    public void RecordGameData()
    {
        Debug.Log("MultiplayerStatsRecorder: Recording game data...");
        if (NetworkClient.localPlayer == null)
        {
            Debug.LogWarning(
                "MultiplayerStatsRecorder: Local player not found."
            );

            return;
        }
        
        NetworkPlayer player =
            NetworkClient.localPlayer
                .GetComponent<NetworkPlayer>();

        if (player == null)
        {
            Debug.LogWarning(
                "MultiplayerStatsRecorder: NetworkPlayer not found."
            );

            return;
        }

        MultiGameManager gameManager =
            FindAnyObjectByType<MultiGameManager>();

        if (gameManager == null)
        {
            Debug.LogError(
                "MultiplayerStatsRecorder: MultiGameManager not found."
            );

            return;
        }

        string gameMode = gameManager.resultGameMode;

        int difficultyLevel = gameManager.resultDifficulty;



        if (player.quizRound < 3)
        {
            return;
        }
            


        if (gameMode == "INDU")
        {
            RecordMode(
                "MIndu",
                difficultyLevel,
                player
            );
        }

        else if (gameMode == "SYNONYM")
        {
            RecordMode(
                "MSyno",
                difficultyLevel,
                player
            );
        }

        else if (gameMode == "ANTONYM")
        {
            RecordMode(
                "MAnto",
                difficultyLevel,
                player
            );
        }

        PlayerPrefs.Save();
        var varObj = FindAnyObjectByType<MultiGameManager>();
        Debug.Log(
            $"MULTIPLAYER STATS SAVED | " +
            $"Player: {player.playerName} | " +
            $"Score: {player.score} | " +
            $"Mistakes: {player.totalMistakes} | " +
            $"Highest Streak: {varObj.highestConsecutiveFirstChoice} | " + 
            $"Game Mode: {gameMode} | " +
            $"Difficulty Level: {difficultyLevel}"
        );
    }


    private void RecordMode(
        string prefix,
        int difficultyLevel,
        NetworkPlayer player)
    {

        
        var varObj = FindAnyObjectByType<MultiGameManager>();
        // =========================================
        // HIGHEST ROUND
        // =========================================


        SaveMoney(varObj.sharedScore, varObj.highestConsecutiveFirstChoice, varObj.totalMistakes);


        UpdateHighestRecord(
            prefix + "HR",
            player.quizRound
        );

        // =========================================
        // HIGHEST SCORE
        // =========================================

        UpdateHighestRecord(
            prefix + "HScore",
            varObj.sharedScore
        );

        // =========================================
        // HIGHEST STREAK
        // =========================================

        UpdateHighestRecord(
            prefix + "HS",
            varObj.highestConsecutiveFirstChoice
        );


        if (varObj.timeRemaining == -1)
        {
            Debug.Log($"Endless time record : {varObj.elapsedGameTime}");
            UpdateHighestRecord(
                prefix + "LongRun",
                varObj.elapsedGameTime
            );
        }

        // =========================================
        // GAME COUNT BY DIFFICULTY
        // =========================================

        if(prefix == "MIndu")
        {
            switch (difficultyLevel)
            {
                case 0:
                    IncrementData(prefix + "Easy");
                    break;

                case 1:
                    IncrementData(prefix + "Hard");
                    break;

                // case 2:
                //     IncrementData(prefix + "Hard");
                //     break;
            }            
        }
        else
        {
            switch (difficultyLevel)
            {
                case 0:
                    IncrementData(prefix + "Easy");
                    break;

                case 1:
                    IncrementData(prefix + "Medium");
                    break;

                case 2:
                    IncrementData(prefix + "Hard");
                    break;
            }       
        }
    }


    private void IncrementData(string key)
    {
        int currentValue =
            PlayerPrefs.GetInt(key, 0);

        PlayerPrefs.SetInt(
            key,
            currentValue + 1
        );
    }


    private void UpdateHighestRecord(
        string key,
        int newValue)
    {
        int currentValue =
            PlayerPrefs.GetInt(key, 0);

        if (newValue > currentValue)
        {
            PlayerPrefs.SetInt(
                key,
                newValue
            );
        }
    }

    private void SaveMoney(int score, int streak, int wrong)
    {
        int totalValue;
        int currentCash = PlayerPrefs.GetInt("Money", 0);

        totalValue = (int)((score * 0.3) - (wrong * 10) + (streak * 6)); 

        if(totalValue <= 0)
        {
            totalValue = 0;
        }

        if(moneyDisplay != null)
        moneyDisplay.text = totalValue.ToString();
        
        PlayerPrefs.SetInt("Money", currentCash + totalValue);
        PlayerPrefs.Save();
    }
}