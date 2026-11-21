using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Mirror.Examples.Pong;

public class CustomBackground : MonoBehaviour
{

    public Image backgroundDesign;
    public Sprite[] backgroundImages;

    public GameObject balloons;
    public static int selectedItem = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeStart();
    }

    public void InitializeStart()
    {
        balloons.SetActive(false);
        selectedItem = PlayerPrefs.GetInt("SelectedBackground", 0);
        string currentSceneName = SceneManager.GetActiveScene().name;

        if(currentSceneName != "Bootstrap" && currentSceneName != "BootIntro")
            UpdateBackground(selectedItem);  
    }
    public void UpdateBackground(int index)
    {
        if(index == 0)
        {
            backgroundDesign.gameObject.SetActive(false);
        }
        else
        {
            backgroundDesign.gameObject.SetActive(true);
            backgroundDesign.sprite = backgroundImages[index - 1];
        }

        if(index == 3)
            balloons.SetActive(true);
        else
            balloons.SetActive(false);


        PlayerPrefs.SetInt("SelectedBackground", index);
        PlayerPrefs.Save();
    }
}
