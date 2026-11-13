using System.Collections.Generic;
using UnityEngine;

public class FlashcardManager : MonoBehaviour
{
    public Transform contentParent;
    public FlashCardsBtn flashcardPrefab;
    public string resourcesPath = "Introduction/IndungSulat";
    private void Start()
    {

        Debug.Log("FlashcardManager Start");

        Sprite[] sprites =
            Resources.LoadAll<Sprite>(resourcesPath);
        Debug.Log("Sprites loaded: " + sprites.Length);


        Dictionary<string, FlashcardData> cards =
            new Dictionary<string, FlashcardData>();

        foreach (Sprite sprite in sprites)
        {
            string spriteName = sprite.name;

            if (spriteName.EndsWith("_front"))
            {
                string key =
                    spriteName.Replace("_front", "");

                if (!cards.ContainsKey(key))
                    cards[key] = new FlashcardData();

                cards[key].frontSprite = sprite;
            }
            else if (spriteName.EndsWith("_back"))
            {
                string key =
                    spriteName.Replace("_back", "");

                if (!cards.ContainsKey(key))
                    cards[key] = new FlashcardData();

                cards[key].backSprite = sprite;
            }
        }

        int spawned = 0;
        foreach (var pair in cards)
        {
    //         Debug.Log(
    //     $"Key: {pair.Key}, Front: {pair.Value.frontSprite}, Back: {pair.Value.backSprite}"
    // );
            FlashCardsBtn card =
        Instantiate(flashcardPrefab, contentParent);

        card.Setup(
            pair.Value.frontSprite,
            pair.Value.backSprite
        );
            spawned++;
        }
        Debug.Log("Spawned cards: " + spawned);
    }
}