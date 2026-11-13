using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HostButtonUI : MonoBehaviour
{
    public Uri serverUri;
    public long serverId;
    public float lastSeen;

    public TMP_Text roomNameText;
    public TMP_Text roomDescriptionText;
    public TMP_Text playerCountText;

    public string serverAddress;

    private Button button;

    private bool isInGame;
    public bool isGameStarted;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void SetServerAddress(string address)
    {
        serverAddress = address;
    }

    public void Setup(
        string roomName,
        string description,
        int currentPlayers,
        int maxPlayers,
        bool inGame)
    {
        roomNameText.text = roomName;
        roomDescriptionText.text = description;

        isInGame = inGame;
        this.isGameStarted = inGame;

        // =========================================
        // IN-GAME
        // ========================================= 

        if (inGame)
        {
            playerCountText.text = "In-Game";

            Debug.Log($"Text is now ======== " + playerCountText.text);
            button.interactable = false;

            // Brown
            button.image.color =
                new Color32(87, 175, 14, 255);

            return;
        }

        // =========================================
        // FULL
        // =========================================

        if (currentPlayers >= maxPlayers)
        {
            playerCountText.text =
                currentPlayers + "/" + maxPlayers;

            button.interactable = false;

            // Red
            button.image.color = new Color32(255, 75, 59, 255);

            return;
        }

        // =========================================
        // AVAILABLE
        // =========================================

        playerCountText.text =
            currentPlayers + "/" + maxPlayers;

        button.interactable = true;

        // Yellow = joinable
        button.image.color = new Color32(150, 255, 53, 255);
    }

    public bool IsInGame()
    {
        return isInGame;
    }

    public void SetSelected(bool selected)
    {
        // Never allow visual selection to overwrite
        // the room's actual state.

        if (isInGame)
        {
            button.image.color =
                new Color32(139, 69, 19, 255);

            return;
        }

        if (!button.interactable)
        {
            // Full room
            button.image.color = Color.red;

            return;
        }

        if (selected)
            button.image.color = Color.green;
        else
            button.image.color = Color.yellow;
    }
}