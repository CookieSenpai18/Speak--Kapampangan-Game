using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

public class AvatarCreation : MonoBehaviour
{
    [Header("Text Info")]
    public TMP_Text userName, age;



    [Header("Input Fields")]
    public TMP_InputField userNameInput;
    public TMP_InputField ageInput;

    [Header("Gender")]
    public Button genderButton;
    public Sprite[] genderSprites;

    [Header("Gender Text")]
    public TMP_Text genderText;

    [Header("Warning Text")]
    public TMP_Text warningText;

    [SerializeField] private LocalizedString maleTxt;
    [SerializeField] private LocalizedString femaleTxt;



    int currentGender = 0;


    private void Start()
    {
        int savedGender = PlayerPrefs.GetInt("Gender", 0);
        ToggleGender(savedGender);

        string savedName = PlayerPrefs.GetString("UserName", "Player");
        int savedAge = PlayerPrefs.GetInt("UserAge", 8);
        userName.text = savedName;
        if(age != null)
        {
            age.text = savedAge.ToString();
        }

        if (userNameInput != null)
        {
            userNameInput.text = savedName;
        }
        if (ageInput != null)
        {
            ageInput.text = savedAge.ToString();
        }


    }

    public void SaveProfile()
    {
        if (string.IsNullOrEmpty(userNameInput.text))
        {
            warningText.text = "Please enter a name.";
            return;
        }

        else if (!int.TryParse(ageInput.text, out int age))
        {
            warningText.text = "Please enter a valid age.";
            return;
        }

        else if (age <= 0 || age >= 150)
        {
            warningText.text = "Please enter an age between 1 and 149.";
            return;
        }

        else
        {
            warningText.text = "";
            PlayerPrefs.SetString("UserName", userNameInput.text);
            PlayerPrefs.SetInt("Gender", currentGender);
            PlayerPrefs.SetInt("UserAge", age);
            PlayerPrefs.Save();
            SaveAvatar();
            SlideShow.Instance.NextSlide();
            AvatarIcon.Instance.Start();
        }

        
    }

    public void ToggleGender(int genderIndex)
    {   currentGender = genderIndex; 
        if (genderButton != null)
        {
            if (genderIndex == 0)
            {
                genderText.text = maleTxt.GetLocalizedString();
            }
            else if (genderIndex == 1)
            {
                genderText.text = femaleTxt.GetLocalizedString();
            }
            
        }
    }

    public void SaveAvatar()
    {
        PlayerPrefs.SetString("UserName", userNameInput.text);
        PlayerPrefs.SetInt("Gender", currentGender);
        int parsedAge = 8;

        if (int.TryParse(ageInput.text, out parsedAge))
        {
            PlayerPrefs.SetInt("UserAge", parsedAge);
        }
        else
        {
            PlayerPrefs.SetInt("UserAge", 8);
        }
        PlayerPrefs.Save();
        Debug.Log("Avatar saved");
    }

}