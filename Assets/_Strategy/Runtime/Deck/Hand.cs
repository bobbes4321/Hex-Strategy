using System.Collections.Generic;
using _Strategy.Runtime.Gameloop;
using Reflex.Attributes;
using Runtime.Messaging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Strategy.Runtime.Deck
{
    public enum HandState
    {
        None,
        Hovered, //When a card is being hovered or dragged towards a piece
        Selected
    }

    public class Hand : MonoBehaviour
    {
        public int MaxCardsInHand => _playerData.MaxCardsInHand;

        [SerializeField] private Card _prefab;
        [SerializeField] private Transform _parent;

        [Inject] private MessagingManager _messagingManager;
        [Inject] private GameManager _gameManager;
        [Inject] private PlayerData _playerData;
        [Inject] private Deck _deck;

        private List<Card> _cards = new();

        public HandState State { get; private set; } = HandState.None;

        public int CardsInHand => _cards.Count;
        [ReadOnly] public Card SelectedCard = null;

        private void Awake()
        {
            SetupInitialCards();
        }

        private void SetupInitialCards()
        {
            foreach (var cardData in _deck.GetCards(MaxCardsInHand))
            {
                Add(cardData);
            }
        }

        [Button]
        public void ResetHand()
        {
            for (int index = _cards.Count - 1; index >= 0; index--)
            {
                var card = _cards[index];

                if (_cards.Contains(card))
                {
                    card.OnHoverEntered -= HandleHoverEntered;
                    card.OnHoverExited -= HandleHoverExited;
                    card.OnSelected -= HandleSelected;
                    card.OnDeselected -= HandleDeselected;

                    _cards.Remove(card);
                    Destroy(card.gameObject);
                }
            }
            
            SetupInitialCards();
        }

        public void Add(CardData cardData)
        {
            var newCard = Instantiate(_prefab, _parent);
            newCard.Initialize(cardData, this);
            _cards.Add(newCard);
            newCard.OnHoverEntered += HandleHoverEntered;
            newCard.OnHoverExited += HandleHoverExited;
            newCard.OnSelected += HandleSelected;
            newCard.OnDeselected += HandleDeselected;
        }

        private void HandleDeselected(Card card)
        {
            State = HandState.None;
            SelectedCard = null;
        }

        private void HandleSelected(Card card)
        {
            State = HandState.Selected;
            SelectedCard = card;
        }

        private void HandleHoverExited(Card card)
        {
            State = HandState.None;
            SelectedCard = null;
        }

        private void HandleHoverEntered(Card card)
        {
            State = HandState.Hovered;
        }

        public void Remove(Card card)
        {
            if (_cards.Contains(card))
            {
                card.OnHoverEntered -= HandleHoverEntered;
                card.OnHoverExited -= HandleHoverExited;
                card.OnSelected -= HandleSelected;
                card.OnDeselected -= HandleDeselected;

                _cards.Remove(card);
                Destroy(card.gameObject);

                if (_cards.Count == 0)
                    FinishTurn();
                else
                    _gameManager.CheckIfGameOver();
            } else Debug.LogWarning("[Hand.Remove] Tried to remove a card that wasn't in the hand.");
        }

        public void FinishTurn()
        {
            if (GameManager.IsPlayersTurn)
                _gameManager.FinishTurn();
        }
    }
}