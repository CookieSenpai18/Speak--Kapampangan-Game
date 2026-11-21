using System.Collections;
using TMPro;
using UnityEngine;
public class PerfStatDis : MonoBehaviour
{
    public float enumarationSpeed = 0.01f;
    public float pauseBeforeNext = 1f;
    public GameObject parentObject;
    public GameObject tapToConTxt;
    public TMP_Text scoreTxt;
    public TMP_Text streakTxt;
    public TMP_Text wrongTxt;
    public TMP_Text formulaTxt;
    public TMP_Text totalTxt;
        int score = 0;
        int streak = 0;
        int wrong = 0;
        int totalValue = 0;
        bool isFinished = false;
        int suffix = 4;
        string lang = "en";
        string scoreLang = "";
        string incorrectLang = "";
        string highStreakLang = "";
        string earningLang = "";
        string totalLang = "";

        string closeTxt = "";
    void Start()
    {
        lang = PlayerPrefs.GetString("Language", "en");
        if(lang == "en")
        {
            scoreLang = "Score: ";
            incorrectLang = "Incorrect: ";
            highStreakLang = "Highest streak: ";
            earningLang = "Earnings ";
            totalLang = "Total Earned: ";
            closeTxt = "Tap anywhere to continue.";
        }
        else if(lang == "kap")
        {
            scoreLang = "Puntus: ";
            incorrectLang = "Mali: ";
            highStreakLang = "Sunud-sunud: ";
            earningLang = "Penakitan: ";
            totalLang = "Penakitan: ";
            closeTxt = "Pumindut gang nukarin bang misara.";
        }

    }
    void HideObjects()
    {

        scoreTxt.gameObject.SetActive(false);
        streakTxt.gameObject.SetActive(false);
        wrongTxt.gameObject.SetActive(false);
        formulaTxt.gameObject.SetActive(true);
        totalTxt.gameObject.SetActive(false);
    }
    public void DisplayStatsMulti(int scoreA, int streakA, int wrongA)
    {
        suffix = 6;
        parentObject.SetActive(true);
        HideObjects();
        score = scoreA;
        wrong = wrongA;
        streak = streakA;
        totalValue = (int)((score * 0.3) - (wrong * 10) + (streak * 6)); 
        if(totalValue <= 0)
        {
            totalValue = 0;
        }
        StartCoroutine(StartCountSingle(suffix));
    }
    public void DisplayStatsSingle()
    {
        suffix = 4;
        parentObject.SetActive(true);
        HideObjects();
        var indu = FindAnyObjectByType<QuizManager>();
        var syno = FindAnyObjectByType<SynonymQuizManager>();
        var anto = FindAnyObjectByType<AntonymQuizManager>();
        if(indu != null)
        {
            score = indu.score;
            streak = indu.highestStreak;
            wrong = indu.wrongCount;
        }
        if(syno != null)
        {
            score = syno.score;
            streak = syno.highestStreak;
            wrong = syno.wrongCount;
        }
        if(anto != null)
        {
            score = anto.score;
            streak = anto.highestStreak;
            wrong = anto.wrongCount;
        }
        totalValue = (int)((score * 0.3) - (wrong * 10) + (streak * 4));
        if(totalValue <= 0)
        {
            totalValue = 0;
        }
        StartCoroutine(StartCountSingle(suffix));
    }
    IEnumerator StartCountSingle(int suffix)
    {
        yield return new WaitForSeconds(0.35f); 
        AudioManager.Instance.SlamSfx2();
        yield return new WaitForSeconds(0.6f); 
        scoreTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        AudioManager.Instance.SlamSfx1();
        yield return new WaitForSeconds(0.15f);
        formulaTxt.text = $"{earningLang} = (-- * 0.3) - (-- * 10) + (-- * {suffix})";
        yield return StartCoroutine(CountUp(scoreTxt, scoreLang, score));
        formulaTxt.text = $"{earningLang} = ({score} * 0.3) - (-- * 10) + (-- * {suffix})";
        yield return new WaitForSeconds(pauseBeforeNext);
        wrongTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        AudioManager.Instance.SlamSfx1();
        yield return StartCoroutine(CountUp(wrongTxt, incorrectLang, wrong));
        formulaTxt.text = $"{earningLang} = ({score} * 0.3) - ({wrong} * 10) + (-- * {suffix})";
        yield return new WaitForSeconds(pauseBeforeNext);
        streakTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        AudioManager.Instance.SlamSfx1();
        yield return StartCoroutine(CountUp(streakTxt, highStreakLang, streak));
        formulaTxt.text = $"{earningLang} = ({score} * 0.3) - ({wrong} * 10) + ({streak} * {suffix})";
        yield return new WaitForSeconds(pauseBeforeNext);
        totalTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        AudioManager.Instance.SlamSfx1();
        yield return StartCoroutine(CountUp(totalTxt, totalLang, totalValue));

        isFinished = true;
        tapToConTxt.SetActive(true);
    }
    public void SkipEnu()
    {
        if (isFinished)
        {
            var timer = FindAnyObjectByType<GameTimer>();
            if(timer != null)
            {
                timer.CheckHighest();
            }
            var timerMul = FindAnyObjectByType<MultiplayerStatsRecorder>();
            if(timerMul != null)
            {
                timerMul.CheckHighest();
            }
            ClosePanel();
        }
        else
            SkipCounting();
    }
    void SkipCounting()
    {
        isFinished = true;
        StopAllCoroutines();
        formulaTxt.text = $"{earningLang} = ({score} * 0.3) - ({wrong} * 10) + ({streak} * {suffix})";
        scoreTxt.text = scoreLang + score;
        wrongTxt.text = incorrectLang + wrong;
        streakTxt.text = highStreakLang + streak;
        totalTxt.text = totalLang + totalValue;
        scoreTxt.gameObject.SetActive(true);
        streakTxt.gameObject.SetActive(true);
        wrongTxt.gameObject.SetActive(true);
        formulaTxt.gameObject.SetActive(true);
        totalTxt.gameObject.SetActive(true);

        tapToConTxt.SetActive(true);
    }
    void ClosePanel()
    {
        parentObject.SetActive(false);
    }
    IEnumerator CountUp(TMP_Text text, string label, int target)
    {
        int current = 0;
        // Small values animate quickly; large values have a duration cap.
        float duration = Mathf.Clamp(
            target * enumarationSpeed,
            0.2f,
            5f
        );
        if (target <= 0)
        {
            text.text = label + "0";
            yield break;
        }
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            current = Mathf.RoundToInt(
                Mathf.Lerp(0, target, Mathf.Clamp01(elapsed / duration))
            );
            text.text = label + current;
            yield return null;
        }
        text.text = label + target;
    }
}
