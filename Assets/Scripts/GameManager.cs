using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Card;

public class GameManager : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] private Transform drawPile;
    [SerializeField] private Transform discardPile;
    [SerializeField] private Transform playerSelected;
    [SerializeField] private Transform[] playerHand;
    [SerializeField] private Transform opponentSelected;
    [SerializeField] private Transform[] opponentHand;

    [Header("Card Info")]
    [SerializeField] private Card prefab_Card;
    [SerializeField] private int roundNum = 4;
    [SerializeField] private float cardStackOffset = 0.1f;

    private List<Card> drawCards = new List<Card>();
    private List<Card> discardCards = new List<Card>();
    private List<Card> playerCards = new List<Card>();
    private List<Card> opponentCards = new List<Card>();
    private Card playerSelectedCard;
    private Card opponentSelectedCard;

    [Header("Miscellaneous")]
    [SerializeField] private TextMeshProUGUI txt_PlayerScore;
    [SerializeField] private TextMeshProUGUI txt_OpponentScore;

    private int playerScore = 0;
    private int opponentScore = 0;

    private void Awake()
    {
        UpdateText();
        StartGame();
    }

    private void UpdateText()
    {
        txt_PlayerScore.text = playerScore.ToString();
        txt_OpponentScore.text = opponentScore.ToString();
    }

    private void StartGame()
    {
        ClearAllCards();

        for (int i = 0; i < roundNum * 6; i++)
        {
            Card card = Instantiate(prefab_Card);
            card.Flip(false);
            card.UpdateProperties((CardType) (i / roundNum * 2));
            drawCards.Add(card);
        }

        ShuffleCards();

        for (int i = 0; i < drawCards.Count; i++)
            drawCards[i].transform.position = drawPile.position + Vector3.up * i * cardStackOffset;

        DealHands();
    }

    private void ClearAllCards()
    {
        foreach (Card c in drawCards) Destroy(c.gameObject);
        foreach (Card c in discardCards) Destroy(c.gameObject);
        foreach (Card c in playerCards) Destroy(c.gameObject);
        foreach (Card c in opponentCards) Destroy(c.gameObject);

        drawCards.Clear();
        discardCards.Clear();
        playerCards.Clear();
        opponentCards.Clear();
    }

    private void ShuffleCards()
    {
        int n = drawCards.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            Card card = drawCards[k];
            drawCards[k] = drawCards[n];
            drawCards[n] = card;
        }
    }

    private void DealHands()
    {
        Card card;

        card = drawCards[drawCards.Count - 1];
        card.Move(opponentHand[0].position);
        drawCards.RemoveAt(drawCards.Count - 1);
        opponentCards.Add(card);

        card = drawCards[drawCards.Count - 1];
        card.Move(opponentHand[1].position);
        drawCards.RemoveAt(drawCards.Count - 1);
        opponentCards.Add(card);

        card = drawCards[drawCards.Count - 1];
        card.Move(opponentHand[2].position);
        drawCards.RemoveAt(drawCards.Count - 1);
        opponentCards.Add(card);
    }
}
