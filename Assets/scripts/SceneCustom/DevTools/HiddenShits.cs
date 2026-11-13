using System.Collections.Generic;
using UnityEngine;

public class HiddenShits : MonoBehaviour
{

    public List<GameObject> hiddenStuff;
    public List<GameObject> activeStuff;

    public bool isEnabled = false;
    
    void Start()
    {
        isEnabled = PlayerPrefs.GetInt("ShowHidden", 0) == 1;
        UpdateUI(isEnabled);
    }

    public void UpdateUI(bool isTrue)
    {
        if (isTrue)
        {
            foreach (GameObject ui in hiddenStuff)
            {
                if(ui != null)
                {
                    ui.SetActive(true);
                }
            }

            foreach (GameObject ui in activeStuff)
            {
                if(ui != null)
                {
                    ui.SetActive(false);
                }
            }


        }
    }
}
