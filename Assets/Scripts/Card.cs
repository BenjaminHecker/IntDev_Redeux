using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public enum CardType { Rock, Paper, Scissors }

    private CardType type;

    [SerializeField] private SpriteRenderer background;
    [SerializeField] private SpriteRenderer icon;

    [System.Serializable]
    struct CardProperties
    {
        public string name;
        public CardType type;
        public Color background;
        public Sprite icon;
    }
    [SerializeField] private CardProperties[] cardProperties;

    public void UpdateProperties(CardType type)
    {
        this.type = type;
        CardProperties prop = GetProperties();

        background.color = prop.background;
        icon.sprite = prop.icon;
    }

    private CardProperties GetProperties()
    {
        foreach (CardProperties p in cardProperties)
            if (p.type == type)
                return p;

        return new CardProperties();
    }
}
