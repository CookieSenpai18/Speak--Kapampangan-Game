using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PanelLoader : MonoBehaviour
{

    public Sprite activeImage = null, inactiveImage = null;
    public Button buttonObject = null;
    public Image toggleImage = null;

    public GameObject panel = null; // The panel to show/hide
    public GameObject[] elementsToToggle = new GameObject[0]; // Array of UI elements to toggle

    public GameObject timerBar, scoreBar;
    public Image timerShake;

    public Button backButtonUI; // Reference to the back button


    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame && backButtonUI != null)
        {
            
            HandleBackButton(backButtonUI);
        }
    }

    public void HandleBackButton(Button btn)
    {
        if(GameTimer.Instance != null && GameTimer.Instance.timeUp == true)
        {
            return;
        }
        if (btn != null)
        {
            btn.onClick.Invoke();
        }
    }

    public void ShowPanel(GameObject panels)    // For triggering pause menu while timer is active
    {
        if(GameTimer.Instance != null && GameTimer.Instance.timeUp == true)
        {
            return;
        }
        if (panels != null)
        {
            panels.SetActive(!panels.activeSelf);

        }

    }

    public void ShowPanelGen(GameObject panels)     // For general Use no restriction
    {
        if (panels != null)
        {
            panels.SetActive(!panels.activeSelf);

        }

    } 

        public void ShowPauseMenu()
    {

        int timeData = PlayerPrefs.GetInt("TimeDuration", 0);
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
            buttonObject.image.sprite = panel.activeSelf ? activeImage : inactiveImage;
            toggleImage.enabled = !toggleImage.enabled;
            timerShake.enabled = !timerShake.enabled;

            foreach (GameObject element in elementsToToggle)
            {
                element.SetActive(!element.activeSelf);
            }
            if (timerBar != null && timeData == 2)
            {
                timerBar.SetActive(!timerBar.activeSelf);
                scoreBar.SetActive(!scoreBar.activeSelf);
                timerShake.enabled = !timerShake.enabled;
            }
        }

    }

        public void EndSession()
    {
        GameTimer.Instance.StopGame();
        ShowPauseMenu();
    }

    public void ToggleAvatar()
    {
        SceneManagerScript.isAvatarActive = !SceneManagerScript.isAvatarActive;
    }

    public void ToggleGender()
    {
        SceneManagerScript.isGenderActive = !SceneManagerScript.isGenderActive;
    }


}
