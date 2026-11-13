using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCharacterManager : MonoBehaviour
{
    // ============================================================
    // CHARACTER GROUP
    // ============================================================

    [Serializable]
    public class CharacterGroup
    {
        [Tooltip("Name shown in the UI.")]
        public string displayName;

        [Tooltip("Path relative to Assets/Resources/.")]
        public string resourcesFolder;

        [HideInInspector]
        public List<TutorialCharacter> characters =
            new List<TutorialCharacter>();
    }


    // ============================================================
    // UI REFERENCES
    // ============================================================

    [Header("Group Display")]
    [SerializeField] private TMP_Text groupTitle;


    [Header("Character ScrollView")]
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private TutorialCharacterButton buttonPrefab;


    [Header("Character Navigation")]
    [SerializeField] private Button previousCharacterButton;
    [SerializeField] private Button nextCharacterButton;


    [Header("Group Navigation")]
    [SerializeField] private Button previousGroupButton;
    [SerializeField] private Button nextGroupButton;


    [Header("Reset")]
    [SerializeField] private Button resetButton;


    [Header("Tracing")]
    [SerializeField] private CharacterTracingController tracingController;


    // ============================================================
    // GROUPS
    // ============================================================

    [Header("Character Groups")]
    [SerializeField]
    private List<CharacterGroup> groups =
        new List<CharacterGroup>();


    // ============================================================
    // INTERNAL STATE
    // ============================================================

    private int currentGroupIndex = 0;

    private int currentCharacterIndex = -1;

    private readonly List<TutorialCharacterButton>
        characterButtons =
            new List<TutorialCharacterButton>();


    // ============================================================
    // CURRENT GROUP / CHARACTER
    // ============================================================

    private CharacterGroup CurrentGroup
    {
        get
        {
            if (groups.Count == 0)
                return null;

            if (currentGroupIndex < 0 ||
                currentGroupIndex >= groups.Count)
            {
                return null;
            }

            return groups[currentGroupIndex];
        }
    }


    private TutorialCharacter CurrentCharacter
    {
        get
        {
            CharacterGroup group = CurrentGroup;

            if (group == null)
                return null;

            if (currentCharacterIndex < 0 ||
                currentCharacterIndex >= group.characters.Count)
            {
                return null;
            }

            return group.characters[currentCharacterIndex];
        }
    }


    // ============================================================
    // UNITY
    // ============================================================

    private void Awake()
    {
        SetupNavigationButtons();

        LoadAllGroups();

        if (groups.Count > 0)
        {
            LoadGroup(0);
        }
    }


    // ============================================================
    // LOAD ALL GROUPS
    // ============================================================

    private void LoadAllGroups()
    {
        foreach (CharacterGroup group in groups)
        {
            LoadCharactersFromFolder(group);

            SortCharacters(group);
        }
    }


    // ============================================================
    // LOAD CHARACTERS FROM A FOLDER
    // ============================================================

    private void LoadCharactersFromFolder(
        CharacterGroup group)
    {
        group.characters.Clear();

        if (string.IsNullOrWhiteSpace(
                group.resourcesFolder))
        {
            Debug.LogWarning(
                $"Character group '{group.displayName}' " +
                "has no Resources folder."
            );

            return;
        }

        Sprite[] sprites =
            Resources.LoadAll<Sprite>(
                group.resourcesFolder
            );

        if (sprites.Length == 0)
        {
            Debug.LogWarning(
                $"No sprites found in Resources/" +
                $"{group.resourcesFolder}"
            );

            return;
        }


        Dictionary<string, Sprite> backSprites =
            new Dictionary<string, Sprite>(
                StringComparer.OrdinalIgnoreCase
            );


        Dictionary<string, Sprite> frontSprites =
            new Dictionary<string, Sprite>(
                StringComparer.OrdinalIgnoreCase
            );


        foreach (Sprite sprite in sprites)
        {
            if (sprite == null)
                continue;

            string spriteName = sprite.name;


            // ----------------------------
            // BACK
            // ----------------------------

            if (spriteName.EndsWith(
                    "_back",
                    StringComparison.OrdinalIgnoreCase))
            {
                string characterName =
                    spriteName.Substring(
                        0,
                        spriteName.Length - "_back".Length
                    );

                backSprites[characterName] = sprite;
            }


            // ----------------------------
            // FRONT
            // ----------------------------

            else if (spriteName.EndsWith(
                         "_front",
                         StringComparison.OrdinalIgnoreCase))
            {
                string characterName =
                    spriteName.Substring(
                        0,
                        spriteName.Length - "_front".Length
                    );

                frontSprites[characterName] = sprite;
            }
        }


        // ========================================================
        // PAIR BACK + FRONT
        // ========================================================

        foreach (var pair in backSprites)
        {
            string characterName = pair.Key;

            Sprite backSprite = pair.Value;


            if (!frontSprites.TryGetValue(
                    characterName,
                    out Sprite frontSprite))
            {
                Debug.LogWarning(
                    $"Missing front sprite for " +
                    $"'{characterName}' in " +
                    $"Resources/{group.resourcesFolder}"
                );

                continue;
            }


            TutorialCharacter character =
                new TutorialCharacter(
                    characterName,
                    backSprite,
                    frontSprite
                );


            group.characters.Add(character);
        }


        Debug.Log(
            $"{group.displayName}: " +
            $"Loaded {group.characters.Count} characters."
        );
    }


    // ============================================================
    // SORT CHARACTERS
    // ============================================================

    private void SortCharacters(
        CharacterGroup group)
    {
        group.characters.Sort(
            (a, b) =>
                string.Compare(
                    a.characterName,
                    b.characterName,
                    StringComparison.OrdinalIgnoreCase
                )
        );
    }


    // ============================================================
    // BUTTON SETUP
    // ============================================================

    private void SetupNavigationButtons()
    {
        // Character Previous

        if (previousCharacterButton != null)
        {
            previousCharacterButton.onClick.RemoveAllListeners();

            previousCharacterButton.onClick.AddListener(
                PreviousCharacter
            );
        }


        // Character Next

        if (nextCharacterButton != null)
        {
            nextCharacterButton.onClick.RemoveAllListeners();

            nextCharacterButton.onClick.AddListener(
                NextCharacter
            );
        }


        // Group Previous

        if (previousGroupButton != null)
        {
            previousGroupButton.onClick.RemoveAllListeners();

            previousGroupButton.onClick.AddListener(
                PreviousGroup
            );
        }


        // Group Next

        if (nextGroupButton != null)
        {
            nextGroupButton.onClick.RemoveAllListeners();

            nextGroupButton.onClick.AddListener(
                NextGroup
            );
        }


        // Reset

        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners();

            resetButton.onClick.AddListener(
                ResetCurrentCharacter
            );
        }
    }


    // ============================================================
    // LOAD GROUP
    // ============================================================

    private void LoadGroup(int groupIndex)
    {
        if (groups.Count == 0)
            return;


        if (groupIndex < 0 ||
            groupIndex >= groups.Count)
        {
            return;
        }


        currentGroupIndex = groupIndex;


        CharacterGroup group =
            groups[currentGroupIndex];


        // ========================================================
        // UPDATE GROUP TITLE
        // ========================================================

        if (groupTitle != null)
        {
            groupTitle.text =
                group.displayName;
        }


        // ========================================================
        // REBUILD CHARACTER SCROLLVIEW
        // ========================================================

        CreateCharacterButtons(group);


        // ========================================================
        // SELECT FIRST CHARACTER
        // ========================================================

        if (group.characters.Count > 0)
        {
            SelectCharacter(0);
        }
        else
        {
            currentCharacterIndex = -1;
        }


        Debug.Log(
            $"Current group: {group.displayName}"
        );
    }


    // ============================================================
    // CREATE CHARACTER BUTTONS
    // ============================================================

    private void CreateCharacterButtons(
        CharacterGroup group)
    {
        if (buttonContainer == null)
        {
            Debug.LogError(
                "TutorialCharacterManager: " +
                "Button Container is missing."
            );

            return;
        }


        if (buttonPrefab == null)
        {
            Debug.LogError(
                "TutorialCharacterManager: " +
                "Button Prefab is missing."
            );

            return;
        }


        // ========================================================
        // CLEAR OLD BUTTON REFERENCES
        // ========================================================

        characterButtons.Clear();


        // ========================================================
        // DESTROY OLD BUTTONS
        // ========================================================

        for (int i = buttonContainer.childCount - 1;
             i >= 0;
             i--)
        {
            Destroy(
                buttonContainer.GetChild(i).gameObject
            );
        }


        // ========================================================
        // CREATE NEW BUTTONS
        // ========================================================

        for (int i = 0;
             i < group.characters.Count;
             i++)
        {
            TutorialCharacter character =
                group.characters[i];


            TutorialCharacterButton button =
                Instantiate(
                    buttonPrefab,
                    buttonContainer
                );


            button.Setup(
                this,
                character,
                i
            );


            characterButtons.Add(button);
        }


        // ========================================================
        // RESET SCROLL POSITION
        // ========================================================

        ResetScrollViewPosition();
    }


    // ============================================================
    // RESET SCROLLVIEW POSITION
    // ============================================================

    private void ResetScrollViewPosition()
    {
        ScrollRect scrollRect =
            buttonContainer.GetComponentInParent<ScrollRect>();


        if (scrollRect != null)
        {
            scrollRect.horizontalNormalizedPosition = 0f;
        }
    }


    // ============================================================
    // SELECT CHARACTER
    // ============================================================

    public void SelectCharacter(int index)
    {
        CharacterGroup group =
            CurrentGroup;


        if (group == null)
            return;


        if (index < 0 ||
            index >= group.characters.Count)
        {
            return;
        }


        currentCharacterIndex = index;


        TutorialCharacter character =
            group.characters[
                currentCharacterIndex
            ];


        // ========================================================
        // START / RESET TRACING
        // ========================================================

        if (tracingController != null)
        {
            tracingController.BeginTrace(
                character.backSprite,
                character.frontSprite
            );
        }


        // ========================================================
        // UPDATE SELECTED BUTTON
        // ========================================================

        UpdateCharacterButtonSelection();


        Debug.Log(
            $"Selected: " +
            $"{group.displayName} / " +
            $"{character.characterName}"
        );
    }


    // ============================================================
    // UPDATE BUTTON SELECTION
    // ============================================================

    private void UpdateCharacterButtonSelection()
    {
        for (int i = 0;
             i < characterButtons.Count;
             i++)
        {
            characterButtons[i].SetSelected(
                i == currentCharacterIndex
            );
        }
    }


    // ============================================================
    // NEXT CHARACTER
    // ============================================================

    public void NextCharacter()
    {
        CharacterGroup group =
            CurrentGroup;


        if (group == null ||
            group.characters.Count == 0)
        {
            return;
        }


        int nextIndex =
            currentCharacterIndex + 1;


        if (nextIndex >= group.characters.Count)
        {
            nextIndex = 0;
        }


        SelectCharacter(nextIndex);
    }


    // ============================================================
    // PREVIOUS CHARACTER
    // ============================================================

    public void PreviousCharacter()
    {
        CharacterGroup group =
            CurrentGroup;


        if (group == null ||
            group.characters.Count == 0)
        {
            return;
        }


        int previousIndex =
            currentCharacterIndex - 1;


        if (previousIndex < 0)
        {
            previousIndex =
                group.characters.Count - 1;
        }


        SelectCharacter(previousIndex);
    }


    // ============================================================
    // NEXT GROUP
    // ============================================================

    public void NextGroup()
    {
        if (groups.Count == 0)
            return;


        int nextGroup =
            currentGroupIndex + 1;


        if (nextGroup >= groups.Count)
        {
            nextGroup = 0;
        }


        LoadGroup(nextGroup);
    }


    // ============================================================
    // PREVIOUS GROUP
    // ============================================================

    public void PreviousGroup()
    {
        if (groups.Count == 0)
            return;


        int previousGroup =
            currentGroupIndex - 1;


        if (previousGroup < 0)
        {
            previousGroup =
                groups.Count - 1;
        }


        LoadGroup(previousGroup);
    }


    // ============================================================
    // RESET CURRENT CHARACTER
    // ============================================================

    public void ResetCurrentCharacter()
    {
        if (CurrentCharacter == null)
            return;


        if (tracingController != null)
        {
            tracingController.ResetTrace();
        }


        UpdateCharacterButtonSelection();
    }
}
