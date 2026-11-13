using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Localization;

public class LoadingScreen : MonoBehaviour
{
     public static string destinationScene;
     public static bool isMultiplayer;
     public static float overrideLoadingDuration = 0f;

     public static bool isFromTutorial = false;

    [Header("Loading UI")]
    public Image timer_linear_image;
    public TMP_Text loadingText;

    private float loadingTextTimer = 0f;
    private int loadingDots = 1;

    [Header("Loading Settings")]
    public float loadingDuration = 8f;
    private float elapsedTime = 0f;
    public bool loadingStarted = false;

    public GameObject hostLeftToast;
    public TMP_Text hostLeftToastText;

    private int lastTipIndex = -1;
    private float tipTimer = 0f;
    public float tipInterval = 4f;

    public Sprite reloadBgImage;


    [SerializeField] private LocalizedString loadingTxt;
    string loadTxt;

    [SerializeField]
    private List<LocalizedString> loadingTipsMulti = new List<LocalizedString>();       // Missinput == use for single player


    [SerializeField]
    private List<LocalizedString> loadingTipsMultiPlayer = new List<LocalizedString>();

    // =========================================================
    // START
    // =========================================================

void Start()
{
    PersistentEnvironment.Instance.backgroundImg.sprite = reloadBgImage;

    loadingDuration = PlayerPrefs.GetFloat("LoadingDuration", 8f);  // Set to comment before publishing
    
    if(overrideLoadingDuration != 0)
        {
            loadingDuration = overrideLoadingDuration;
        }
    hostLeftToast.SetActive(false);
    elapsedTime = 0f;

    if (timer_linear_image != null)
    {
        timer_linear_image.fillAmount = 0f;
    }
    ShowRandomTip();

    StartLoading(
        destinationScene,
        isMultiplayer
    );

    loadTxt = loadingTxt.GetLocalizedString();
}


    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        if (!loadingStarted)
            return;

        if (elapsedTime < loadingDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsedTime / loadingDuration
                );

            if (timer_linear_image != null)
            {
                timer_linear_image.fillAmount =
                    progress;
            }
        tipTimer += Time.deltaTime;
        loadingTextTimer += Time.deltaTime;

        if(destinationScene != "MainGame" && destinationScene != "GameplayTutorial")
        {
            if (tipTimer >= tipInterval)
            {
                tipTimer = 0f;

                ShowRandomTip();
            }
        }
        else
        {
         if (loadingTextTimer >= 1f)
         {
             loadingTextTimer = 0f;

             loadingDots++;

             if (loadingDots > 3)
             {
                 loadingDots = 1;
             }

             if (loadingText != null)
             {
                 loadingText.text =
                     loadTxt +
                     new string('.', loadingDots);
             }
         }
        }

            // LOADING COMPLETE


            if (progress >= 1f)
            {
                loadingStarted = false;

                FinishLoading();
            }
        }
    }


    // =========================================================
    // START LOADING
    // =========================================================

    public void StartLoading(
        string destination,
        bool multiplayer)
    {
        destinationScene = destination;
        isMultiplayer = multiplayer;

        elapsedTime = 0f;

        if (timer_linear_image != null)
        {
            timer_linear_image.fillAmount = 0f;
        }

        loadingStarted = true;

        Debug.Log(
            "Loading started: " +
            destinationScene +
            " | Multiplayer: " +
            isMultiplayer
        );
    }


    // =========================================================
    // FINISH LOADING
    // =========================================================

    void FinishLoading()
    {
        Debug.Log(
            "Loading finished. Destination: " +
            destinationScene
        );

        if (isMultiplayer)
        {
            LoadForMultiplayer(destinationScene);
        }
        else
        {
            LoadForSingleplayer(destinationScene);
        }
    }


    // =========================================================
    // MULTIPLAYER
    // =========================================================

    void LoadForMultiplayer(string destination)
    {
        if (NetworkManager.singleton == null)
        {
            Debug.LogError(
                "NetworkManager not found!"
            );

            return;
        }

        if (!NetworkServer.active)
        {
            Debug.LogError(
                "Only the server/host can call " +
                "ServerChangeScene()."
            );

            return;
        }

        Debug.Log(
            "Changing multiplayer scene to: " +
            destination
        );

        NetworkManager.singleton.ServerChangeScene(
            destination
        );
    }


    // =========================================================
    // SINGLE PLAYER
    // =========================================================

    void LoadForSingleplayer(string destination)
    {
        Debug.Log(
            "Changing single-player scene to: " +
            destination
        );

        SceneManager.LoadScene(destination);
    }


    void ShowRandomTip()
    {
        int randomIndex;
        if(destinationScene == "Single_Indu" || destinationScene == "Single_Synonyms" || destinationScene =="Single_Antonyms")
        {
            do
            {
                randomIndex = Random.Range(0, loadingTipsMulti.Count);

            } while (loadingTipsMulti.Count > 1 &&
                    randomIndex == lastTipIndex);

            lastTipIndex = randomIndex;

            loadingText.text = loadingTipsMulti[randomIndex].GetLocalizedString();
        }
        else if (destinationScene == "Multi_Indu" || destinationScene == "Multi_Host")
        {
            do
            {
                randomIndex = Random.Range(0, loadingTipsMultiPlayer.Count);

            } while (loadingTipsMultiPlayer.Count > 1 &&
                    randomIndex == lastTipIndex);

            lastTipIndex = randomIndex;

            loadingText.text = loadingTipsMultiPlayer[randomIndex].GetLocalizedString();            
        }
    }
    public void ShowHostLeftToast()
    {
        if (hostLeftToast == null)
            return;

        hostLeftToast.SetActive(true);

        if (hostLeftToastText != null)
        {
            hostLeftToastText.text =
                "The host has left the room.";
        }
    }

}

