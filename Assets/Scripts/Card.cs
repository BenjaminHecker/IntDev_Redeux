using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum CardType { Rock, Paper, Scissors }
    public enum RoundResult { Win, Lose, Tie }

    private CardType type;
    public CardType Type { get { return type; } }

    private SpriteRenderer sRender;
    private GameManager manager;

    [SerializeField] private float moveSpeed = 1f;
    private IEnumerator moveRoutine;

    [SerializeField] private Sprite back;
    private bool faceUp = false;

    private bool interactable = false;

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

    public void Setup(GameManager _manager, bool _faceUp, CardType _type)
    {
        manager = _manager;
        Flip(_faceUp);
        UpdateProperties(_type);
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

    public void SetInteractable(bool interactable)
    {
        this.interactable = interactable;
    }

    private CardProperties GetProperties()
    {
        foreach (CardProperties p in cardProperties)
            if (p.type == type)
                return p;

        return new CardProperties();
    }

    public void Move(Vector3 pos)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = MoveRoutine(pos, (pos - transform.position).magnitude / moveSpeed);
        StartCoroutine(moveRoutine);
    }

    private IEnumerator MoveRoutine(Vector3 target, float duration)
    {
        Vector3 velocity = Vector3.zero;

        while (true)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, duration);

            if ((transform.position - target).magnitude <= 0.01f)
                break;

            yield return new WaitForEndOfFrame();
        }
    }

    public static RoundResult GetRoundResult(CardType player, CardType opponent)
    {
        if (player == opponent)
            return RoundResult.Tie;

        bool win = false;
        switch (player)
        {
            case CardType.Rock:
                win = opponent == CardType.Scissors;
                break;
            case CardType.Paper:
                win = opponent == CardType.Rock;
                break;
            case CardType.Scissors:
                win = opponent == CardType.Paper;
                break;
        }

        return (win) ? RoundResult.Win : RoundResult.Lose;
    } 

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (interactable)
            transform.position += Vector3.up * 0.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (interactable)
            transform.position += Vector3.down * 0.1f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (interactable)
            manager.SelectPlayerCard(this);
    }
}
