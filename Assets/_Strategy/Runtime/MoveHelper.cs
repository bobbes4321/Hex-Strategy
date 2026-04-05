using System;
using System.Collections.Generic;
using System.Linq;
using _Strategy.Runtime.Board;
using _Strategy.Runtime.Piece;
using Runtime.Messaging;

namespace _Strategy.Runtime
{
    public static class MoveHelper
    {
        public static IEnumerable<Hex> GetMovesInDirection(IPiece piece, Direction direction, int distance = 8, bool needsLineOfSight = true, params Func<IPiece, IPiece, bool>[] shouldYieldHex)
        {
            for (int i = 1; i <= distance; i++)
            {
                var hexOffset = new Hex().Neighbor(direction, i);
                
                if (TryGetMoveTo(hexOffset, piece, out var toHex, shouldYieldHex))
                    yield return toHex;
                else if (needsLineOfSight && (!toHex.IsEmpty || !toHex.IsValid))
                    break;
                else break;
            }
        }
        
        public static IEnumerable<Hex> GetDiagonalMovesInDirection(IPiece piece, Direction direction, int distance = 8, bool needsLineOfSight = true, params Func<IPiece, IPiece, bool>[] shouldYieldHex)
        {
            for (int i = 1; i <= distance; i++)
            {
                var hexOffset = new Hex().DiagonalNeighbor(direction, i);
                
                if (TryGetMoveTo(hexOffset, piece, out var toHex, shouldYieldHex))
                    yield return toHex;
                else if (needsLineOfSight && (!toHex.IsEmpty || !toHex.IsValid))
                    break;
                else break;
            }
        }

        /// <summary>
        /// Gets a move to a tile, where the position of the tile is relative to the piece's position.
        /// </summary>
        /// <returns></returns>
        public static bool TryGetMoveTo(Hex hex, IPiece piece, out Hex toHex, Func<IPiece, IPiece, bool>[] shouldYieldHex = null)
        {
            var board = DI.Resolve<Board.Board>();

            var fromHex = board.HexOf(piece);
            toHex = hex + fromHex;
            
            if (!toHex.IsValid)
                return false;
            
            var otherPiece = board.PieceAt(toHex);

            if (shouldYieldHex == null)
            {
                if (otherPiece.AreEnemies(piece))
                    return true;
            }
            else
            {
                if (shouldYieldHex.Any(x => x.Invoke(piece, otherPiece)))
                    return true;
            }

            return false;
        }

        public static bool CanCapture(IPiece currentPiece, IPiece otherPiece) => otherPiece is not NullPiece && currentPiece.AreEnemies(otherPiece);
        public static bool IsEmpty(IPiece currentPiece, IPiece otherPiece) => otherPiece is NullPiece;
    }

    public enum Direction
    {
        Right,
        DownRight,
        DownLeft,
        Left,
        UpLeft,
        UpRight,
    }
}