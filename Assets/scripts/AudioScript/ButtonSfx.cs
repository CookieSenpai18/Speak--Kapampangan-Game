using UnityEngine;

public class ButtonSfx : MonoBehaviour
{
    public static ButtonSfx Instance;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip clickSound;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }
    public void PlayClickSound()
    {
        if (Instance == null)
        {
            Debug.LogError("ButtonSfx Instance missing");
            return;
        }

        if (sfxSource == null)
        {   
            Debug.LogError("SFX AudioSource missing");
            return;
        }

            Debug.Log("Instance: " + Instance);
            Debug.Log("SFX Source: " + sfxSource);
            Debug.Log("Clip: " + clickSound);

        sfxSource.PlayOneShot(clickSound);
        Debug.Log("SFX played from: " + Instance.name); 
    }
}