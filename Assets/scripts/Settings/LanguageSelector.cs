using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageSelector : MonoBehaviour
{
    string lang = "en";

    void Start()
    {
        lang = PlayerPrefs.GetString("Language", "en");
        SetLanguage(lang);
    }

    public void SetEnglish()
    {
        SetLanguage("en");
        PlayerPrefs.SetString("Language", "en");
        PlayerPrefs.Save();
    }

    public void SetFilipino()
    {
        SetLanguage("fil");
        PlayerPrefs.SetString("Language", "fil");
        PlayerPrefs.Save();
    }

    public void SetKapampangan()
    {
        SetLanguage("kap");
        PlayerPrefs.SetString("Language", "kap");
        PlayerPrefs.Save();
    }

    private void SetLanguage(string localeCode)
    {
        var locale =
            LocalizationSettings.AvailableLocales.GetLocale(
                localeCode
            );

        if (locale == null)
        {
            Debug.LogError(
                $"Locale '{localeCode}' was not found."
            );

            return;
        }

        LocalizationSettings.SelectedLocale =
            locale;
    }
}
