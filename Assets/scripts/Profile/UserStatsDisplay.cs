using UnityEngine;
using TMPro;


public class UserStatsDisplay : MonoBehaviour
{
    public GameObject statsPanel;
    [Header("Single Indu")]
    public TMP_Text induEasy;
    public TMP_Text induMedium;
    public TMP_Text induHard;
    public TMP_Text induTotalTxt;
    public TMP_Text induHr;
    public TMP_Text induHs;
    public TMP_Text induHScore;
    public TMP_Text induLongRun;


    [Header("Single Syno")]
    public TMP_Text synEasy;
    public TMP_Text synMedium;
    public TMP_Text synHard;
    public TMP_Text synTotalTxt;
    public TMP_Text synHr;
    public TMP_Text synHs;
    public TMP_Text synHScore;
    public TMP_Text synLongRun;


    [Header("Single Anto")]
    public TMP_Text antEasy;
    public TMP_Text antMedium;
    public TMP_Text antHard;
    public TMP_Text antTotalTxt;
    public TMP_Text antHr;
    public TMP_Text antHs;
    public TMP_Text antHScore;
    public TMP_Text antLongRun;

    [Header("Single Stats")]
    public TMP_Text aveSpd;
    public TMP_Text ansStyle;
    public TMP_Text aveTooltip;
    public TMP_Text ansTooltip;




    // Multiplayer
    [Header("Multiplayer Indu")]
    public TMP_Text minduEasy;
    // public TMP_Text minduMedium;
    public TMP_Text minduHard;
    public TMP_Text minduTotalTxt;
    public TMP_Text minduHr;
    public TMP_Text minduHs;
    public TMP_Text minduHScore;
    public TMP_Text minduLongRun;


    [Header("Multiplayer Syno")]
    public TMP_Text msynEasy;
    public TMP_Text msynMedium;
    public TMP_Text msynHard;
    public TMP_Text msynTotalTxt;
    public TMP_Text msynHr;
    public TMP_Text msynHs;
    public TMP_Text msynHScore;
    public TMP_Text msynLongRun;


    [Header("Multiplayer Anto")]
    public TMP_Text mantEasy;
    public TMP_Text mantMedium;
    public TMP_Text mantHard;
    public TMP_Text mantTotalTxt;
    public TMP_Text mantHr;
    public TMP_Text mantHs;
    public TMP_Text mantHScore;
    public TMP_Text mantLongRun;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadSingleStats();
        LoadMultiplayerStats();
    }

    // Update is called once per frame
    void LoadSingleStats()
    {
        int induTotal, synoTotal, antoTotal;

        induEasy.text = PlayerPrefs.GetInt("InduEasy", 0).ToString();
        induMedium.text = PlayerPrefs.GetInt("InduMedium", 0).ToString();
        induHard.text = PlayerPrefs.GetInt("InduHard", 0).ToString();
        induHr.text = PlayerPrefs.GetInt("InduHR", 0).ToString();
        induHs.text = PlayerPrefs.GetInt("InduHS", 0).ToString();
        induHScore.text = PlayerPrefs.GetInt("InduHScore", 0).ToString();
        induLongRun.text = ConvertTime(PlayerPrefs.GetInt("InduLongRun", 0));


        synEasy.text = PlayerPrefs.GetInt("SynoEasy", 0).ToString();
        synMedium.text = PlayerPrefs.GetInt("SynoMedium", 0).ToString();
        synHard.text = PlayerPrefs.GetInt("SynoHard", 0).ToString();
        synHr.text = PlayerPrefs.GetInt("SynoHR", 0).ToString();
        synHs.text = PlayerPrefs.GetInt("SynoHS", 0).ToString();
        synHScore.text = PlayerPrefs.GetInt("SynoHScore", 0).ToString();
        synLongRun.text = ConvertTime(PlayerPrefs.GetInt("SynoLongRun", 0));

        antEasy.text = PlayerPrefs.GetInt("AntoEasy", 0).ToString();
        antMedium.text = PlayerPrefs.GetInt("AntoMedium", 0).ToString();
        antHard.text = PlayerPrefs.GetInt("AntoHard", 0).ToString();
        antHr.text = PlayerPrefs.GetInt("AntoHR", 0).ToString();
        antHs.text = PlayerPrefs.GetInt("AntoHS", 0).ToString();
        antHScore.text = PlayerPrefs.GetInt("AntoHScore", 0).ToString();
        antLongRun.text = ConvertTime(PlayerPrefs.GetInt("AntoLongRun", 0));


        induTotal = PlayerPrefs.GetInt("InduEasy", 0) + PlayerPrefs.GetInt("InduMedium", 0) + PlayerPrefs.GetInt("InduHard", 0);
        synoTotal = PlayerPrefs.GetInt("SynoEasy", 0) + PlayerPrefs.GetInt("SynoMedium", 0) + PlayerPrefs.GetInt("SynoHard", 0);
        antoTotal = PlayerPrefs.GetInt("AntoEasy", 0) + PlayerPrefs.GetInt("AntoMedium", 0) + PlayerPrefs.GetInt("AntoHard", 0);


        induTotalTxt.text = induTotal.ToString();
        synTotalTxt.text = synoTotal.ToString();
        antTotalTxt.text = antoTotal.ToString();

        if(induTotal + synoTotal + antoTotal >= 8)
        {
            aveSpd.text = "Average speed: " + PlayerPrefs.GetFloat("AverageSpeed", 2.36f).ToString() + "s";
            aveTooltip.text = "Average time to complete a single round.";


            float gTend = PlayerPrefs.GetFloat("GuessingTendency", 0f);
            string styleTxt;

            if (gTend <= 20f)
            {
                styleTxt = "Steady";
                ansTooltip.text = "STEADY: Takes a generally measured approach when answering.";
            }
            else if (gTend <= 40f)
            {
                styleTxt = "Explorative";
                ansTooltip.text = "EXPLORATIVE: Often explores possible answers during gameplay.";
            }
            else if (gTend <= 60f)
            {
                styleTxt = "Dynamic";
                ansTooltip.text = "DYNAMIC: Uses a mixed approach when responding to questions.";
            }
            else if (gTend <= 80f)
            {
                styleTxt = "Fast-Paced";
                ansTooltip.text = "FAST-PACED: Frequently makes rapid answer decisions.";
            }
            else if (gTend <= 100f)
            {
                styleTxt = "Spontaneous";
                ansTooltip.text = "SPONTANEOUS: Often responds with immediate answer selections.";
            }
            else
            {
                styleTxt = "Invalid Range";
                ansTooltip.text = "Invalid Range: Guessing tendency value is out of expected bounds.";
            }


            ansStyle.text = "Answering style: " + styleTxt;
        }
        else
        {
            aveSpd.text = "Average speed: Not enough data";
            ansStyle.text = "Answering style: Not enough data";

            aveTooltip.text = "Play more games to get data";
            ansTooltip.text = "Play more games to get data";
        }      
    }

    void LoadMultiplayerStats()
    {
        int induTotal, synoTotal, antoTotal;

        minduEasy.text = PlayerPrefs.GetInt("MInduEasy", 0).ToString();
        // minduMedium.text = PlayerPrefs.GetInt("InduMedium", 0).ToString();
        minduHard.text = PlayerPrefs.GetInt("MInduHard", 0).ToString();
        minduHr.text = PlayerPrefs.GetInt("MInduHR", 0).ToString();
        minduHs.text = PlayerPrefs.GetInt("MInduHS", 0).ToString();
        minduHScore.text = PlayerPrefs.GetInt("MInduHScore", 0).ToString();
        minduLongRun.text = ConvertTime(PlayerPrefs.GetInt("MInduLongRun", 0));

        msynEasy.text = PlayerPrefs.GetInt("MSynoEasy", 0).ToString();
        msynMedium.text = PlayerPrefs.GetInt("MSynoMedium", 0).ToString();
        msynHard.text = PlayerPrefs.GetInt("MSynoHard", 0).ToString();
        msynHr.text = PlayerPrefs.GetInt("MSynoHR", 0).ToString();
        msynHs.text = PlayerPrefs.GetInt("MSynoHS", 0).ToString();
        msynHScore.text = PlayerPrefs.GetInt("MSynoHScore", 0).ToString();
        msynLongRun.text = ConvertTime(PlayerPrefs.GetInt("MSynoLongRun", 0));

        mantEasy.text = PlayerPrefs.GetInt("MAntoEasy", 0).ToString();
        mantMedium.text = PlayerPrefs.GetInt("MAntoMedium", 0).ToString();
        mantHard.text = PlayerPrefs.GetInt("MAntoHard", 0).ToString();
        mantHr.text = PlayerPrefs.GetInt("MAntoHR", 0).ToString();
        mantHs.text = PlayerPrefs.GetInt("MAntoHS", 0).ToString();
        mantHScore.text = PlayerPrefs.GetInt("MAntoHScore", 0).ToString();
        mantLongRun.text = ConvertTime(PlayerPrefs.GetInt("MAntoLongRun", 0));


        induTotal = PlayerPrefs.GetInt("MInduEasy", 0) + PlayerPrefs.GetInt("MInduHard", 0);
        synoTotal = PlayerPrefs.GetInt("MSynoEasy", 0) + PlayerPrefs.GetInt("MSynoMedium", 0) + PlayerPrefs.GetInt("MSynoHard", 0);
        antoTotal = PlayerPrefs.GetInt("MAntoEasy", 0) + PlayerPrefs.GetInt("MAntoMedium", 0) + PlayerPrefs.GetInt("MAntoHard", 0);


        minduTotalTxt.text = induTotal.ToString();
        msynTotalTxt.text = synoTotal.ToString();
        mantTotalTxt.text = antoTotal.ToString();
    }

    string ConvertTime(int timeVal)
    {
        int minutes = timeVal / 60;
        int seconds = timeVal % 60;

        string formattedTime = string.Format("{0:D2}:{1:D2}", minutes, seconds);

        return formattedTime;
    }
}
