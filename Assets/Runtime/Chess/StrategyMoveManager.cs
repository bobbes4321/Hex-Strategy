using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Signals;
using Runtime.Messaging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Chess
{
    public class StrategyMoveManager : MonoBehaviour
    {
        private IPiece _selectedPiece = new NullPiece();
        private List<Move> _moves = new();
        private bool _isPieceSelected => !_selectedPiece.IsNull();

        private void Awake()
        {
            Tile.OnTileSelected += HandleTileSelected;
        }

        private void OnDestroy()
        {
            Tile.OnTileSelected -= HandleTileSelected;
        }

        private void HandleTileSelected(Tile tile)
        {
            var board = ServiceLocator.Get<ChessBoard>();
            if (_isPieceSelected)
            {
                //Get the piece that was previously on the tile we selected BEFORE moving the new piece there
                var pieceAtTile = board.PieceAt(tile);

                bool didMove = false;
                if (_moves != null)
                {
                    foreach (var move in _moves)
                    {
                        if (move.To != tile) continue;
                        move.MoveAction();
                        didMove = true;
                        break;
                    }

                    board.UnhighlightTiles(_moves.Select(x => x.To));
                }

                if (pieceAtTile is NullPiece || pieceAtTile.Team != GameManager.CurrentTeam)
                {
                    _moves = null;
                }
                else
                {
                    //As far as I know, this only needs to happen when long castling by clicking on the rook tile
                    if (!didMove)
                    {
                        _selectedPiece = pieceAtTile;
                        HighlightMoves(_selectedPiece);
                    }
                }

                if (didMove)
                    ServiceLocator.Get<MessagingManager>().Publish(new MoveMessage());
            }
            else
            {
                _selectedPiece = board.PieceAt(tile);

                if (_selectedPiece is NullPiece)
                    return;

                if (_selectedPiece.Team != GameManager.CurrentTeam)
                    return;

                HighlightMoves(_selectedPiece);
            }
        }

        private void HighlightMoves(IPiece piece)
        {
            _moves = piece.GetMoves().ToList();
            ServiceLocator.Get<ChessBoard>().HighlightTiles(_moves.Select(x => x.To));
        }
    }
    
    public static class VectorExtensions
    {
        public static Vector2 ToVector2(this Vector2Int vector) => new Vector2(vector.x, vector.y);
        public static Vector2Int ToVector2Int(this Vector2 vector) => new Vector2Int((int)vector.x, (int)vector.y);
        public static float SquaredDistance(this Vector2 vector) => vector.x * vector.x + vector.y * vector.y;
    }
}