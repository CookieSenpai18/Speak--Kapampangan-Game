using TMPro;
using UnityEngine;
using UnityEngine.InputSystem; 

public class ToolTip : MonoBehaviour
{
    public GameObject toolTipPanel;
    public TMP_Text tooltipText;

    [Header("Settings")]
    public float displayTime = 3f;

    private float timer;
    private bool isShowing;


    void Start()
    {
        isShowing = false;
        toolTipPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShowing)
            return;

        // Hide after a certain amount of time
        timer -= Time.unscaledDeltaTime;

        if (timer <= 0)
        {
            Hide();
            return;
        }

        if (Keyboard.current != null &&
            Keyboard.current.anyKey.isPressed)
        {
            Hide();
            return;
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.isPressed)
        {
            Hide();
            return;
        }


        if (Mouse.current != null &&
            Mouse.current.rightButton.isPressed)
        {
            Hide();
            return;
        }

        if (Mouse.current != null &&
            Mouse.current.middleButton.isPressed)
        {
            Hide();
            return;
        }

        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed)
                {
                    Hide();
                    return;
                }
            }
        }       
    }

    public void Show(string message)
    {
        if (toolTipPanel == null || tooltipText == null)
            return;

        if(message != "")
        {
            tooltipText.text = message;
        }
        

        toolTipPanel.SetActive(true);

        timer = displayTime;
        isShowing = true;
    }

    void Hide()
    {
        toolTipPanel.SetActive(false);
        isShowing = false;
    }
}
