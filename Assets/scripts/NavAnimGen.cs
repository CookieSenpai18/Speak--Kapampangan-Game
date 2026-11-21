
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavAnimGen : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [SerializeField] private Animator animator;
    [SerializeField] private Button button;

    private bool isHovering;
    private bool isPressed;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        UpdateAnimator();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        UpdateAnimator();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        UpdateAnimator();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isPressed) return;

        isPressed = false;
        UpdateAnimator();

        if (isHovering && button.interactable)
        {
            button.onClick.Invoke();
        }
    }

    private void UpdateAnimator()
    {
        animator.SetBool("IsHovering", isHovering);
        animator.SetBool("IsPressed", isPressed);
    }
}
