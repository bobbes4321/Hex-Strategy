using System;
using System.Collections.Generic;
using System.Linq;
using _Strategy.Runtime.Board;
using _Strategy.Runtime.Gameloop;
using _Strategy.Runtime.Piece;
using PrimeTween;
using Runtime.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace _Strategy.Runtime.Deck
{
    /// <summary>
    /// Card that activates a command via a piece.
    /// </summary>
    [Serializable]
    public class PieceCardProcessor : CardProcessor
    {
        private IPiece _selectedPiece = new NullPiece();
        
        /// <summary>
        /// Highlights all the pieces this command can be used on
        /// </summary>
        private void HighlightUsablePieces()
        {
            var pieces = _board.GetPiecesOfTeam(GameManager.CurrentTeam);
            _hexHighlightManager.HighlightHexes(pieces.Select(x => x.Hex));
        }

        /// <summary>
        /// Highlights the tiles that this piece can activate the command to.
        /// </summary>
        private void Highlight()
        {
            var tiles = _data.Command.GetHexes(_selectedPiece).ToList();
            _hexHighlightManager.HighlightHexes(tiles);
            _hexHighlightManager.HighlightHex(_board.HexOf(_selectedPiece), HighlightState.Selected);
            _card.Select();
        }

        protected void Unhighlight()
        {
            _hexHighlightManager.UnhighlightHexes();
            Hex.OnHexSelected -= OnHexClicked;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            HighlightUsablePieces();
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            Unhighlight();
        }

        public override void OnEndDrag(Hex hex)
        {
            var piece = _board.PieceAt(hex);
            if (!piece.IsNull())
            {
                _selectedPiece = piece;
                Unhighlight();
                Highlight();
                Hex.OnHexSelected += OnHexClicked;
            }
            else
            {
                Unhighlight();
            }
        }

        private void OnHexClicked(Hex clickedHex)
        {
            Debug.Log($"[Card.OnTileClicked] {clickedHex}", _card);
            Unhighlight();
            _card.Deselect();
            if (_selectedPiece.IsNull()) return;

            var validHexes = _data.Command.GetHexes(_selectedPiece);

            if (validHexes.Contains(clickedHex))
                _card.Activate(_selectedPiece, clickedHex);

            _selectedPiece = new NullPiece();
        }
        
        public override IEnumerable<Hex> GetHexes() => _data.Command.GetHexes(_selectedPiece);
    }

    [Serializable]
    public abstract class CardProcessor
    {
        protected Board.Board _board;
        protected HexHighlightManager _hexHighlightManager;
        protected Card _card;
        protected CardData _data;
        protected Camera _camera;

        public void Initialize(Card card, CardData data)
        {
            _card = card;
            _data = data;
            _board = DI.Resolve<Board.Board>();
            _hexHighlightManager = DI.Resolve<HexHighlightManager>();
            _camera = Camera.main;
        }
        
        public abstract void OnPointerEnter(PointerEventData eventData);
        public abstract void OnPointerExit(PointerEventData eventData);
        public abstract void OnEndDrag(Hex hex);
        public abstract IEnumerable<Hex> GetHexes();
    }
}