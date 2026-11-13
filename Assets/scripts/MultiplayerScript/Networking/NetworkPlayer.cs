using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public static readonly List<NetworkPlayer> Players = new List<NetworkPlayer>();
    public System.Action<NetworkPlayer> AvatarChanged;

    [Header("Player Data")]
    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    public string playerName;

    [SyncVar]
    public int score;


    [SyncVar]
    public bool isQuizHolder;

    [SyncVar]
    public bool isReady;

    [SyncVar]
    public bool isHost;

    [SyncVar]
    public bool answered;

    [SyncVar]
    public int quizRound;

    [Header("Quiz Round Data")]

    [SyncVar(hook = nameof(OnQuizRoleChanged))]
    public bool isCorrectAnswerPlayer;

    [SyncVar(hook = nameof(OnChoice1Changed))]
    public string choice1;

    [SyncVar(hook = nameof(OnChoice2Changed))]
    public string choice2;

    [SyncVar(hook = nameof(OnChoice3Changed))]
    public string choice3;

    [SyncVar(hook = nameof(OnQuestionChanged))]
    public string questionName;

    [SyncVar(hook = nameof(OnFlashcardResourcePathChanged))]
    public string flashcardResourcePath;
    [SyncVar]
    public string correctAnswer;

    [SyncVar]
    public int totalMistakes = 0;   

    [SyncVar]
    public float performance = 100f;

    public int currentFirstChoiceStreak = 0;

    [SyncVar]
    public int highestFirstChoiceStreak = 0;

    [SyncVar]
    public int totalCorrectAnswers = 0;

    [SyncVar]
    public int totalHintsUsed = 0;

    [SyncVar]
    public int avatarIndex = 0;

    [SyncVar(hook = nameof(OnUsingCustomAvatarChanged))]
    public bool usingCustomAvatar = false;

    [SyncVar(hook = nameof(OnCustomAvatarChanged))]
    public byte[] customAvatarData;


        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!Players.Contains(this))
            {
                Players.Add(this);
            }

            LobbyManager.Instance?.RefreshLobby();  //Added
            //RefreshLobbyIfAvailable();
        }


        public override void OnStopClient()
        {
            Players.Remove(this);

            RefreshLobbyIfAvailable();

            LobbyManager.Instance?.RefreshLobby();  //Added
            base.OnStopClient();
        }
    private void RefreshLobbyIfAvailable()
    {
        if (LobbyManager.Instance != null)
        {
            LobbyManager.Instance.RefreshLobby();
        }
    }

    public static void CleanupPlayers()
    {
        for (int i = Players.Count - 1; i >= 0; i--)
        {
            if (Players[i] == null ||
                !Players[i].isActiveAndEnabled)
            {
                Players.RemoveAt(i);
            }
        }
    }
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        string localName =
            PlayerPrefs.GetString("UserName", "Player");

        bool useCustom =
            PlayerPrefs.GetInt("UsingCustomAvatar", 0) == 1;

        Debug.Log(
            $"[Avatar] Local player started. Custom = {useCustom}"
        );

        CmdSetPlayerName(localName);

        if (useCustom)
        {
            string path = System.IO.Path.Combine(
                Application.persistentDataPath,
                "custom_avatar.png"
            );

            Debug.Log(
                $"[Avatar] Looking for custom avatar: {path}"
            );

            if (System.IO.File.Exists(path))
            {
                byte[] originalData =
                    System.IO.File.ReadAllBytes(path);

                byte[] avatarData =
                    ResizeAndEncodeAvatar(originalData);

                if (avatarData != null &&
                    avatarData.Length > 0)
                {
                    CmdSetCustomAvatar(avatarData);
                }
                else
                {
                    Debug.LogError(
                        "[Avatar] Failed to prepare custom avatar."
                    );

                    CmdSetPlayerAvatar(
                        PlayerPrefs.GetInt(
                            "SelectedAvatar",
                            0
                        )
                    );
                }
                                Debug.Log(
                "[Avatar] CmdSetCustomAvatar call completed."
            );
            }
            else
            {
                Debug.LogWarning(
                    "[Avatar] Custom avatar file does NOT exist."
                );

                CmdSetPlayerAvatar(
                PlayerPrefs.GetInt("SelectedAvatar", 0)
                );
            }
        }
        else
        {
            int localAvatar =
                PlayerPrefs.GetInt("SelectedAvatar", 0);

            Debug.Log(
                $"[Avatar] Using preset avatar: {localAvatar}"
            );

            CmdSetPlayerAvatar(localAvatar);
        }
    }
[Command]
public void CmdSetPlayerAvatar(int newAvatarIndex)
{
    avatarIndex = newAvatarIndex;

    usingCustomAvatar = false;
    customAvatarData = null;

    Debug.Log(
        $"[Avatar SERVER] {playerName} selected preset {newAvatarIndex}"
    );
}

    [Command]
    public void CmdSetCustomAvatar(byte[] imageData)
    {
        Debug.Log(
            $"[Avatar SERVER] CmdSetCustomAvatar received from " +
            $"{connectionToClient?.connectionId}"
        );

        if (imageData == null || imageData.Length == 0)
        {
            Debug.LogWarning(
                "[Avatar SERVER] Avatar data is empty."
            );

            return;
        }

        Debug.Log(
            $"[Avatar SERVER] Received {imageData.Length} bytes."
        );

        string base64 =
            System.Convert.ToBase64String(imageData);

        customAvatarData = imageData;

        avatarIndex = 0;

        usingCustomAvatar = true;

        Debug.Log(
            $"[Avatar SERVER] Custom avatar assigned. " +
            $"Base64 length = {base64.Length}"
        );
    }

    [Command]
    private void CmdSetPlayerName(string newName)
    {
        playerName = newName;
    }


    private void OnPlayerNameChanged(string oldName, string newName)
    {
        //Debug.Log($"Player name synchronized: {newName}");

        LobbyManager.Instance?.RefreshLobby();
    }


    public override void OnStartServer()
    {
        base.OnStartServer();

        //Debug.Log("Player object spawned on Server.");
    }


    private void OnQuizRoleChanged(bool oldValue, bool newValue)
    {
        MultiQuizUI.Instance?.RefreshUI();
    }

    private void OnChoice1Changed(string oldValue, string newValue)
    {
        MultiQuizUI.Instance?.RefreshUI();
    }

    private void OnChoice2Changed(string oldValue, string newValue)
    {
        MultiQuizUI.Instance?.RefreshUI();
    }

    private void OnChoice3Changed(string oldValue, string newValue)
    {
        MultiQuizUI.Instance?.RefreshUI();
    }

    private void OnQuestionChanged(string oldValue, string newValue)
    {
        MultiQuizUI.Instance?.RefreshQuestionCard();
    }
    [Server]
    public void SetQuizRoundData(
        bool quizHolder,
        bool correctAnswerPlayer,
        string newChoice1,
        string newChoice2,
        string newChoice3,
        string newCorrectAnswer,
        string newQuestionName)
    {
        isQuizHolder = quizHolder;
        isCorrectAnswerPlayer = correctAnswerPlayer;

        choice1 = newChoice1;
        choice2 = newChoice2;
        choice3 = newChoice3;

        correctAnswer = newCorrectAnswer;
        questionName = newQuestionName;

        answered = false;
    }

    [Command]
    public void CmdSubmitAnswer(string selectedAnswer)
    {
        if (MultiGameManager.Instance == null)
            return;

        MultiGameManager.Instance.ServerProcessAnswer(
            this,
            selectedAnswer
        );
    }

    [Command]
    public void CmdUseHint()
    {
        Debug.Log(
            playerName + " requested a hint."
        );

        if (MultiGameManager.Instance == null)
        {
            Debug.LogError(
                "CmdUseHint: MultiGameManager.Instance is null."
            );

            return;
        }

        MultiGameManager.Instance.ServerProcessHint(this);
    }

    [Command]
    public void CmdEndGame()
    {
        if (MultiGameManager.Instance == null)
        {
            Debug.LogError(
                "CmdEndGame: MultiGameManager.Instance is null."
            );

            return;
        }
        MultiGameManager.Instance.ServerRegisterEndGameVote(this);  
        // MultiGameManager.Instance.ServerEndGame();
    }

    [Command]
    public void CmdVoteReturnToLobby()
    {
        if (MultiGameManager.Instance == null)
        {
            Debug.LogWarning(
                "CmdVoteReturnToLobby: MultiGameManager not found."
            );

            return;
        }

        MultiGameManager.Instance.ServerRegisterLobbyVote(this);
    }

private void OnCustomAvatarChanged(
    byte[] oldData,
    byte[] newData)
{
    AvatarChanged?.Invoke(this);
}

    private void OnUsingCustomAvatarChanged(
        bool oldValue,
        bool newValue)
    {
        Debug.Log(
            $"[Avatar CLIENT] {playerName} " +
            $"usingCustomAvatar changed: " +
            $"{oldValue} -> {newValue}"
        );
        AvatarChanged?.Invoke(this);
    }

    [ClientRpc]
    public void RpcPlayMainMenuMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMainMenuMusic();
            Debug.Log("[Audio] Main menu music started.");
        }
    }

    private void OnFlashcardResourcePathChanged(string oldValue, string newValue)
    {
        MultiQuizUI.Instance?.RefreshQuestionCard();
    }


    private byte[] ResizeAndEncodeAvatar(
        byte[] originalData)
    {
        Texture2D original =
            new Texture2D(2, 2);

        if (!original.LoadImage(originalData))
        {
            Destroy(original);

            Debug.LogError(
                "[Avatar] Failed to load original avatar."
            );

            return null;
        }

        int maxSize = 256;

        int newWidth = original.width;
        int newHeight = original.height;

        if (newWidth > maxSize ||
            newHeight > maxSize)
        {
            float scale =
                Mathf.Min(
                    (float)maxSize / newWidth,
                    (float)maxSize / newHeight
                );

            newWidth =
                Mathf.RoundToInt(
                    newWidth * scale
                );

            newHeight =
                Mathf.RoundToInt(
                    newHeight * scale
                );
        }

        RenderTexture renderTexture =
            RenderTexture.GetTemporary(
                newWidth,
                newHeight,
                0,
                RenderTextureFormat.ARGB32
            );

        Graphics.Blit(
            original,
            renderTexture
        );

        RenderTexture previous =
            RenderTexture.active;

        RenderTexture.active =
            renderTexture;

        Texture2D resized =
            new Texture2D(
                newWidth,
                newHeight,
                TextureFormat.RGBA32,
                false
            );

        resized.ReadPixels(
            new Rect(
                0,
                0,
                newWidth,
                newHeight
            ),
            0,
            0
        );

        resized.Apply();

        RenderTexture.active =
            previous;

        RenderTexture.ReleaseTemporary(
            renderTexture
        );

        byte[] result =
            resized.EncodeToPNG();

        Destroy(original);
        Destroy(resized);

        Debug.Log(
            $"[Avatar] Resized avatar to " +
            $"{newWidth}x{newHeight}. " +
            $"Network size = " +
            $"{result.Length / 1024f:F2} KB"
        );

        return result;
    }
}

