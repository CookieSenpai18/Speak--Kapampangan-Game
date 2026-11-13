using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ClientLobbyManager : MonoBehaviour
{
    private HostButtonUI selectedHost;
    private Dictionary<long, HostButtonUI> discoveredHosts =
    new Dictionary<long, HostButtonUI>();
    [Header("UI")]
    public Transform content;
    public Button hostButtonPrefab;
    public Button joinButton;

    private Button selectedButton;
    private QuizNetDiscovery discovery;

    [System.Obsolete]
    void Start()
    {
        joinButton.interactable = false;


    discovery = FindObjectOfType<QuizNetDiscovery>();

    if (discovery == null)
    {
        Debug.LogError("QuizNetDiscovery not found!");
        return;
    }

    discovery.OnServerFound.AddListener(OnServerFound);

    discovery.StartDiscovery();
    }

void Update()
{
    List<long> expiredRooms = new List<long>();

    foreach (var room in discoveredHosts)
    {
        if (Time.time - room.Value.lastSeen > 4f)
        {
            expiredRooms.Add(room.Key);
        }
    }

    foreach (long id in expiredRooms)
    {
        RemoveRoom(id);
    }
}

    void RemoveRoom(long serverId)
    {
        if (!discoveredHosts.TryGetValue(
            serverId,
            out HostButtonUI room))
            return;

        bool wasSelected =
            selectedHost != null &&
            selectedHost.serverId == serverId;

        Debug.Log(
            "REMOVING ROOM | Server ID: " +
            serverId +
            " | Was Selected: " +
            wasSelected
        );

        // Remove from dictionary first.
        discoveredHosts.Remove(serverId);

        Destroy(room.gameObject);

        // Only clear the Join button if THIS
        // specific room was selected.
        if (wasSelected)
        {
            selectedHost = null;
            selectedButton = null;

            joinButton.interactable = false;

            Debug.Log(
                "Selected room disappeared. Join disabled."
            );
        }
    }

    
void OnServerFound(QuizServerResponse response)
{
if (discoveredHosts.TryGetValue(
    response.serverId,
    out HostButtonUI existing))
{
    existing.Setup(
        response.roomName,
        response.gameMode + " - " +
        response.difficulty + " - " +
        response.timeMode,
        response.currentPlayers,
        response.maxPlayers,
        response.isInGame
    );

    Debug.Log(
    "DISCOVERY UPDATE | Server ID: " +
    response.serverId +
    " | Players: " +
    response.currentPlayers +
    " | InGame: " +
    response.isInGame
);

    existing.lastSeen = Time.time;

    // =========================================
    // PRESERVE CURRENT SELECTION
    // =========================================

    if (selectedHost == existing)
    {
        bool available =
            !existing.IsInGame() &&
            response.currentPlayers < response.maxPlayers;

        if (available)
        {
            // DO NOT lose the selection.
            joinButton.interactable = true;
        }
        else
        {
            // Room became unavailable.
            selectedHost = null;
            selectedButton = null;

            joinButton.interactable = false;
        }
    }

    return;
}
    // =========================================
    // NEW ROOM
    // =========================================

    Button newButton = CreateHostButton(
        response.roomName,
        response.gameMode + " - " +
        response.difficulty + " - " +
        response.timeMode,
        response.currentPlayers,
        response.maxPlayers,
        response.isInGame
    );

    HostButtonUI ui =
        newButton.GetComponent<HostButtonUI>();

    ui.serverId = response.serverId;
    ui.serverUri = response.uri;
    ui.lastSeen = Time.time;

    discoveredHosts.Add(
        response.serverId,
        ui
    );
}

    public void ClearHostList()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        discoveredHosts.Clear();

        selectedHost = null;
        selectedButton = null;

        joinButton.interactable = false;
    }
    public Button CreateHostButton(
        string roomName,
        string description,
        int currentPlayers,
        int maxPlayers,
        bool isInGame)
    {
        Button btn =
            Instantiate(
                hostButtonPrefab,
                content
            );

        HostButtonUI ui =
            btn.GetComponent<HostButtonUI>();

        ui.Setup(
            roomName,
            description,
            currentPlayers,
            maxPlayers,
            isInGame
        );

        btn.onClick.AddListener(() =>
        {
            SelectHost(btn);
        });

        return btn;
    }
    void SelectHost(Button btn)
    {
        HostButtonUI host =
            btn.GetComponent<HostButtonUI>();

        if (host == null)
            return;

        // Don't select unavailable rooms.
        if (host.IsInGame())
        {
            joinButton.interactable = false;
            return;
        }

        if (!btn.interactable)
        {
            joinButton.interactable = false;
            return;
        }

        selectedButton = btn;
        selectedHost = host;

        joinButton.interactable = true;

        Debug.Log(
            "Selected Room: " +
            selectedHost.roomNameText.text
        );
    }

public void JoinSelectedRoom()
{
    if (selectedHost == null)
    {
        Debug.Log("No room selected.");
        return;
    }

    if (selectedHost.isGameStarted)
    {
        Debug.Log("Cannot join. The game is already in progress.");
        return;
    }

    QuizNetworkManager.singleton.networkAddress =
        selectedHost.serverUri.Host;

    QuizNetworkManager.singleton.StartClient();

    LoadingScreen.destinationScene = "Multi_Indu";
    LoadingScreen.isMultiplayer = true;
}
    }