using System.Linq;
using Doozy.Runtime.UIManager.Components;
using PrimeTween;
using Reflex.Attributes;
using Teo.AutoReference;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Strategy.Runtime.Deck
{
    /// <summary>
    /// Unlike a regular card which has the logic for using it, the deck card's only purpose is to be chosen to add from the deck to the hand.
    /// </summary>
    public class DeckCard : MonoBehaviour, IPointerClickHandler
    {
        [Inject] private Deck _deck;

        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _cost;
        [SerializeField] private Image _border;
        [SerializeField, GetInChildren] private UIToggle _selectedToggle;
        [SerializeField] private UIButton _mulliganButton;

        private bool _isSelected = false;
        private CardData _cardData;
        public CardData CardData => _cardData;

        public void Click() => OnPointerClick(null);

        public void OnPointerClick(PointerEventData _)
        {
            if (!_isSelected)
            {
                if (!_deck.CanSelectCard())
                    return;

                _isSelected = true;
                _deck.SelectCard(this);
            }
            else
            {
                _isSelected = false;
                _deck.UnSelectCard(this);
            }

            _selectedToggle.SetIsOn(_isSelected);
        }

        public void Initialize(CardData cardData)
        {
            _cardData = cardData;

            _image.sprite = cardData.Icon;
            _title.SetText(cardData.Name);
            _description.SetText(cardData.Description);
            _cost.SetText(_cardData.Cost.ToString());
            _border.color = cardData.Rarity.GetColor();
        }

        public void Mulligan()
        {
            if (_isSelected)
            {
                _isSelected = false;
                _deck.UnSelectCard(this);
                _selectedToggle.SetIsOn(false);
            }

            
            var newCard = _deck.GetCards(1).First();
            while (newCard == _cardData)
            {
                newCard = _deck.GetCards(1).First();
                Debug.Log("Card was the same after mulligan, retrying.");
            }

            Initialize(newCard);
            _mulliganButton.interactable = false;
            Tween.PunchScale(transform, Vector3.one * 0.2f, 0.2f);
        }
    }
}