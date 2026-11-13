using UnityEngine;
using UnityEngine.UI;

public class FlashCardsBtn : MonoBehaviour
{
    public Image cardImage;

    private Sprite frontSprite;
    private Sprite backSprite;

    private bool showingFront = true;

    private Button button;

        private void Awake()
    {

        button = GetComponent<Button>();
        button.onClick.AddListener(FlipCard);
        if (cardImage == null)
            cardImage = GetComponentInChildren<Image>();

        GetComponent<Button>().onClick.AddListener(FlipCard);
    }

  public void Setup(Sprite front, Sprite back)
{
    if (front == null || back == null)
    {
        Debug.LogWarning("Missing flashcard sprite.");
        return;
    }

    frontSprite = front;
    backSprite = back;

    showingFront = true;
    cardImage.sprite = frontSprite;
}

    public void FlipCard()
    {
        showingFront = !showingFront;

        cardImage.sprite =
            showingFront
            ? frontSprite
            : backSprite;
    }
}