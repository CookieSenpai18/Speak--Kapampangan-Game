using UnityEngine;
using UnityEngine.SceneManagement;


public class BootIntroAnim : MonoBehaviour
{
    public int delayTransition = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(PlayIntro());
    }


    System.Collections.IEnumerator PlayIntro()
    {
        LoadingScreen.destinationScene = "MainGame";
        LoadingScreen.isMultiplayer = false;
        yield return new WaitForSeconds(delayTransition);
        SceneManager.LoadScene("LoadingScreen");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
