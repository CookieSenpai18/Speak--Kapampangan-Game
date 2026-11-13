using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class MultiTimer : MonoBehaviour
{

    [Header("Untimed Timer Display")]
    public TMP_Text untimedTimeText;

    [Header("Timer Mode UI")]

    public GameObject timedUI;
    public GameObject untimedUI;

    [Header("Multiplayer Game Over UI")]

    public List<GameObject> gameplayUIToHide;

    [Header("Multiplayer Result")]
    public GameObject resultPanel;

    [Header("Timer Colors")]
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;

    public Image clockImageAnimated;
    public Image clockImageStatic;
    public Animator clockAnimator;
    public Image timerShake;
    public float animationDuration = 2f;


    public static MultiTimer Instance;
    public GameObject times_up_textholder;
    public Image timer_linear_image;
    // public Image timer_radial_image;    // For radial type timer
    public float time_remaining;
    public float max_time = 60f;
    public bool timeUp = false;
    public GameObject timerBar, scoreBar, pauseButton, pausePanel;   // To disable when time's up
    //public GameObject timer_radial_textholder;   //idk
    public Image titleImage;

    public bool isPaused = false;
    bool isTimeClosing = false;
    bool timerInitialized = false;
    bool animationPlayed = false;
    int timeData = 0;
    public GameObject pausePanelUI;

    public TMP_Text digitalClock;

    bool isTinted = false;

    // private bool timerModeInitialized = false;



    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    void Start()
    {
        clockAnimator.enabled = false;
        StartCoroutine(InitializeTimerModeUI());
    }



    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null &&
        Keyboard.current.escapeKey.wasPressedThisFrame )
        {
            PauseButton(pausePanelUI);
        }
        
        if (MultiGameManager.Instance == null)
        {
            return;
        }
            

        MultiGameManager game =
            MultiGameManager.Instance;

        isPaused = game.timerPaused;



        // =========================================
        // WAIT FOR SERVER TO SEND INITIAL TIME
        // =========================================

        // =========================================
        // UNTIMED MODE
        // =========================================

        if (game.timeRemaining == -1)
        {
            if (timerBar != null)
                timerBar.SetActive(false);

            if (timerShake != null)
                timerShake.enabled = false;

            return;
        }


        // =========================================
        // WAIT FOR SERVER TO SEND INITIAL TIME
        // =========================================

    // =========================================
    // WAIT FOR SERVER TO SEND INITIAL TIME
    // =========================================

    if (!timerInitialized)
    {
        // -------------------------------
        // UNTIMED
        // -------------------------------

        if (game.timeRemaining == -1)
        {
            timerInitialized = true;

            Debug.Log(
                "[MULTI TIMER] UNTIMED MODE DETECTED"
            );
        }

        // -------------------------------
        // TIMED
        // -------------------------------

        else if (game.timeRemaining > 0f)
        {
            max_time = game.timeRemaining;

            timerInitialized = true;

        }

        else
        {
            return;
        }
    }

        // =========================================
        // GAME TIME UP
        // =========================================

        if (game.gameTimeUp)
        {
            if (!timeUp)
            {
                timeUp = true;

                Debug.Log(
                    "[CLIENT TIMER] GAME TIME UP RECEIVED"
                );
            }
            return;

        }

        // =========================================
        // GET SERVER TIME
        // =========================================

        time_remaining =
            game.timeRemaining;
            
        // =========================================
        // TIMER BAR
        // =========================================

        if (timer_linear_image != null &&
            max_time > 0f)
        {
            timer_linear_image.fillAmount =
                Mathf.Clamp01(
                    time_remaining / max_time
                );
        }

        // =========================================
        // WARNING COLOR
        // =========================================

        if (time_remaining <= 20f)
        {
            float t =
                1f - (time_remaining / 20f);

            timer_linear_image.color =
                Color.Lerp(
                    normalColor,
                    warningColor,
                    t
                );
        }
        else
        {
            timer_linear_image.color =
                normalColor;
        }

        digitalClock.text = ConvertTime(game.timeRemaining);

        // =========================================
        // WARNING ANIMATION
        // =========================================

        CheckTimeRemaining();
    }

    public void UpdateTimerModeUI()
    {
        if (MultiGameManager.Instance == null)
            return;

        bool isUntimed =
            MultiGameManager.Instance.timeRemaining == -1;

        if (timedUI != null && !timeUp)
            timedUI.SetActive(!isUntimed);

        if (untimedUI != null && !timeUp)
            untimedUI.SetActive(isUntimed);
    }

    public void HideScoreBoard()
    {
        timedUI.SetActive(false);
        untimedUI.SetActive(false);
    }

    public void PauseGame()
    {
        isPaused = true; 
    }

    public void ResumeGame()
    {
        isPaused = false;
    }

    public void HideAllGameElements()
    {
        {
                times_up_textholder.SetActive(true);
                scoreBar.SetActive(false);
                timerBar.SetActive(false);
                timerShake.enabled = false;
                pauseButton.SetActive(false);
                pausePanel.SetActive(false);

                titleImage.enabled = false;

                titleImage.transform.position -= new Vector3(0, 250f, 0); // Move title up to make room for "Time's Up" text
                titleImage.transform.localScale -= new Vector3(-3f, -3f, 0); // Enlarge title for emphasis
                if (QuizManager.Instance != null)
                {
                    QuizManager.Instance.HideAllButtons();
                }
                if (SynonymQuizManager.Instance != null)
                {
                    SynonymQuizManager.Instance.HideAllButtons();
                }
                if (AntonymQuizManager.Instance != null)
                {
                    AntonymQuizManager.Instance.HideAllButtons();
                }

            }
    }

    void HideTimer()
    {
        int destinationScene = PlayerPrefs.GetInt("DestinationScene", 0);
        if (destinationScene == 1)
        {
            timerBar.SetActive(false);
            timerShake.enabled = false;
            scoreBar.SetActive(true);
            //scoreBar.transform.position += new Vector3(-60, -10, 0);
            //scoreBar.transform.localScale += new Vector3(1.2f, 0.5f, 0); // Enlarge score bar to fill space left by timer
        }
        else if (destinationScene == 2 || destinationScene == 3)
        {
            timerBar.SetActive(false);
            timerShake.enabled = false;
            scoreBar.SetActive(true);
            //scoreBar.transform.position += new Vector3(-75, -20, 0);
            //scoreBar.transform.localScale += new Vector3(1.2f, 0.5f, 0);
        }
        
    }

    public void HideElements(){
        scoreBar.SetActive(false);
        timerBar.SetActive(false);
        timerShake.enabled = false;
        titleImage.enabled = false;
        pauseButton.SetActive(false);

        if(timeData == 2)
        {
            scoreBar.SetActive(false);
            timerShake.enabled = false;
        }
    }
    public void ShowElements(){
        timerBar.SetActive(true);
        timerShake.enabled = true;
        titleImage.enabled = true;
        pauseButton.SetActive(true);
        if (timeData == 2)
        {
            timerBar.SetActive(!timerBar.activeSelf);
            scoreBar.SetActive(true);
            timerShake.enabled = false;
        }
    }
    public void StopGame()
    {
        time_remaining = 0f;
        AudioManager.Instance.StopTimesUpSfx();
    }

    void CheckTimeRemaining()
    {
        if (time_remaining <= 10f &&
            animationPlayed == false &&
            timeData != 2)
        {
            TimeClosing();
        }
    }
    void TimeClosing()
    {
        isTimeClosing = true;
        animationPlayed = true;
        AudioManager.Instance.TickingSfx();
        StartCoroutine(TimerShake());
        StartCoroutine(ClockFlicker());
    }

    public void AnimateClockImage()
    {
        StartCoroutine(AnimateClockImageCoroutine());
    }

    IEnumerator AnimateClockImageCoroutine()
    {
        clockImageAnimated.gameObject.SetActive(true);
        clockImageStatic.gameObject.SetActive(false);

        // Wait for the animation to finish (assuming the animation is 1 second long)
        yield return new WaitForSeconds(animationDuration);

        clockImageAnimated.gameObject.SetActive(false);
        clockImageStatic.gameObject.SetActive(true);
    }

    IEnumerator TimerShake()
    {
        clockAnimator.enabled = isTimeClosing;
        yield return new WaitForSeconds(10f);
        clockAnimator.enabled = isTimeClosing;
        
    }

    IEnumerator InitializeTimerModeUI()
    {
        while (NetworkRoomSettings.Instance == null)
        {
            yield return null;
        }

        UpdateTimerModeUI();

        // timerModeInitialized = true;
    }

    public void HideObjects()
    {
        Debug.Log("UI Objects are now hidden");
        foreach (GameObject ui in gameplayUIToHide)
        {
            if (ui != null)
                ui.SetActive(false);
        }
    }

    public void PauseButton(GameObject panel)
    {
        if(panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }

    string ConvertTime(int timeVal)
    {
        int minutes = timeVal / 60;
        int seconds = timeVal % 60;

        string formattedTime = string.Format("{0:D2}:{1:D2}", minutes, seconds);

        return formattedTime;
    }


    IEnumerator ClockFlicker()
    {
        while(!timeUp)
        {
            isTinted = !isTinted;
            yield return new WaitForSeconds(0.5f);
            if(isTinted)
                digitalClock.color = Color.red;
            else
                digitalClock.color = Color.white;
        }
    }

}
