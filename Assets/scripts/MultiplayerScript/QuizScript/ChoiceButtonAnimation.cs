using UnityEngine;
using UnityEngine.UI;
public class ChoiceButtonAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayIncorrect(bool isCorrect)
    {

        
        if (animator == null)
            return;

        if (!gameObject.activeInHierarchy)
            return;

        animator.ResetTrigger("Shake");
        animator.ResetTrigger("CorrectTrigger");    

        if (isCorrect)
        {
            animator.SetTrigger("CorrectTrigger");
        }
        else
        {
            animator.SetTrigger("Shake");
        }
            
    }
}