using TMPro;
using UnityEngine;

public class ProcessShopItems : MonoBehaviour
{
    public TMP_Text logDisplay;
    public TMP_Text moneyDisplay;
    public TMP_Text moneyDisplay1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateMoneyDisplay();
    }

    // Update is called once per frame
    public void ProcessBoughtItem(int index, int price)
    {
        var varObj = FindAnyObjectByType<CustomBackground>();
        if(varObj == null) return;

        int currentCash = PlayerPrefs.GetInt("Money", 0);
        PlayerPrefs.SetInt("Money", currentCash - price);
        PlayerPrefs.SetInt("Index" + index, 1);
        PlayerPrefs.SetInt("SelectedBackground", index);
        PlayerPrefs.Save();

        UpdateMoneyDisplay();
        logDisplay.text = "Item bought successfully.";

        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.BuySfx();
        }
    }

    public void UpdateMoneyDisplay()
    {
        if(moneyDisplay != null)
        moneyDisplay.text = PlayerPrefs.GetInt("Money", 0).ToString();
        if(moneyDisplay1 != null)
        moneyDisplay1.text = PlayerPrefs.GetInt("Money", 0).ToString();
    } 
}
