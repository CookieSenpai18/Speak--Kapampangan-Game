using UnityEngine;
using UnityEngine.UI;

public class TutorialCharacterButton : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image characterImage;
    [SerializeField] private Button button;

    [Header("Selection")]
    [SerializeField] private Image selectionImage;

    private TutorialCharacterManager manager;
    private int characterIndex;

    public void Setup(
        TutorialCharacterManager manager,
        TutorialCharacter character,
        int index)
    {
        this.manager = manager;
        this.characterIndex = index;

        // Show the completed character on the button.
        characterImage.sprite = character.frontSprite;

        // Make sure we don't accumulate listeners
        // when the prefab is reused.
        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(
            OnButtonClicked
        );

        SetSelected(false);
    }

    private void OnButtonClicked()
    {
        if (manager == null)
            return;

        manager.SelectCharacter(characterIndex);
    }

    public void SetSelected(bool selected)
    {
        if (selectionImage != null)
        {
            selectionImage.gameObject.SetActive(selected);
        }
    }
}
