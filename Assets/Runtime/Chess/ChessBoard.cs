using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common;
using Runtime.Messaging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Chess
{
    public class ChessBoard : MonoBehaviour
    {
        private readonly List<TileView> _tileViews = new();
        private readonly List<IPiece> _enemyPieces = new();
        private readonly Dictionary<Tile, IPiece> _pieces = new();
        private static int _boardSize = 8;
        private static float _tileSize = 1f;
        private static Vector3 _bottomLeftPosition = Vector3.zero;

        private void Awake()
        {
            ServiceLocator.Register(this);
            CreateBoard();
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<ChessBoard>();
        }

        private void CreateBoard()
        {
            _pieces.Clear();

            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    var tile = new Tile(x: i, y: j);
                    _pieces.Add(tile, new NullPiece());
                    //_tileViews.Add(TileView.Create(tile, _tileSize, _bottomLeftPosition));
                }
            }
        }

        public void AddPiece(IPiece piece, Tile tile)
        {
            if (_pieces.ContainsKey(tile)) _pieces[tile] = piece;
        }

        public IPiece PieceAt(Tile tile) => _pieces.TryGetValue(tile, out var piece) ? piece : new NullPiece();

        public Tile TileOf(IPiece piece)
        {
            foreach (var kvp in _pieces)
            {
                if (kvp.Value.Equals(piece))
                {
                    return kvp.Key;
                }
            }

            Debug.LogError($"[Board.TileOf] Could not find the tile for the given piece: {piece}");
            return new Tile(-1, -1); // Return an invalid tile if not found
        }

        public static Tile WorldToTile(Vector3 worldPositionTile)
        {
            Vector3 localPosition = worldPositionTile - _bottomLeftPosition;

            int x = Mathf.FloorToInt(localPosition.x / _tileSize);
            int y = Mathf.FloorToInt(localPosition.z / _tileSize);
            return new(x, y);
        }

        public static Vector3 TileToWorld(Tile tile) => _bottomLeftPosition + new Vector3(tile.X, 0, tile.Y) * _tileSize;

        public bool IsTileAt(Tile tile)
        {
            return _pieces.TryGetValue(tile, out _);
        }

        public bool IsTileAt(int x, int y) => IsTileAt(new(x, y));

        public void Move(IPiece piece, Tile tile)
        {
            var fromTile = TileOf(piece);

            RemoveAt(fromTile);
            AddPiece(piece, tile);

            piece.MoveTo(tile);
        }

        [Button]
        public void RemoveAt(Tile tile)
        {
            if (_pieces.ContainsKey(tile))
                _pieces[tile] = new NullPiece();
        }

        [Button]
        public void CapturePiece(Tile tile)
        {
            if (_pieces.TryGetValue(tile, out var piece))
                piece.GetCaptured();

            RemoveAt(tile);
        }

        public void HighlightTiles(IEnumerable<Tile> tiles)
        {
            foreach (var tile in tiles)
            {
                var tileView = _tileViews.Find(t => t.Tile == tile);

                if (tileView != null)
                    tileView.Highlight();
            }
        }

        public void UnhighlightTiles(IEnumerable<Tile> tiles)
        {
            foreach (var tile in tiles)
            {
                var tileView = _tileViews.Find(t => t.Tile == tile);

                if (tileView != null)
                    tileView.Unhighlight();
            }
        }

        public List<IPiece> GetPiecesOfTeam(Team pieceTeam)
        {
            if (pieceTeam == Team.None)
            {
                Debug.LogError("Can't get pieces of team None!", this);
                return null;
            }

            _enemyPieces.Clear();
            foreach (var kvp in _pieces)
            {
                var piece = kvp.Value;
                if (piece.Team != Team.None && piece.Team == pieceTeam)
                {
                    _enemyPieces.Add(piece);
                }
            }

            return _enemyPieces;
        }

        public bool AreAllPiecesTaken(Team team) => GetPiecesOfTeam(team) is not { Count: > 0 };

        public List<IPiece> GetEnemyPieces(Team pieceTeam) => GetPiecesOfTeam(pieceTeam.GetOther());

        public IPiece GetKing(Team team)
        {
            foreach (var kvp in _pieces)
            {
                var piece = kvp.Value;
                if (piece.Team == team && piece is King king)
                {
                    return king;
                }
            }

            return new NullPiece();
        }
    }

    public struct MoveMessage : IMessage
    {
    }
}