using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class Card : MonoBehaviour
{
    public enum CardType { Rock, Paper, Scissors }

    private CardType type;
    private SpriteRenderer sRender;

    [SerializeField] private Sprite back;
    private bool faceUp = false;

    [System.Serializable]
    struct CardProperties
    {
        public string name;
        public CardType type;
        public Sprite sprite;
    }
    [SerializeField] private CardProperties[] cardProperties;

    private void Awake()
    {
        sRender = GetComponent<SpriteRenderer>();
    }

    public void Flip(bool faceUp)
    {
        this.faceUp = faceUp;
        UpdateSprite();
    }

    public void UpdateProperties(CardType type)
    {
        this.type = type;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        sRender.sprite = (faceUp) ? GetProperties().sprite : back;
    }

    private CardProperties GetProperties()
    {
        foreach (CardProperties p in cardProperties)
            if (p.type == type)
                return p;

        return new CardProperties();
    }
}
