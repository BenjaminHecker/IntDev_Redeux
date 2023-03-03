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
            card.Setup(this, false, (CardType)(i / roundNum / 2));
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
        DrawOpponentCard(0);
        DrawOpponentCard(1);
        DrawOpponentCard(2);

        DrawPlayerCard(0);
        DrawPlayerCard(1);
        DrawPlayerCard(2);

        foreach (Card card in playerCards)
            card.Flip(true);

        SelectOpponentCard();
    }

    private void DrawOpponentCard(int id)
    {
        Card card = drawCards[drawCards.Count - 1];
        card.Move(opponentHand[id].position);
        drawCards.RemoveAt(drawCards.Count - 1);
        opponentCards.Add(card);
    }

    private void DrawPlayerCard(int id)
    {
        Card card = drawCards[drawCards.Count - 1];
        card.Move(playerHand[id].position);
        card.SetInteractable(true);
        drawCards.RemoveAt(drawCards.Count - 1);
        playerCards.Add(card);
    }

    private void SelectOpponentCard()
    {
        Card card = opponentCards[Random.Range(0, opponentCards.Count)];
        opponentSelectedCard = card;
        card.Move(opponentSelected.position);
    }

    public void SelectPlayerCard(Card card)
    {
        playerSelectedCard = card;
        card.Move(playerSelected.position);

        foreach (Card c in playerCards)
            c.SetInteractable(false);

        ResolveRound();
    }

    private void ResolveRound()
    {
        opponentSelectedCard.Flip(true);

        RoundResult result = GetRoundResult(playerSelectedCard.Type, opponentSelectedCard.Type);

        if (result == RoundResult.Win)
            playerScore++;
        if (result == RoundResult.Lose)
            opponentScore++;

        UpdateText();

        DiscardHands();
    }

    private void DiscardHands()
    {
        playerSelectedCard = null;
        opponentSelectedCard = null;

        foreach (Card card in playerCards)
        {
            discardCards.Add(card);
            card.Move(discardPile.position);
        }
        foreach (Card card in opponentCards)
        {
            discardCards.Add(card);
            card.Flip(true);
            card.Move(discardPile.position);
        }

        playerCards.Clear();
        opponentCards.Clear();

        if (drawCards.Count <= 0)
            ResetDrawPile();

        DealHands();
    }

    private void ResetDrawPile()
    {
        
    }
}
