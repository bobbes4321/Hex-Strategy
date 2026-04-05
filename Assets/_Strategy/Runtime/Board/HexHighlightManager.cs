using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using _Strategy.Runtime.Deck;
using _Strategy.Runtime.Gameloop;
using _Strategy.Runtime.Piece;
using Reflex.Attributes;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace _Strategy.Runtime.Board
{
    public class HexHighlightManager : MonoBehaviour
    {
        [Inject] private Board _board;
        [Inject] private Hand _hand;
        [Inject] private HexView _hexViewPrefab;

        private readonly List<HexView> _hexViews = new();
        private void Start() => _board.OnHexGenerated += HandleHexGenerated;

        private void OnDestroy()
        {
            _board.OnHexGenerated -= HandleHexGenerated;

            foreach (var hexView in _hexViews)
            {
                hexView.OnHoverEntered -= HoverHex;
                hexView.OnHoverExited -= HoverHexExit;
            }
        }

        private void HandleHexGenerated(Hex hex)
        {
            var hexView = HexView.Create(hex, transform, _hexViewPrefab);
            _hexViews.Add(hexView);

            hexView.OnHoverEntered += HoverHex;
            hexView.OnHoverExited += HoverHexExit;
        }

        private void HoverHex(HexView hexView)
        {
            if (!GameManager.IsPlayersTurn)
                return;

            var piece = _board.PieceAt(hexView.Hex);
            if (piece is EnemyPiece enemyPiece && _hand.State == HandState.None)
            {
                var moves = enemyPiece.Settings.MoveCommand.GetHexes(piece);
                HighlightHexes(moves, HighlightState.Enemy);
                enemyPiece.ShowDetails();
            }
            else if (_hand.State == HandState.Selected)
            {
                var hoverHexes = _hand.SelectedCard.GetHoverHexes(hexView.Hex).ToList();
                var hexes = _hand.SelectedCard.GetHexes().ToList();

                if (hexes.Contains(hexView.Hex))
                    HighlightHexes(hoverHexes, HighlightState.Enemy);
            }
            else
            {
                if (piece is Piece.Piece piecePiece)
                    piecePiece.ShowDetails();

                if (hexView.HighlightState == HighlightState.None)
                    HighlightHex(hexView, HighlightState.Hovered);
            }
        }

        private void HoverHexExit(HexView hexView)
        {
            var piece = _board.PieceAt(hexView.Hex);
            if (piece is EnemyPiece enemyPiece && _hand.State == HandState.None)
            {
                var moves = enemyPiece.Settings.MoveCommand.GetHexes(piece);
                HighlightHexes(moves, HighlightState.None);
                enemyPiece.HideDetails();
            }
            else if (_hand.State == HandState.Selected)
            {
                var hoverHexes = _hand.SelectedCard.GetHoverHexes(hexView.Hex).ToList();
                var hexes = _hand.SelectedCard.GetHexes().ToList();
                foreach (var hoverHex in hoverHexes)
                {
                    if (hexes.Contains(hoverHex))
                        HighlightHex(hoverHex, HighlightState.Highlighted);
                    else HighlightHex(hoverHex, HighlightState.None);
                }
            }
            else
            {
                if (hexView.HighlightState == HighlightState.Hovered)
                    HighlightHex(hexView, HighlightState.None);
            }
            
            if (piece is Piece.Piece piecePiece)
                piecePiece.HideDetails();
        }

        private void HighlightHex(HexView hexView, HighlightState state)
        {
            if (hexView != null)
                hexView.SetHighlightState(state);
        }

        public void HighlightHex(Hex hex, HighlightState state = HighlightState.Highlighted)
        {
            var hexView = _hexViews.Find(t => t.Hex == hex);
            HighlightHex(hexView, state);
        }

        public void HighlightHexes(IEnumerable<Hex> hexes, HighlightState state = HighlightState.Highlighted)
        {
            foreach (var hex in hexes)
            {
                var hexView = _hexViews.Find(t => t.Hex == hex);

                if (hexView)
                {
                    var piece = _board.PieceAt(hex);
                    if (state == HighlightState.Highlighted && piece is EnemyPiece)
                    {
                        Debug.Log($"[Board.HighlightHexes] Enemy piece found at hex {hex}", hexView);
                        hexView.SetHighlightState(HighlightState.Enemy);
                    }
                    else
                        hexView.SetHighlightState(state);
                }
            }
        }

        public void UnhighlightHexes()
        {
            foreach (var hex in _hexViews)
            {
                if (hex.HighlightState == HighlightState.Hovered)
                    continue;

                hex.SetHighlightState(HighlightState.None);
            }
        }
    }
}