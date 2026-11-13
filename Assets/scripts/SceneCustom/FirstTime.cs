using UnityEngine;

public class FirstTime : MonoBehaviour
{
    //string sceneName;
    int firstTimeMulti = 0;
    public GameObject showTutorial;
    

    void Start()
    {
        //sceneName = SceneManager.GetActiveScene().name;
        firstTimeMulti = PlayerPrefs.GetInt("FirstTimeMultiplayer", 1);
        CheckFirstTime();
    }

    void CheckFirstTime()
    {
        if(firstTimeMulti == 1 && PlayerPrefs.GetInt("ShowHidden", 0) == 1)
        {
            showTutorial.SetActive(true);
            PlayerPrefs.SetInt("FirstTimeMultiplayer", 0);
            PlayerPrefs.Save();
        }
    }
}
