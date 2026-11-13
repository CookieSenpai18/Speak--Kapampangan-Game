using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SceneManagerScript : MonoBehaviour
{

    public int isFirstTime = 0;
    public TMP_Text versionDisplay;

    public GameObject devPanel;

    public GameObject secretBaackground;

    public Button backButtonUI; 


    string currentSceneName;
    public static bool isGenderActive = false;
    public static bool isAvatarActive = false;

    public Button genderButton, avatarButton, avatarButton2; // References to the buttons


    void Start()
    {
        if(PlayerPrefs.GetString("UserName", "Player") == "CookieSenpai")
        {
            if(devPanel != null)
            {
                devPanel.SetActive(true);
            }
        }
        else
        {
            if(devPanel != null)
            {
                devPanel.SetActive(false);
            }            
        }
        currentSceneName = SceneManager.GetActiveScene().name;
        LoadHiddenStuffs();

        // ResetGame(); // Delete this before final build

        if(versionDisplay != null)
        {
            versionDisplay.text = "Version " + Application.version;
        }


        isFirstTime = PlayerPrefs.GetInt("IsFirstTime", 0);
        if (isFirstTime == 0 && currentSceneName != "Introduction" && currentSceneName != "GameplayTutorial")
        {
            SceneManager.LoadScene("Introduction");
        }
    }

    public void ResetGame()
    {
        // PlayerPrefs.DeleteAll();
        PlayerPrefs.DeleteKey("IsFirstTime");       // Delete this before final build
        PlayerPrefs.Save();
        // SceneManager.LoadScene("Introduction");
    }

    public void LoadGame(int sceneIndex)
    {   
        PlayerPrefs.SetInt("DestinationScene", sceneIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SingleCustom");
    }

    public void LoadGameMulti(string sceneName)
    {   
        string gmode;

        switch (sceneName)
        {
            case "Multi_Indu":
                gmode = "Indu";
                break;
            case "Multi_Synonym":
                gmode = "Synonym";
                break;
            case "Multi_Antonym":
                gmode = "Antonym";
                break;
            default:
                gmode = "Indu";
                break;
        }
        PlayerPrefs.SetString("MGameMode", gmode);
        PlayerPrefs.SetString("MDestinationScene", sceneName);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MultiCustom");
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowLoadingScreen(string destination)
    {
            LoadingScreen.destinationScene = destination;
            LoadingScreen.isMultiplayer = false;

            SceneManager.LoadScene("LoadingScreen");
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame && backButtonUI != null)
        {
            if(currentSceneName =="ProfileMenu" && isGenderActive == true)
            {
                if(genderButton != null)
                {
                    genderButton.onClick.Invoke();
                }
                isGenderActive = false;
                return;
            }

            if(currentSceneName =="ProfileMenu" || currentSceneName =="Introduction")
            {
                if(isAvatarActive == true)
                {
                    Debug.Log("Escape key pressed in ProfileMenu or Introduction scene.");
                    if(avatarButton != null)
                    {
                        if(avatarButton2 != null)
                        {
                            avatarButton2.onClick.Invoke();
                        }
                        avatarButton.onClick.Invoke();

                    }
                    isAvatarActive = false;
                    return;                    
                }

            }
            HandleBackButton(backButtonUI);
        }
    }

    public void HandleBackButton(Button btn)
    {
        if (btn != null)
        {
            btn.onClick.Invoke();
        }
    }

    public void LoadHiddenStuffs()
    {
        if(currentSceneName != "Bootstrap" && currentSceneName != "BootIntro")
        {
            var varObj = FindAnyObjectByType<CustomBackground>();
            if(varObj != null)
            {
                varObj.InitializeStart();
            }
        }


        // string playerName = PlayerPrefs.GetString("UserName", "Player");         // Used for testing featshits

        // if(secretBaackground != null && playerName == "Autumn843y")
        // {
        //     secretBaackground.SetActive(true);
        // }
        // if(playerName == "Kapampangan")
        // {
        //     var varObj = FindAnyObjectByType<LanguageSelector>();
        //     if(varObj != null)
        //     {
        //         varObj.SetKapampangan();
        //     }
        // }
    }

}
