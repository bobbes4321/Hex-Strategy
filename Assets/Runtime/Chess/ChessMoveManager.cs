using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Messaging;
using UnityEngine;

namespace Runtime.Chess
{
    public class ChessMoveManager : MonoBehaviour
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
            FilterValidMoves(_moves);
            ServiceLocator.Get<ChessBoard>().HighlightTiles(_moves.Select(x => x.To));
        }

        private void FilterValidMoves(List<Move> moves)
        {
            for (int index = moves.Count - 1; index >= 0; index--)
            {
                var move = moves[index];
                if (!IsValidMove(move))
                    moves.Remove(move);
            }
        }

        private bool IsValidMove(Move move)
        {
            //1. Simulate the movement to the tile
            var board = ServiceLocator.Get<ChessBoard>();
            var oldPiece = board.PieceAt(move.To);
            var fromTile = board.TileOf(move.Piece);
            board.RemoveAt(fromTile);
            board.AddPiece(move.Piece, move.To);

            //2. See if the king is in check
            bool isValidMove = true;
            var enemyPieces = board.GetEnemyPieces(move.Piece.Team);
            var kingTile = board.TileOf(board.GetKing(move.Piece.Team));
            if (King.IsInCheck(enemyPieces, kingTile))
                isValidMove = false;

            //3. Undo the movement
            board.RemoveAt(move.To);
            board.AddPiece(move.Piece, fromTile);
            board.AddPiece(oldPiece, move.To);

            return isValidMove;
        }
    }

    public struct Move : IEquatable<Move>
    {
        public Move(Tile to, IPiece piece, Action moveAction = null) : this()
        {
            To = to;
            Piece = piece;
            From = ServiceLocator.Get<ChessBoard>().TileOf(piece);
            MoveAction = moveAction ?? DoMove;
        }

        public Tile From { get; set; }
        public Tile To { get; set; }
        public IPiece Piece { get; set; }
        public Action MoveAction { get; set; }

        private void DoMove()
        {
            var board = ServiceLocator.Get<ChessBoard>();
            var piece = Piece;

            board.RemoveAt(From);

            var currentPiece = board.PieceAt(To);
            if (currentPiece is NullPiece || currentPiece.AreEnemies(piece))
            {
                board.CapturePiece(To);
                board.AddPiece(piece, To);
                piece.MoveTo(To);
            }
            else
                Debug.LogError($"[Board.Move] Tried to move piece {piece} to tile {To} where there is already an allied piece: {currentPiece}");
        }

      
        public bool Equals(Move other)
        {
            return From.Equals(other.From) && To.Equals(other.To) && Equals(Piece, other.Piece);
        }

        public override bool Equals(object obj)
        {
            return obj is Move other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, To, Piece);
        }

        public static bool operator ==(Move left, Move right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Move left, Move right)
        {
            return !left.Equals(right);
        }
    }

    public static class MoveExtensions
    {
        public static void DoAttack(this Move move)
        {
            var board = ServiceLocator.Get<ChessBoard>();
            var piece = move.Piece;

            var currentPiece = board.PieceAt(move.To);
            if (currentPiece is NullPiece || currentPiece.AreEnemies(piece))
            {
                currentPiece.Get<Health>()?.DoDamage(1f);
            }
            else
                Debug.LogError($"[Board.Move] Tried to attack allied piece {piece} on tile {move.To}");
        }

    }

    [Serializable]
    public class Health : IPieceComponent
    {
        private float _health;
        private float _maxHealth;

        public bool IsDead => _health > 0;
        
        public Health(float maxHealth)
        {
            _maxHealth = maxHealth;
            _health = maxHealth;
        }
        
        public void DoDamage(float damage)
        {
            _health -= damage;
            
            if (_health <= 0)
                _health = 0;
        }
        
        public void Rejuvenate() => _health = _maxHealth;
    }
    
    public interface IPieceComponent{}
}