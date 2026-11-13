using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Player Info")]
    public TMP_Text idText;
    public TMP_Text nameText;
    public Image avatarImage;

    [Header("Statistics")]
    public TMP_Text correctText;
    public TMP_Text wrongText;
    public TMP_Text hintText;

    [Header("Avatar Sprites")]
    public Sprite[] avatarSprites;


    private Sprite displayedCustomSprite;
    private Texture2D displayedCustomTexture;

    private NetworkPlayer currentPlayer;



    // private void Update()   
    // {
    //     if (currentPlayer != null)
    //     {
    //         DisplayAvatar(currentPlayer);
    //     }
    // }



    public void SetPlayerData(
        int playerNumber,
        NetworkPlayer player)
    {
        if (player == null)
        {
            Debug.LogWarning(
                "PlayerStatsUI: Player is null."
            );

            return;
        }

        currentPlayer = player;

    player.AvatarChanged -= OnPlayerAvatarChanged;
    player.AvatarChanged += OnPlayerAvatarChanged;

        // -----------------------------
        // PLAYER NUMBER
        // -----------------------------

        if (idText != null)
        {
            idText.text =
                playerNumber.ToString();
        }

        // -----------------------------
        // USERNAME
        // -----------------------------

        if (nameText != null)
        {
            nameText.text =
                player.playerName;
        }

        // -----------------------------
        // CORRECT
        // -----------------------------

        if (correctText != null)
        {
            float performanceScore;
            float mistakes = player.totalMistakes;
            float rounds = MultiGameManager.Instance.currentRound;

            performanceScore = 100 * (1 - (mistakes / (3 * rounds)));

            correctText.text =
                performanceScore.ToString("F2") + "%";

            Debug.Log(
                $"PlayerStatsUI: Player {player.playerName} performance score updated to {performanceScore:F2}%, mistakes: {mistakes}, rounds: {rounds}"
            );
        }

        // -----------------------------
        // WRONG
        // -----------------------------

        if (wrongText != null)
        {
            wrongText.text =
                player.totalMistakes.ToString();
        }

        // -----------------------------
        // HINT
        // -----------------------------

        if (hintText != null)
        {
            hintText.text =
                player.totalHintsUsed.ToString();
        }

        // -----------------------------
        // AVATAR
        // -----------------------------

    DisplayAvatar(player);
    }


private void DisplayAvatar(NetworkPlayer player)
{
    if (avatarImage == null || player == null)
        return;

    // =========================================
    // CUSTOM AVATAR
    // =========================================

    if (player.usingCustomAvatar)
    {
        if (player.customAvatarData == null ||
            player.customAvatarData.Length == 0)
        {
            Debug.LogWarning(
                $"[PlayerStatsUI] {player.playerName} " +
                "has custom avatar enabled but " +
                "no image data has arrived yet."
            );

            return;
        }

        try
        {
            Texture2D texture =
                new Texture2D(2, 2);

            if (!texture.LoadImage(
                    player.customAvatarData))
            {
                Debug.LogWarning(
                    $"[PlayerStatsUI] Failed to load " +
                    $"custom avatar for {player.playerName}."
                );

                Destroy(texture);
                return;
            }

            Sprite customSprite =
                Sprite.Create(
                    texture,
                    new Rect(
                        0,
                        0,
                        texture.width,
                        texture.height
                    ),
                    new Vector2(0.5f, 0.5f)
                );

            displayedCustomTexture = texture;
            displayedCustomSprite = customSprite;

            avatarImage.sprite =
                displayedCustomSprite;

            Debug.Log(
                $"[PlayerStatsUI] Custom avatar displayed " +
                $"for {player.playerName}. " +
                $"Resolution = " +
                $"{texture.width}x{texture.height}"
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                $"[PlayerStatsUI] Failed to decode " +
                $"custom avatar: {e.Message}"
            );
        }

        return;
    }


    // =========================================
    // PRESET AVATAR
    // =========================================

    if (avatarSprites == null ||
        avatarSprites.Length == 0)
    {
        return;
    }

    int index =
        Mathf.Clamp(
            player.avatarIndex,
            0,
            avatarSprites.Length - 1
        );

    avatarImage.sprite =
        avatarSprites[index];
}

private void OnPlayerAvatarChanged(NetworkPlayer player)
{
    if (currentPlayer != player)
        return;

    Debug.Log(
        $"[PlayerStatsUI] Avatar changed for " +
        $"{player.playerName}"
    );

    ClearCustomAvatar();

    DisplayAvatar(player);
}


private void ClearCustomAvatar()
{
    if (displayedCustomSprite != null)
    {
        Destroy(displayedCustomSprite);
        displayedCustomSprite = null;
    }

    if (displayedCustomTexture != null)
    {
        Destroy(displayedCustomTexture);
        displayedCustomTexture = null;
    }
}

private void OnDestroy()
{
    if (currentPlayer != null)
    {
        currentPlayer.AvatarChanged -=
            OnPlayerAvatarChanged;
    }

    ClearCustomAvatar();
}
}