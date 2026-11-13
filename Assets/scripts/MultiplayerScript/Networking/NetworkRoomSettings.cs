using Mirror;
using UnityEngine;

public class NetworkRoomSettings : NetworkBehaviour
{
    public static NetworkRoomSettings Instance;

    [Header("Room Settings")]

    [SyncVar(hook = nameof(OnTimeDurationChanged))]
    public int timeDuration;

    [SyncVar]
    public int difficultyLevel;

    [SyncVar]
    public string destinationScene;

    [SyncVar]
    public string gameMode;


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


    public override void OnStartServer()
    {
        base.OnStartServer();

        LoadHostSettings();
    }


    [Server]
    void LoadHostSettings()
    {
        timeDuration =
            PlayerPrefs.GetInt("MTimeDuration", 0);

        difficultyLevel =
            PlayerPrefs.GetInt("MDifficultyLevel", 0);

        destinationScene =
            PlayerPrefs.GetString(
                "MDestinationScene",
                "Multi_Indu"
            );

        gameMode =
            PlayerPrefs.GetString(
                "MGameMode",
                "INDU"
            );

    }


    // ==========================================
    // Helper Functions
    // ==========================================

    public string GetTimeMode()
    {
        switch (timeDuration)
        {
            case 0:
                return "SLOW";

            case 1:
                return "FAST";

            case 2:
                return "UNTIMED";

            default:
                return "UNKNOWN";
        }
    }


    public string GetDifficulty()
    {
        // Script / Indu:
        // 0 = Easy
        // 1 = Medium-Hard

        if (gameMode == "INDU")
        {
            switch (difficultyLevel)
            {
                case 0:
                    return "EASY";

                case 1:
                    return "HARD";

                default:
                    return "UNKNOWN";
            }
        }


        // Synonym and Antonym still use
        // Easy / Medium / Hard

        switch (difficultyLevel)
        {
            case 0:
                return "EASY";

            case 1:
                return "MEDIUM";

            case 2:
                return "HARD";

            default:
                return "UNKNOWN";
        }
    }
    private void OnTimeDurationChanged(int oldValue, int newValue)
    {
        Debug.Log(
            "[CLIENT TIME MODE SYNC] " +
            oldValue + " -> " + newValue
        );

        if (MultiTimer.Instance != null)
        {
            if(MultiGameManager.Instance != null)
            {
                bool paused = MultiGameManager.Instance.timerPaused;
                if(!paused)
                    MultiTimer.Instance.UpdateTimerModeUI();
            }
            
        }
    }

    public string GetGameMode()
    {
        return gameMode;
    }
}