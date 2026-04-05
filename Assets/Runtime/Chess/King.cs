using System.Collections.Generic;
using System.Linq;
using Runtime.Messaging;
using UnityEngine;

namespace Runtime.Chess
{
    public class King : Piece
    {
        public override IEnumerable<Move> GetMoves()
        {
            MoveComposer composer = new(this);
            foreach (var move in composer
                         .Up(1)
                         .Down(1)
                         .Left(1)
                         .Right(1)
                         .UpLeft(1)
                         .UpRight(1)
                         .DownLeft(1)
                         .DownRight(1))
            {
                yield return move;
            }
            
          //  int[] deltas = { -1, 0, 1 };

            //foreach (var dx in deltas)
            //{
            //    foreach (var dy in deltas)
            //    {
            //        var move = MoveHelper.GetMoveTo(new(dx, dy), this);
            //        if (move != default)
            //            yield return move;
            //    }
            //}

            if (HasMoved) yield break;
            var board = ServiceLocator.Get<ChessBoard>();
            var currentTile = board.TileOf(this);

            var shortCastleMoves = GetCastleMoves(board, currentTile, isShortCastle: true);
            if (shortCastleMoves != null)
                foreach (var shortCastleMove in shortCastleMoves) yield return shortCastleMove;

            var longCastleMoves = GetCastleMoves(board, currentTile, isShortCastle: false);
            if (longCastleMoves != null)
                foreach (var longCastleMove in longCastleMoves) yield return longCastleMove;
        }

        private IEnumerable<Move> GetCastleMoves(ChessBoard board, Tile kingTile, bool isShortCastle)
        {
            int direction = isShortCastle ? 1 : -1;
            int rookColumn = isShortCastle ? 7 : 0;
            int kingTargetColumn = kingTile.X + (2 * direction);

            var rookTile = new Tile(rookColumn, kingTile.Y);
            var rook = board.PieceAt(rookTile);

            if (rook is not Rook || rook.Team != Team)
                yield break;

            if (rook.HasMoved)
                yield break;

            int startColumn = kingTile.X + direction;
            int endColumn = isShortCastle ? rookTile.X - 1 : rookTile.X + 1;

            for (int col = startColumn; col != endColumn + direction; col += direction)
            {
                var checkTile = new Tile(col, kingTile.Y);
                if (!checkTile.IsEmpty)
                    yield break;
            }

            var enemyPieces = board.GetEnemyPieces(Team);
            var toTile = new Tile(kingTargetColumn, kingTile.Y);

            if (IsInCheck(enemyPieces, kingTile) ||
                IsInCheck(enemyPieces, new(kingTile.X + direction, kingTile.Y)) ||
                IsInCheck(enemyPieces, toTile))
                yield break;

            yield return new(toTile, this, () => CastleAction(toTile, rook, direction));
            if (!isShortCastle)
                yield return new(rookTile, this, () => CastleAction(toTile, rook, direction));
        }

        /// <summary>
        /// Returns if the king is in check when placed in position 'kingPosition'.
        /// </summary>
        public static bool IsInCheck(List<IPiece> enemyPieces, Tile kingPosition)
        {
            foreach (var enemyPiece in enemyPieces.ToList())
            {
                foreach (var move in enemyPiece.GetMoves())
                {
                    if (move.To == kingPosition)
                        return true;
                }
            }

            return false;
        }

        private void CastleAction(Tile to, IPiece rook, int direction)
        {
            var board = ServiceLocator.Get<ChessBoard>();

            board.Move(this, to);
            board.Move(rook, to + new Tile(-direction, 0));
        }
    }
}