using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Messaging;

namespace Runtime.Chess
{
    public class MoveComposer : IEnumerable<Move>
    {
        private List<Move> _moves = new();

        private IPiece _piece;

        public MoveComposer(IPiece piece)
        {
            _piece = piece;
        }

        public MoveComposer Up(int distance, Func<IPiece, IPiece, bool> shouldYieldTile = null) => GenerateMovesInDirection(_piece, distance, shouldYieldTile, Direction.Up);
        public MoveComposer Down(int distance, Func<IPiece, IPiece, bool> shouldYieldTile = null) => GenerateMovesInDirection(_piece, distance, shouldYieldTile, Direction.Down);
        public MoveComposer Left(int distance, Func<IPiece, IPiece, bool> shouldYieldTile = null) => GenerateMovesInDirection(_piece, distance, shouldYieldTile, Direction.Left);
        public MoveComposer Right(int distance, Func<IPiece, IPiece, bool> shouldYieldTile = null) => GenerateMovesInDirection(_piece, distance, shouldYieldTile, Direction.Right);
        public MoveComposer UpLeft(int distance, Func<IPiece, IPiece, bool> shouldYieldTile = null) => GenerateMovesInDirection(_piece, distance, shouldYieldTile, Direction.UpLeft);
        public MoveComposer UpRight(int distance, Func<IPiece, IPiece, bool> shouldYieldTile = null) => GenerateMovesInDirection(_piece, distance, shouldYieldTile, Direction.UpRight);
        public MoveComposer DownLeft(int distance, Func<IPiece, IPiece, bool> shouldYieldTile = null) => GenerateMovesInDirection(_piece, distance, shouldYieldTile, Direction.DownLeft);
        public MoveComposer DownRight(int distance, Func<IPiece, IPiece, bool> shouldYieldTile = null) => GenerateMovesInDirection(_piece, distance, shouldYieldTile, Direction.DownRight);

        private MoveComposer GenerateMovesInDirection(IPiece piece, int distance, Func<IPiece, IPiece, bool> shouldYieldTile, Direction direction)
        {
            for (int i = 1; i <= distance; i++)
            {
                var tile = GetTileInDirection(direction, i);
                var move = GetMoveTo(tile, piece, shouldYieldTile);

                if (move != default)
                    _moves.Add(move);
                else break;

                if (!move.To.IsEmpty)
                    break;
            }

            return this;
        }

        private Move GetMoveTo(Tile tile, IPiece piece, Func<IPiece, IPiece, bool> shouldYieldTile)
        {
            var board = ServiceLocator.Get<ChessBoard>();

            var fromTile = board.TileOf(piece);
            var toTile = tile + fromTile;
            var otherPiece = board.PieceAt(toTile);

            if (shouldYieldTile == null)
            {
                if (otherPiece.AreEnemies(piece))
                    return new(toTile, piece);
            }
            else
            {
                if (shouldYieldTile.Invoke(piece, otherPiece))
                    return new(toTile, piece);
            }

            return default;
        }

        private static Tile GetTileInDirection(Direction direction, int i)
        {
            Tile tile = direction switch
            {
                Direction.Up => new(0, i),
                Direction.Down => new(0, -i),
                Direction.Left => new(-i, 0),
                Direction.Right => new(i, 0),
                Direction.UpLeft => new(-i, i),
                Direction.UpRight => new(i, i),
                Direction.DownLeft => new(-i, -i),
                Direction.DownRight => new(i, -i),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
            return tile;
        }
        
        public IEnumerator<Move> GetEnumerator()
        {
            foreach (var move in _moves)
                yield return move;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class MoveHelper
    {
        public static IEnumerable<Move> GetMovesInDirection(IPiece piece, Direction direction, int distance = 8, Func<IPiece, IPiece, bool> shouldYieldTile = null)
        {
            for (int i = 1; i <= distance; i++)
            {
                var tile = GetTileInDirection(direction, i);
                var move = GetMoveTo(tile, piece, shouldYieldTile);

                if (move != default)
                    yield return move;
                else break;

                if (!move.To.IsEmpty)
                    break;
            }
        }

        /// <summary>
        /// Gets a move to a tile, where the position of the tile is relative to the piece's position.
        /// </summary>
        /// <returns></returns>
        public static Move GetMoveTo(Tile tile, IPiece piece, Func<IPiece, IPiece, bool> shouldYieldTile = null)
        {
            var board = ServiceLocator.Get<ChessBoard>();

            var fromTile = board.TileOf(piece);
            var toTile = tile + fromTile;
            var otherPiece = board.PieceAt(toTile);

            if (shouldYieldTile == null)
            {
                if (otherPiece.AreEnemies(piece))
                    return new(toTile, piece);
            }
            else
            {
                if (shouldYieldTile.Invoke(piece, otherPiece))
                    return new(toTile, piece);
            }

            return default;
        }

        public static bool CanCapture(IPiece currentPiece, IPiece otherPiece) => otherPiece is not NullPiece && currentPiece.AreEnemies(otherPiece);
        public static bool IsEmpty(IPiece currentPiece, IPiece otherPiece) => otherPiece is NullPiece;

        private static Tile GetTileInDirection(Direction direction, int i)
        {
            Tile tile = direction switch
            {
                Direction.Up => new(0, i),
                Direction.Down => new(0, -i),
                Direction.Left => new(-i, 0),
                Direction.Right => new(i, 0),
                Direction.UpLeft => new(-i, i),
                Direction.UpRight => new(i, i),
                Direction.DownLeft => new(-i, -i),
                Direction.DownRight => new(i, -i),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
            return tile;
        }
    }
}