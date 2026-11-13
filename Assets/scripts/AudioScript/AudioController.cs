using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        public Image bgmImage, sfxImage;

        public Sprite bgmActive, bgmInactive, sfxActive, sfxInactive;

    void Start()
    {
        // Load saved values
        bgmSlider.value = PlayerPrefs.GetFloat("MusicVolume", AudioManager.Instance.initialVolume);
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume", AudioManager.Instance.initialVolume);

        // Apply loaded values
        AudioManager.Instance.SetMusicVolume(bgmSlider.value);
        AudioManager.Instance.SetSfxVolume(sfxSlider.value);

        // Listen to slider changes
        bgmSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        AudioManager.Instance.SetMusicVolume(volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();

        if (volume > 0)
        {
            bgmImage.sprite = bgmActive;
        }
        else
        {
            bgmImage.sprite = bgmInactive;
        }

    }

    public void SetSfxVolume(float volume)
    {
        AudioManager.Instance.SetSfxVolume(volume);
        PlayerPrefs.SetFloat("SfxVolume", volume);
        PlayerPrefs.Save();

        if (volume > 0)
        {
            sfxImage.sprite = sfxActive;
        }
        else
        {
            sfxImage.sprite = sfxInactive;
        }
    }
    public void PlayClickSound()
    {
        AudioManager.Instance.PlayClick();
    }
}
