using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class PersistentEnvironment : MonoBehaviour
{
    public static PersistentEnvironment Instance;
    public Image backgroundImg;


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
