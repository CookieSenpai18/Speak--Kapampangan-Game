using UnityEngine;
using UnityEngine.InputSystem; 
public class HideGesture : MonoBehaviour
{
    public GameObject hideObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(hideObject != null && hideObject.activeSelf)
        {
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
    }

    void Hide()
    {
        hideObject.SetActive(false);
    }
}
