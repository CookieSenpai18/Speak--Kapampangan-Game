using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class ItemShopBtn : MonoBehaviour
{
    [SerializeField] private GameObject lockedDisplay;
    [SerializeField] private int itemIndex;

    [SerializeField] private int price = 100;
    [SerializeField] private TMP_Text priceDisplay;

    private static ItemShopBtn[] allButtons;
    string lang = "en";
    string eqp = "Equip";
    string eqpd = "Eqquipped";
    

    private void Awake()
    {
        allButtons = FindObjectsByType<ItemShopBtn>(
            FindObjectsInactive.Include
        );
    }

    private void Start()
    {
        UpdateLockDisplay();

        lang = PlayerPrefs.GetString("Language", "en");

        if(lang == "en")
        {
            eqp = "Equip";
            eqpd = "Eqquipped";
        }
        else if(lang == "fil")
        {
            eqp = "Gamitin";
            eqpd = "Ginagamit";
        }
        else if(lang == "kap")
        {
            eqp = "Gamitan";
            eqpd = "Makagamit";
        }

    }

    public void UpdateLockDisplay()
    {
        bool purchased =
            PlayerPrefs.GetInt("Index" + itemIndex, 0) == 1;

        bool equipped =
            PlayerPrefs.GetInt("SelectedBackground", 0) == itemIndex;

        if (lockedDisplay != null)
        {
            lockedDisplay.SetActive(!purchased);
        }

        if (priceDisplay == null)
            return;

        if (!purchased)
        {
            priceDisplay.text = "₱ " + price.ToString("N0");
        }
        else if (equipped)
        {
            priceDisplay.text = eqpd; 
        }
        else
        {
            priceDisplay.text = eqp;
        }
    }

    public void PurchaseItem()
    {
        ProcessShopItems processShop =
            FindAnyObjectByType<ProcessShopItems>();

        if (processShop == null)
            return;

        CustomBackground customBackground =
            FindAnyObjectByType<CustomBackground>();

        if (customBackground == null)
            return;

        int cash = PlayerPrefs.GetInt("Money", 0);

        // Already purchased
        if (PlayerPrefs.GetInt("Index" + itemIndex, 0) == 1)
        {
            customBackground.UpdateBackground(itemIndex);

            PlayerPrefs.SetInt("SelectedBackground", itemIndex);
            PlayerPrefs.Save();

            processShop.logDisplay.text =
                "Background successfully equipped.";

            RefreshAllShopButtons();

            return;
        }

        // Not enough money
        if (cash < price)
        {
            processShop.logDisplay.text =
                "Not Enough Money.";

            return;
        }

        // Purchase
        processShop.ProcessBoughtItem(itemIndex, price);

        customBackground.UpdateBackground(itemIndex);

        PlayerPrefs.SetInt("SelectedBackground", itemIndex);
        PlayerPrefs.Save();

        RefreshAllShopButtons();
    }

    private void RefreshAllShopButtons()
    {
        // Get all ItemShopBtn objects
        allButtons = FindObjectsByType<ItemShopBtn>(
            FindObjectsInactive.Include
        );

        foreach (ItemShopBtn button in allButtons)
        {
            button.UpdateLockDisplay();
        }
    }

    public void SetDefault()
    {
        ProcessShopItems processShop =
            FindAnyObjectByType<ProcessShopItems>();

        if (processShop == null)
            return;

        CustomBackground customBackground =
            FindAnyObjectByType<CustomBackground>();

        if (customBackground == null)
            return;

        customBackground.UpdateBackground(0);

        PlayerPrefs.SetInt("SelectedBackground", 0);
        PlayerPrefs.Save();

        processShop.logDisplay.text =
            "Background was set to default";

        RefreshAllShopButtons();
    }
}
