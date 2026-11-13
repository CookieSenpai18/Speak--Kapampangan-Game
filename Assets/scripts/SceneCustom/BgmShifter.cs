using UnityEngine;

public class BgmShifter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SwapBgm()
    {
        AudioManager.Instance.PlayMainMenuMusic();
    }
    
    public void StopBgm()
    {
        AudioManager.Instance.StopMusic();
    }

}
