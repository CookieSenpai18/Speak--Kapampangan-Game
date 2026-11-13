using UnityEngine;
using Mirror;
using Mirror.Discovery;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetHUD : MonoBehaviour
{
    [SerializeField]
    private Button startGameButton;
    public Button returnToLobby;

    private void Update()
    {
        if (!NetworkServer.active)
            return;

        UpdateStartGameButton();
    }

    private void UpdateStartGameButton()
    {
        if (startGameButton == null)
            return;

        if (!NetworkServer.active)
            return;

        QuizNetworkManager manager =
            (QuizNetworkManager)NetworkManager.singleton;

        bool enoughPlayers =
            NetworkPlayer.Players.Count >= 2;

        startGameButton.interactable =
            enoughPlayers &&
            !manager.gameInProgress;
    }


    [System.Obsolete]
    public void HostGame()
    {
        if (NetworkManager.singleton != null)
        {
            // Debug.Log("Starting Host...");
            NetworkManager.singleton.StartHost();
            FindObjectOfType<QuizNetDiscovery>()
    .AdvertiseServer();
        }
        else
        {
            Debug.LogError("NetworkManager not found!");
        }
    }

    public void StartGame()
    {
        if (NetworkPlayer.Players.Count < 2)
        {
            return;
        }

        QuizNetworkManager manager =
            (QuizNetworkManager)NetworkManager.singleton;

        manager.gameInProgress = true;
        manager.gameStarted = true;

        // Immediately mark the room as unavailable.
        QuizNetDiscovery discovery =
            FindAnyObjectByType<QuizNetDiscovery>();

        if (discovery != null)
        {
            discovery.isGameStarted = true;

            Debug.Log("Discovery: Game starting, room is now unavailable.");
        }

        LoadingScreen.destinationScene = "Multi_Indu";
        LoadingScreen.isMultiplayer = true;

        NetworkManager.singleton.ServerChangeScene("LoadingScreen");
    }

    public void ChangeScene(string sceneName)
    {
        if (NetworkServer.active)
        {
            NetworkManager.singleton.ServerChangeScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Only the host can change scenes.");
        }
    }

    // [System.Obsolete]
    // public void BackButton()
    // {
    //     QuizNetDiscovery discovery =
    //         FindObjectOfType<QuizNetDiscovery>();

    //     if (NetworkManager.singleton != null)
    //     {
    //         // Host
    //         if (NetworkServer.active)
    //         {
    //             if (discovery != null)
    //             {
    //                 discovery.StopDiscovery();
    //             }

    //             NetworkManager.singleton.StopHost();
    //             SceneManager.LoadScene("MultiplayerMode");
    //         }
    //         // Client
    //         else if (NetworkClient.isConnected)
    //         {
    //             QuizNetworkManager manager =
    //                 (QuizNetworkManager)NetworkManager.singleton;

    //             manager.leavingLobby = true;

    //             NetworkManager.singleton.StopClient();
    //             SceneManager.LoadScene("Multi_Client");
    //         }

        
    // }
    // }

    public void BackFromLobby()
    {
        if (NetworkServer.active)
        {
            NetworkManager.singleton.StopHost();

            // if (AudioManager.Instance != null)
            // {
            //     AudioManager.Instance.PlayMainMenuMusic();
            // }

            SceneManager.LoadScene("MultiplayerMode");
        }
        else
        {
            QuizNetworkManager manager =
                (QuizNetworkManager)NetworkManager.singleton;

            manager.returningToLobby = true;

            NetworkManager.singleton.StopClient();

            // if (AudioManager.Instance != null)
            // {
            //     AudioManager.Instance.PlayMainMenuMusic();
            // }

            SceneManager.LoadScene("Multi_Client");
        }
    }



    public void JoinGame(string ip)
    {
        if (NetworkManager.singleton != null)
        {
            NetworkManager.singleton.networkAddress = ip;
            NetworkManager.singleton.StartClient();
        }
        else
        {
            Debug.LogError("NetworkManager not found!");
        }
    }



public void ReturnToLobby()
{
    if (NetworkServer.active)
    {
        // =========================================
        // HOST
        // =========================================

        QuizNetworkManager manager =
        (QuizNetworkManager)NetworkManager.singleton;

        manager.gameStarted = false;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMainMenuMusic();
        }

        SceneManager.LoadScene("Multi_Host");
    }
    else if (NetworkClient.isConnected)
    {
        // =========================================
        // CLIENT
        // =========================================

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMainMenuMusic();
        }

        SceneManager.LoadScene("Multi_Host");
    }
}

    public void ReturnToMainMenu()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMainMenuMusic();
        }

        Debug.Log("=== RETURN TO MAIN MENU ===");

        if (NetworkManager.singleton == null)
        {
            Debug.LogError("NetworkManager not found.");
            return;
        }

        // =========================================
        // HOST
        // =========================================

        if (NetworkServer.active && NetworkClient.isConnected)
        {
            Debug.Log("Returning HOST to Main Menu.");

            // Stop discovery if it is running.
            QuizNetDiscovery discovery =
                FindAnyObjectByType<QuizNetDiscovery>();

            if (discovery != null)
            {
                discovery.StopDiscovery();
            }

            // if (AudioManager.Instance != null)
            // {
            //     AudioManager.Instance.PlayMainMenuMusic();
            // }

            // This disconnects the host and all connected clients.
            NetworkManager.singleton.StopHost();

            // Host goes to Main Menu.
            LoadingScreen.destinationScene = "MainGame";
            LoadingScreen.isMultiplayer = false;

            SceneManager.LoadScene("LoadingScreen");

            return;
        }

        // =========================================
        // CLIENT
        // =========================================

        if (NetworkClient.isConnected)
        {
            Debug.Log("Returning CLIENT to Main Menu.");

            QuizNetworkManager manager =
                (QuizNetworkManager)NetworkManager.singleton;

            // Tell QuizNetworkManager that this is
            // an intentional disconnect.
            manager.leavingLobby = true;

            // if (AudioManager.Instance != null)
            // {
            //     AudioManager.Instance.PlayMainMenuMusic();
            // }

            // Disconnect ONLY this client.
            NetworkManager.singleton.StopClient();

            // Client goes to Main Menu.
            LoadingScreen.destinationScene = "MainGame";
            LoadingScreen.isMultiplayer = false;

            SceneManager.LoadScene("LoadingScreen");

            return;
        }

        // =========================================
        // ALREADY DISCONNECTED
        // =========================================

        Debug.Log(
            "Player is already disconnected. " +
            "Loading Main Menu."
        );

            LoadingScreen.destinationScene = "MainGame";
            LoadingScreen.isMultiplayer = false;

            SceneManager.LoadScene("LoadingScreen");
    }
}