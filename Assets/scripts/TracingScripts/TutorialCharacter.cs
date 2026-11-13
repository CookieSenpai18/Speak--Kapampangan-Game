using UnityEngine;

[System.Serializable]
public class TutorialCharacter
{
    public string characterName;
    public Sprite backSprite;
    public Sprite frontSprite;

    public TutorialCharacter(
        string characterName,
        Sprite backSprite,
        Sprite frontSprite)
    {
        this.characterName = characterName;
        this.backSprite = backSprite;
        this.frontSprite = frontSprite;
    }
}
