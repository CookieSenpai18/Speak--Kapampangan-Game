using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class AvatarIcon : MonoBehaviour
{
    public static AvatarIcon Instance;

    [Header("Avatar Cropper")]
    public AvatarCropper avatarCropper;

    [Header("Text Info")]
    public TMP_Text userName, age;

    [Header("Current Avatar Display")]
    public Image avatarImage;

    [Header("Preset Avatars")]
    public Sprite[] avatarSprites;

    [Header("Input Fields")]
    public TMP_InputField userNameInput;
    public TMP_InputField ageInput;

    [Header("Gender")]
    public Button genderButton;
    public Sprite[] genderSprites;
    int currentGender = 0;
    int currentIndex = 0;

    [Header("Custom Avatar")]
    public bool usingCustomAvatar = false;

    public Button customAvatarButton;
    public Sprite defaultButtonImage;

    public GameObject overlayImage;

    // Current custom image being previewed
    private Texture2D currentCustomTexture;
    private Sprite currentCustomSprite;

    // True if a custom image has been selected during this session
    private bool hasCustomSelection = false;

    public void Start()
    {
        LoadSavedAvatar();
        // int savedIndex = PlayerPrefs.GetInt("SelectedAvatar", 0);
        int savedGender = PlayerPrefs.GetInt("Gender", 0);  
        ToggleGender(savedGender);
        // currentIndex = savedIndex;
        // avatarImage.sprite = avatarSprites[savedIndex];


    usingCustomAvatar =
        PlayerPrefs.GetInt("UsingCustomAvatar", 0) == 1;

    if (usingCustomAvatar)
    {
        
        LoadCustomAvatar();
        
        if(overlayImage != null)
            overlayImage.SetActive(true);
    }
    else
    {
        int savedIndex =
            PlayerPrefs.GetInt("SelectedAvatar", 0);

        // Safety check
        if (savedIndex < 0 || savedIndex >= avatarSprites.Length)
        {
            savedIndex = 0;
        }

        currentIndex = savedIndex;
        if(avatarImage == null)
            return;
        avatarImage.sprite = avatarSprites[savedIndex];

        // Restore default image for custom button
        if (customAvatarButton != null)
        {
            customAvatarButton.image.sprite = defaultButtonImage;
        }
    }



        string savedName = PlayerPrefs.GetString("UserName", "Player");
        int savedAge = PlayerPrefs.GetInt("UserAge", 8);
        userName.text = savedName;
        if(age != null)
        {
            age.text = savedAge.ToString();
        }

        if (userNameInput != null)
        {
            userNameInput.text = savedName;
        }
        if (ageInput != null)
        {
            ageInput.text = savedAge.ToString();
        }


    }

    void Awake()
    {
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
            Instance = this;
    }


    public void ToggleGender(int genderIndex)
    {   currentGender = genderIndex; 
        if (genderButton != null)
        {
            if (genderIndex == 0)
            {
                genderButton.image.sprite = genderSprites[currentGender];
            }
            else if (genderIndex == 1)
            {
                genderButton.image.sprite = genderSprites[currentGender];
            }
            
        }
    }

    public void SelectAvatar(int index)
    {
        Debug.Log("Selecting preset avatar: " + index);

        if (avatarImage == null)
        {
            Debug.LogError("avatarImage is NULL");
            return;
        }

        if (index < 0 || index >= avatarSprites.Length)
        {
            Debug.LogError("Invalid avatar index: " + index);
            return;
        }

        currentIndex = index;
        usingCustomAvatar = false;

        currentCustomTexture = null;
        currentCustomSprite = null;
        hasCustomSelection = false;

        avatarImage.sprite = avatarSprites[index];

        if (customAvatarButton != null)
        {
            customAvatarButton.image.sprite = defaultButtonImage;
        }

        Debug.Log(
            "Preset selected: " + index +
            " | Custom = " + usingCustomAvatar
        );
    }
    
    public void SaveAvatar()
    {
        SceneManagerScript.isAvatarActive = false;
        // =========================================
        // SAVE AVATAR
        // =========================================

        if (usingCustomAvatar && currentCustomTexture != null)
        {
            SaveCustomAvatar(currentCustomTexture);
        }
        else
        {
            PlayerPrefs.SetInt("SelectedAvatar", currentIndex);
            PlayerPrefs.SetInt("UsingCustomAvatar", 0);
        }

        // =========================================
        // SAVE PROFILE INFORMATION
        // =========================================

        if (userNameInput != null)
        {
            if(userNameInput.text != "")
                PlayerPrefs.SetString("UserName", userNameInput.text);
            else
                PlayerPrefs.SetString("UserName", "Player");
        }

        PlayerPrefs.SetInt("Gender", currentGender);

        int parsedAge = 8;

        if (ageInput != null &&
            int.TryParse(ageInput.text, out parsedAge))
        {
            PlayerPrefs.SetInt("UserAge", parsedAge);
        }
        else
        {
            PlayerPrefs.SetInt("UserAge", 8);
        }

        PlayerPrefs.Save();

        Debug.Log(
            "SAVED → Custom: " +
            PlayerPrefs.GetInt("UsingCustomAvatar", 0) +
            " | Preset: " +
            PlayerPrefs.GetInt("SelectedAvatar", 0)
        );
    }


    public void LoadAvatar()
    {
        // =========================================
        // AVATAR
        // =========================================

        usingCustomAvatar =
            PlayerPrefs.GetInt("UsingCustomAvatar", 0) == 1;

        if (usingCustomAvatar)
        {
            LoadCustomAvatar();
        }
        else
        {
            int savedIndex =
                PlayerPrefs.GetInt("SelectedAvatar", 0);

            if (savedIndex < 0 || savedIndex >= avatarSprites.Length)
            {
                savedIndex = 0;
            }

            currentIndex = savedIndex;

            if (avatarImage != null)
            {
                avatarImage.sprite = avatarSprites[savedIndex];
            }

            if (customAvatarButton != null)
            {
                customAvatarButton.image.sprite = defaultButtonImage;
            }
        }

        // =========================================
        // NAME
        // =========================================

        string savedName =
            PlayerPrefs.GetString("UserName", "Player");

        if (userName != null)
        {
            userName.text = savedName;
        }

        if (userNameInput != null)
        {
            userNameInput.text = savedName;
        }

        // =========================================
        // AGE
        // =========================================

        int savedAge =
            PlayerPrefs.GetInt("UserAge", 8);

        if (age != null)
        {
            age.text = savedAge.ToString();
        }

        if (ageInput != null)
        {
            ageInput.text = savedAge.ToString();
        }

        // =========================================
        // GENDER
        // =========================================

        int savedGender =
            PlayerPrefs.GetInt("Gender", 0);

        if (savedGender < 0 || savedGender >= genderSprites.Length)
        {
            savedGender = 0;
        }

        ToggleGender(savedGender);

        Debug.Log("Profile loaded.");
    }

    public void SelectCustomAvatar()
    {
        Debug.Log("[AvatarIcon] Opening gallery...");

        NativeGallery.GetImageFromGallery((path) =>
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("[AvatarIcon] User cancelled gallery.");
                return;
            }

            Debug.Log(
                "[AvatarIcon] Image selected: " + path
            );

            Texture2D loadedTexture =
                NativeGallery.LoadImageAtPath(path, 1024);

            if (loadedTexture == null)
            {
                Debug.LogError(
                    "[AvatarIcon] Failed to load selected image."
                );

                return;
            }

            Debug.Log(
                "[AvatarIcon] Image loaded successfully: " +
                loadedTexture.width + "x" +
                loadedTexture.height
            );

            // Create a readable copy for cropping
            Texture2D readableTexture =
                MakeReadableTexture(loadedTexture);

            // We no longer need the original texture
            Destroy(loadedTexture);

            if (readableTexture == null)
            {
                Debug.LogError(
                    "[AvatarIcon] Failed to create readable texture."
                );

                return;
            }

            Debug.Log(
                "[AvatarIcon] Readable texture created: " +
                readableTexture.width + "x" +
                readableTexture.height
            );

            if (avatarCropper == null)
            {
                Debug.LogError(
                    "[AvatarIcon] AvatarCropper is NOT assigned!"
                );

                Destroy(readableTexture);
                return;
            }

            Debug.Log(
                "[AvatarIcon] Opening crop panel..."
            );

            avatarCropper.StartCrop(
                readableTexture,
                OnAvatarCropConfirmed
            );

        },
        "Select Profile Picture",
        "image/*");
    }

    private Texture2D MakeReadableTexture(Texture2D source)
    {
        if (source == null)
            return null;

        RenderTexture temporary =
            RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.ARGB32
            );

        Graphics.Blit(source, temporary);

        RenderTexture previous =
            RenderTexture.active;

        RenderTexture.active =
            temporary;

        Texture2D readable =
            new Texture2D(
                source.width,
                source.height,
                TextureFormat.RGBA32,
                false
            );

        readable.ReadPixels(
            new Rect(
                0,
                0,
                source.width,
                source.height
            ),
            0,
            0
        );

        readable.Apply();

        RenderTexture.active =
            previous;

        RenderTexture.ReleaseTemporary(
            temporary
        );

        return readable;
    }

    private void OnAvatarCropConfirmed(Texture2D croppedTexture)
{
    if (croppedTexture == null)
        return;

    currentCustomTexture = croppedTexture;

    currentCustomSprite = Sprite.Create(
        croppedTexture,
        new Rect(
            0,
            0,
            croppedTexture.width,
            croppedTexture.height
        ),
        new Vector2(0.5f, 0.5f)
    );

    usingCustomAvatar = true;
    hasCustomSelection = true;

    avatarImage.sprite = currentCustomSprite;

    if (customAvatarButton != null)
    {
        customAvatarButton.image.sprite =
            currentCustomSprite;
    }

    SaveCustomAvatar(croppedTexture);

    Debug.Log(
        "Cropped custom avatar selected and saved."
    );
}

    private void SaveCustomAvatar(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogError("Cannot save custom avatar: texture is NULL.");
            return;
        }

        string path = Path.Combine(
            Application.persistentDataPath,
            "custom_avatar.png"
        );

        try
        {
            // Create a readable copy of the texture
            RenderTexture renderTexture = RenderTexture.GetTemporary(
                texture.width,
                texture.height,
                0,
                RenderTextureFormat.ARGB32
            );

            Graphics.Blit(texture, renderTexture);

            RenderTexture previous =
                RenderTexture.active;

            RenderTexture.active =
                renderTexture;

            Texture2D readableTexture =
                new Texture2D(
                    texture.width,
                    texture.height,
                    TextureFormat.RGBA32,
                    false
                );

            readableTexture.ReadPixels(
                new Rect(
                    0,
                    0,
                    texture.width,
                    texture.height
                ),
                0,
                0
            );

            readableTexture.Apply();

            RenderTexture.active = previous;

            RenderTexture.ReleaseTemporary(
                renderTexture
            );

            // Now the texture is readable
            byte[] imageData =
                readableTexture.EncodeToPNG();

            if (imageData == null ||
                imageData.Length == 0)
            {
                Debug.LogError(
                    "Failed to encode custom avatar."
                );

                Destroy(readableTexture);
                return;
            }

            File.WriteAllBytes(
                path,
                imageData
            );

            // Verify the file exists
            if (!File.Exists(path))
            {
                Debug.LogError(
                    "Custom avatar file was not created!"
                );

                Destroy(readableTexture);
                return;
            }

            // Only mark as custom after successful save
            PlayerPrefs.SetInt(
                "UsingCustomAvatar",
                1
            );

            PlayerPrefs.Save();

            Debug.Log(
                "CUSTOM AVATAR SAVED SUCCESSFULLY\n" +
                "Path: " + path + "\n" +
                "Size: " + imageData.Length + " bytes"
            );

            Destroy(readableTexture);
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "Failed to save custom avatar: " +
                e.Message
            );
        }
        if(overlayImage != null)
        overlayImage.SetActive(true);
    }
    private void LoadCustomAvatar()
    {
        string path = Path.Combine(
            Application.persistentDataPath,
            "custom_avatar.png"
        );

        if (!File.Exists(path))
        {
            Debug.LogWarning(
                "Custom avatar file does not exist. Resetting avatar."
            );

            usingCustomAvatar = false;

            PlayerPrefs.SetInt(
                "UsingCustomAvatar",
                0
            );

            PlayerPrefs.SetInt(
                "SelectedAvatar",
                0
            );

            PlayerPrefs.Save();

            currentIndex = 0;

            avatarImage.sprite =
                avatarSprites[0];

            if (customAvatarButton != null)
            {
                customAvatarButton.image.sprite =
                    defaultButtonImage;
            }

            return;
        }

        byte[] imageData =
            File.ReadAllBytes(path);

        Texture2D texture =
            new Texture2D(2, 2);

        if (!texture.LoadImage(imageData))
        {
            Debug.LogError(
                "Failed to load custom avatar."
            );

            return;
        }

        Sprite customSprite =
            Sprite.Create(
                texture,
                new Rect(
                    0,
                    0,
                    texture.width,
                    texture.height
                ),
                new Vector2(0.5f, 0.5f)
            );

        currentCustomTexture = texture;
        currentCustomSprite = customSprite;
        usingCustomAvatar = true;

        avatarImage.sprite = customSprite;

        if (customAvatarButton != null)
        {
            customAvatarButton.image.sprite =
                customSprite;
        }

        Debug.Log(
            "Custom avatar loaded."
        );
    }

    private void LoadSavedAvatar()
    {
        usingCustomAvatar =
            PlayerPrefs.GetInt("UsingCustomAvatar", 0) == 1;

        if (usingCustomAvatar)
        {
            LoadCustomAvatar();
            return;
        }

        int savedIndex =
            PlayerPrefs.GetInt("SelectedAvatar", 0);

        if (savedIndex < 0 || savedIndex >= avatarSprites.Length)
        {
            savedIndex = 0;
        }

        currentIndex = savedIndex;

        if(avatarImage == null)
            return;

        avatarImage.sprite = avatarSprites[savedIndex];

        if (customAvatarButton != null)
        {
            customAvatarButton.image.sprite = defaultButtonImage;
        }
    }

    public void CheckSavedAvatar()
    {
        int custom =
            PlayerPrefs.GetInt("UsingCustomAvatar", 0);

        int preset =
            PlayerPrefs.GetInt("SelectedAvatar", 0);

        Debug.Log(
            "UsingCustomAvatar = " + custom +
            " | SelectedAvatar = " + preset
        );
    }
}