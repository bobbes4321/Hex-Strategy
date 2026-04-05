using System;
using System.Collections.Generic;
using System.Linq;
using _Strategy.Runtime.Board;
using _Strategy.Runtime.Gameloop;
using _Strategy.Runtime.Piece;
using PrimeTween;
using Reflex.Attributes;
using Runtime.Messaging;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using HighlightState = _Strategy.Runtime.Board.HighlightState;

namespace _Strategy.Runtime.Deck
{
    public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [BoxGroup("References")] [SerializeField]
        private Image _image;

        [BoxGroup("References")] [SerializeField]
        private TMP_Text _title;

        [BoxGroup("References")] [SerializeField]
        private TMP_Text _description;

        [BoxGroup("References")] [SerializeField]
        private TMP_Text _cost;

        [BoxGroup("References")] [SerializeField]
        private Image _border;

        [Inject] protected MessagingManager _messagingManager;
        [Inject] protected AP _ap;

        protected CardData _data;
        protected Hand _hand;

        protected bool _isDragging = false;
        protected CanvasGroup _canvasGroup;
        protected RectTransform _rectTransform;
        protected RectTransform _contentRectTransform;
        protected Vector3 _originalPosition;
        protected Camera _camera;
        protected Vector3 _defaultScale;
        private CardProcessor _processor;

        protected static Card _currentlySelectedCard;
        protected bool IsSelected => _currentlySelectedCard == this;
        public event Action<Card> OnHoverEntered;
        public event Action<Card> OnHoverExited;
        public event Action<Card> OnSelected;
        public event Action<Card> OnDeselected;

        private void Awake()
        {
            _defaultScale = transform.localScale;
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            _contentRectTransform = _rectTransform.GetChild(0).GetComponent<RectTransform>();
            _camera = Camera.main;

            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        private void Update()
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (IsSelected)
                    Deselect();
            }
        }

        [Button]
        public void Initialize(CardData cardData, Hand hand)
        {
            _data = cardData;
            _hand = hand;
            _processor = (CardProcessor)Activator.CreateInstance(cardData.Processor.GetType());
            _processor.Initialize(this, _data);
            _image.sprite = _data.Icon;
            _title.SetText(_data.Name);
            _cost.SetText(_data.Cost.ToString());
            _description.SetText(_data.Description);
            _border.color = _data.Rarity.GetColor();
        }

        public void Select()
        {
            _currentlySelectedCard = this;
            Tween.UIAnchoredPosition(_contentRectTransform, Vector2.up * 50, 0.2f);

            OnSelected?.Invoke(this);
        }

        public void Deselect()
        {
            _currentlySelectedCard = null;

            if (_contentRectTransform.anchoredPosition != Vector2.zero)
                Tween.UIAnchoredPosition(_contentRectTransform, Vector2.zero, 0.2f);

            OnDeselected?.Invoke(this);
        }

        public void ResetPosition()
        {
            _isDragging = false;

            Tween.Position(_rectTransform, _originalPosition, 0.2f);
            Tween.Alpha(_canvasGroup, 1f, 0.2f);
            Tween.Scale(transform, _defaultScale, 0.2f).OnComplete(() =>
            {
                    _canvasGroup.blocksRaycasts = true;
            }, warnIfTargetDestroyed: false);
        }

        public IEnumerable<Hex> GetHoverHexes(Hex currentlyHoveredHex) => _data.Command.GetHoverHexes(currentlyHoveredHex);
        public IEnumerable<Hex> GetHexes() => _processor.GetHexes();

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_currentlySelectedCard == null && !IsSelected)
            {
                OnHoverEntered?.Invoke(this);
                _processor.OnPointerEnter(eventData);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_currentlySelectedCard != null && _currentlySelectedCard != this)
                return;

            if (!_isDragging && !IsSelected)
            {
                _processor.OnPointerExit(eventData);
                OnHoverExited?.Invoke(this);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_ap.Current < _data.Cost)
            {
                _messagingManager.Publish(new TriedToUseCardThatWasTooExpensiveMessage());
                return;
            }

            _isDragging = true;
            _originalPosition = _rectTransform.position;
            Tween.Alpha(_canvasGroup, 0.2f, 0.2f);
            Tween.Scale(transform, Vector3.one * 0.5f, 0.2f);
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_ap.Current < _data.Cost)
                return;

            _rectTransform.position = Vector3.Lerp(_rectTransform.position, eventData.position, Time.deltaTime * 50f);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_ap.Current < _data.Cost)
                return;
            
            ResetPosition();
            _processor.OnEndDrag(GetHexUnderMouse());
        }

        /// <summary>
        /// Activates the command for the piece in question with the selected tile.
        /// </summary>
        public void Activate(IPiece piece, Hex selectedHex)
        {
            Debug.Log($"[Deck.Activate]", this);
            _data.Command.Execute(piece, selectedHex);
            _hand.Remove(this);
            _ap.Remove(_data.Cost);
        }

        private Hex GetHexUnderMouse()
        {
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit)) return new(-1000, -1000);
            if (!hit.transform.TryGetComponent(out HexView hexView)) return new(-1000, -1000);
            return hexView.Hex;
        }
    }

    public struct TriedToUseCardThatWasTooExpensiveMessage : IMessage
    {
    }
}