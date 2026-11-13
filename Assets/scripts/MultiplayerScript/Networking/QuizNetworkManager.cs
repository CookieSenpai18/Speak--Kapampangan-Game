using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizNetworkManager : NetworkManager
{
    public bool isGameActive = false;
    public bool leavingLobby = false;
    public bool returningToLobby = false;
    public GameObject roomSettingsPrefab;
    public GameObject multiGameManagerPrefab;
    public bool HostStillExists { get; private set; }
    public bool gameInProgress = false;
    public bool gameStarted = false;

    private NetworkRoomSettings roomSettingsInstance;


    public override void OnStartServer()
    {
        base.OnStartServer();

        HostStillExists = true; 

        GameObject settings =
            Instantiate(roomSettingsPrefab);

        DontDestroyOnLoad(settings);

        roomSettingsInstance =
            settings.GetComponent<NetworkRoomSettings>();

        NetworkServer.Spawn(settings);

        Debug.Log("=== NETWORK ROOM SETTINGS SPAWNED ===");
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        // Debug.Log($"Player spawned for connection: {conn.connectionId}");
    }

    public override void OnServerConnect(
        NetworkConnectionToClient conn)
    {
        QuizNetDiscovery discovery =
            FindAnyObjectByType<QuizNetDiscovery>();

        if (discovery != null &&
            discovery.isGameStarted)
        {
            Debug.Log(
                "Connection rejected: game is already in progress."
            );

            conn.Disconnect();
            return;
        }

        base.OnServerConnect(conn);

        Debug.Log(
            "Client connection accepted."
        );  
    }

    public override void OnServerDisconnect(
        NetworkConnectionToClient conn)
    {
        // =========================================
        // GET LEAVING PLAYER
        // =========================================

        NetworkPlayer leavingPlayer =
            conn.identity != null
                ? conn.identity.GetComponent<NetworkPlayer>()
                : null;

        string leavingPlayerName = "A player";

        if (leavingPlayer != null)
        {
            leavingPlayerName =
                leavingPlayer.playerName;
        }

        Debug.Log(
            "PLAYER LEFT: " +
            leavingPlayerName
        );

        // =========================================
        // SHOW TOAST TO REMAINING PLAYERS
        // =========================================

        MultiGameManager gameManager =
            MultiGameManager.Instance;

        if (gameManager != null)
        {
            gameManager.RpcShowPlayerLeftToast(
                leavingPlayerName
            );
            gameManager.ServerPlayerDisconnected(leavingPlayer);
        }

        // =========================================
        // LET MIRROR HANDLE DISCONNECT
        // =========================================

        base.OnServerDisconnect(conn);


    if (gameManager != null)
    {
        gameManager.CheckPlayersAfterDisconnect();
    }

    }
    public override void OnClientConnect()
    {
        HostStillExists = true;
        base.OnClientConnect();
        NetworkManager.singleton.ServerChangeScene("Multi_Host");
        // UnityEngine.SceneManagement.SceneManager.LoadScene("Multi_Host");
    }

    public override void OnClientDisconnect()
    {
        HostStillExists = false;

        string sceneName =
            SceneManager.GetActiveScene().name;

        Debug.Log(
            "=== HOST DISCONNECTED ==="
        );

        Debug.Log(
            "Current Scene: " + sceneName
        );

        // =========================================
        // CHECK IF GAME IS STILL ACTIVE
        // =========================================

        bool isMultiplayerGame =
            sceneName == "Multi_Indu" ||
            sceneName == "Multi_Synonym" ||
            sceneName == "Multi_Antonym";

        bool gameIsOver = false;

        if (MultiGameManager.Instance != null)
        {
            gameIsOver =
                MultiGameManager.Instance.gameOver;
        }

        // =========================================
        // HOST LEFT DURING GAME
        // =========================================

        if (isMultiplayerGame && !gameIsOver)
        {
            Debug.Log(
                "Host left while game was active. " +
                "Returning client to Main Menu."
            );

            LoadingScreen.destinationScene = "MultiSelectMode";
            LoadingScreen.isMultiplayer = false;

            SceneManager.LoadScene("LoadingScreen");
        }

        // =========================================
        // HOST LEFT FROM LOBBY
        // =========================================

        else if (sceneName == "Multi_Host")
        {
            Debug.Log(
                "Host left the lobby. " +
                "Returning client to Main Menu."
            );

            SceneManager.LoadScene(
                "MultiSelectMode"
            );
        }
        else if(sceneName == "LoadingScreen")
        {
            LoadingScreen.destinationScene = "MultiSelectMode";
            LoadingScreen.isMultiplayer = false;    

        LoadingScreen loading =
            FindAnyObjectByType<LoadingScreen>();

        if (loading != null)
        {
            loading.ShowHostLeftToast();
        }   
        }

        base.OnClientDisconnect();
    }

    public override void OnStopServer()
    {

        HostStillExists = false;
        NetworkPlayer.Players.Clear();
        base.OnStopServer();
    }

    public override void OnStopClient()
    {
        NetworkPlayer.Players.Clear();

        // Debug.Log("Client stopped. NetworkPlayer list cleared.");

        base.OnStopClient();
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        QuizNetDiscovery discovery =
            FindAnyObjectByType<QuizNetDiscovery>();

        if (sceneName == "Multi_Indu" ||
            sceneName == "Multi_Synonym" ||
            sceneName == "Multi_Antonym")
        {
            if (discovery != null)
            {
                discovery.isGameStarted = true;

                Debug.Log(
                    "Discovery: Game session started."
                );
            }

            SpawnMultiGameManager();
        }
        else if (sceneName == "Multi_Host")
        {
            if (discovery != null)
            {
                discovery.isGameStarted = false;

                Debug.Log(
                    "Discovery: Lobby available again."
                );
            }
        }
    }
    [Server]
    void SpawnMultiGameManager()
    {
        if (MultiGameManager.Instance != null)
        {
            Debug.Log(
                "MultiGameManager already exists."
            );

            return;
        }

        if (multiGameManagerPrefab == null)
        {
            Debug.LogError(
                "MultiGameManager Prefab is not assigned!"
            );

            return;
        }


        GameObject manager =
            Instantiate(
                multiGameManagerPrefab
            );

        NetworkServer.Spawn(manager);

        Debug.Log(
            "=== MULTI GAME MANAGER SPAWNED ==="
        );
    }

}