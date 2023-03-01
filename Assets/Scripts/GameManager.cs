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

    private List<Card> cards = new List<Card>();

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
        cards.Clear();

        for (int i = 0; i < roundNum * 6; i++)
        {
            Vector3 pos = drawPile.position + Vector3.up * i * cardStackOffset;
            Card card = Instantiate(prefab_Card, pos, Quaternion.identity);
            card.Flip(false);
            card.UpdateProperties((CardType) (i / roundNum * 2));

            cards.Add(card);
        }
    }
}
