using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Mirror;
using UnityEngine.Localization;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    public TMP_Text infoText;
    public Button startButton;
    [SerializeField] private TMP_Text[] playerSlots;

    [SerializeField] private LocalizedString waitingTxt;
    [SerializeField] private LocalizedString allSetTxt;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (!NetworkServer.active)
        {
            startButton.gameObject.SetActive(false);
        }
        UpdateInfoText();
    }

    void Update()
    {
        if(startButton == null)
            return;

        startButton.gameObject.SetActive(
            NetworkServer.active
        );
    }

    public void RefreshLobby()
    {
        NetworkPlayer.CleanupPlayers();

        // Clear everything first
        for (int i = 0; i < playerSlots.Length; i++)
        {
            playerSlots[i].text = "";
        }

        // Fill connected players
        for (int i = 0;
            i < NetworkPlayer.Players.Count &&
            i < playerSlots.Length;
            i++)
        {
            NetworkPlayer player =
                NetworkPlayer.Players[i];

            if (player == null)
                continue;

            string displayName =
                player.playerName;

            if (player.isHost)
                displayName += " (Host)";

            playerSlots[i].text =
                displayName;
        }

        UpdateStartButton();
        UpdateInfoText();
    }

    void UpdateStartButton()
    {
        if (startButton == null)
            return;

        // Host counts as one player
        int playerCount = NetworkPlayer.Players.Count;

        startButton.interactable = playerCount > 1;
    }
    void UpdateInfoText()
    {
                if(NetworkPlayer.Players.Count == 4 && infoText != null)
        {
            infoText.text = "All set!\nPress Start to begin.";
        }
        else
        {
            infoText.text = waitingTxt.GetLocalizedString() + "(" + NetworkPlayer.Players.Count + "/4)";
        }

    }

}