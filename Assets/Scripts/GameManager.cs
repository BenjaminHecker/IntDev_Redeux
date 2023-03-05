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
    [SerializeField] private Transform shuffleSpot;
    [SerializeField] private Transform playerSelected;
    [SerializeField] private Transform[] playerHand;
    [SerializeField] private Transform opponentSelected;
    [SerializeField] private Transform[] opponentHand;

    [Header("Card Info")]
    [SerializeField] private Card prefab_Card;
    [SerializeField] private int roundNum = 4;
    [SerializeField] private float cardStackOffset = 0.1f;
    [SerializeField] private float cardDrawDelay = 0.2f;
    [SerializeField] private float cardRevealDelay = 1f;
    [SerializeField] private float cardShuffleDelay = 0.05f;

    private List<Card> drawCards = new List<Card>();
    private List<Card> discardCards = new List<Card>();
    private List<Card> playerCards = new List<Card>();
    private List<Card> opponentCards = new List<Card>();
    private Card playerSelectedCard;
    private Card opponentSelectedCard;
    private int discardStackIdx = 0;

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

        StartCoroutine(DealHands());
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

    private IEnumerator DealHands()
    {
        DrawOpponentCard(0);
        yield return new WaitForSeconds(cardDrawDelay);
        DrawOpponentCard(1);
        yield return new WaitForSeconds(cardDrawDelay);
        DrawOpponentCard(2);
        yield return new WaitForSeconds(cardDrawDelay);

        DrawPlayerCard(0);
        yield return new WaitForSeconds(cardDrawDelay);
        DrawPlayerCard(1);
        yield return new WaitForSeconds(cardDrawDelay);
        DrawPlayerCard(2);
        yield return new WaitForSeconds(cardDrawDelay);

        yield return new WaitForSeconds(cardRevealDelay);

        foreach (Card card in playerCards)
            card.SetInteractable(true);

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
        card.Flip(true);
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
        
        StartCoroutine(ResolveRound());
    }

    private IEnumerator ResolveRound()
    {
        yield return new WaitForSeconds(cardRevealDelay);

        opponentSelectedCard.Flip(true);

        yield return new WaitForSeconds(cardRevealDelay);

        RoundResult result = GetRoundResult(playerSelectedCard.Type, opponentSelectedCard.Type);

        if (result == RoundResult.Win)
            playerScore++;
        if (result == RoundResult.Lose)
            opponentScore++;

        UpdateText();

        yield return new WaitForSeconds(cardRevealDelay);

        StartCoroutine(DiscardHands());
    }

    private IEnumerator DiscardHands()
    {
        playerSelectedCard = null;
        opponentSelectedCard = null;

        foreach (Card card in playerCards)
        {
            discardCards.Add(card);
            card.Move(discardPile.position + Vector3.up * discardStackIdx++ * cardStackOffset);
        }
        foreach (Card card in opponentCards)
        {
            discardCards.Add(card);
            card.Flip(true);
            card.Move(discardPile.position + Vector3.up * discardStackIdx++ * cardStackOffset);
        }

        playerCards.Clear();
        opponentCards.Clear();

        yield return new WaitForSeconds(cardRevealDelay);

        if (drawCards.Count <= 0)
            yield return ResetDrawPile();

        StartCoroutine(DealHands());
    }

    private IEnumerator ResetDrawPile()
    {
        discardStackIdx = 0;

        foreach (Card card in discardCards)
        {
            card.Flip(false);
            card.Move(shuffleSpot.position);
            drawCards.Add(card);
            yield return new WaitForSeconds(cardShuffleDelay);
        }
        discardCards.Clear();

        yield return new WaitForSeconds(cardRevealDelay);

        ShuffleCards();

        for (int i = 0; i < drawCards.Count; i++)
        {
            drawCards[i].Move(drawPile.position + Vector3.up * i * cardStackOffset);
            yield return new WaitForSeconds(cardShuffleDelay);
        }

        yield return new WaitForSeconds(cardRevealDelay);
    }
}
