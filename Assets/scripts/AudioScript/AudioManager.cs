using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public float initialVolume = 0.5f;

    [Header("Music Volume Balance")]
    [SerializeField] private float mainMenuVolume = 0.6f;
    [SerializeField] private float gameplayVolume = 1f;

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource tickingSfxSource;


    [Header("SFX")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip wrongSound;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip tickingSound;
    [SerializeField] private AudioClip timesUpSound;

    [SerializeField] private AudioClip cardFlip1;
    [SerializeField] private AudioClip cardFlip2;

    [SerializeField] private AudioClip buySound;

    [Header("BGM Pause Delay")]
    [SerializeField] private float bgmPauseDelay = 1f;

    [Header("Music")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

        void Start()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.5f);

        SetMusicVolume(musicVolume);
        SetSfxVolume(sfxVolume);

        PlayMainMenuMusic();
    }

    // 🎵 MUSIC
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip)
            return;

        musicSource.Stop();

        musicSource.clip = clip;
        musicSource.loop = true;

        ApplyCurrentMusicVolume();

        musicSource.Play();
    }


    public void SetMusicVolume(float volume)
    {
        initialVolume = volume;
        ApplyCurrentMusicVolume();
}

    private void ApplyCurrentMusicVolume()
    {
        float finalVolume = initialVolume * initialVolume;

        if (musicSource.clip == mainMenuMusic)
        {
            finalVolume *= mainMenuVolume;
        }
        else if (musicSource.clip == gameplayMusic)
        {
            finalVolume *= gameplayVolume;
        }

        musicSource.volume = finalVolume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxSource.volume = volume * volume;
        tickingSfxSource.volume = volume * volume;
    }


        // 🎵 STOP MUSIC
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // ⏸ PAUSE MUSIC
    public void PauseMusic()
    {
        musicSource.Pause();
    }

    // ▶ RESUME MUSIC
    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    // MAIN MENU MUSIC
    public void PlayMainMenuMusic()
    {
        StartCoroutine(DelayMainMenuMusic());
    }
    private IEnumerator DelayMainMenuMusic()
    {
        musicSource.Stop();
        yield return new WaitForSeconds(bgmPauseDelay);
        PlayMusic(mainMenuMusic);
    }

    // GAMEPLAY MUSIC
    public void PlayGameplayMusic()
    {
        StartCoroutine(DelayGameplayMusic());
    }
    private IEnumerator DelayGameplayMusic()
    {
        musicSource.Stop();
        yield return new WaitForSeconds(bgmPauseDelay);
        PlayMusic(gameplayMusic);
    }



    // 🔊 SFX
    public void PlayClick()
    {
        if (clickSound == null) return;

        sfxSource.PlayOneShot(clickSound);
    }
    public void CorrectSfx()
    {
        if(correctSound == null) return;
        sfxSource.PlayOneShot(correctSound);
    }
    public void WrongSfx()
    {
        if(wrongSound == null) return;
        sfxSource.PlayOneShot(wrongSound);
    }
    public void TimesUpSfx()
    {
        if(timesUpSound == null) return;
        sfxSource.PlayOneShot(timesUpSound);
    }
    public void TickingSfx()
    {
        if(tickingSound == null) return;
        tickingSfxSource.PlayOneShot(tickingSound);
    }
    public void StopTimesUpSfx()
    {
        tickingSfxSource.Stop();
    }
    public void CardFlipSfx1()
    {
        if(cardFlip1 == null) return;
        sfxSource.PlayOneShot(cardFlip1);
    }
    public void CardFlipSfx2()
    {
        if(cardFlip2 == null) return;
        sfxSource.PlayOneShot(cardFlip2);
    }

    public void BuySfx()
    {
        if(buySound == null) return;
        sfxSource.PlayOneShot(buySound);
    }
}