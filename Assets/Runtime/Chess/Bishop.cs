using System.Collections.Generic;

namespace Runtime.Chess
{
    public class Bishop : Piece
    {
        public override IEnumerable<Move> GetMoves()
        {
            foreach (var upLeftTiles in MoveHelper.GetMovesInDirection(this, Direction.UpLeft)) yield return upLeftTiles;
            foreach (var upRightTiles in MoveHelper.GetMovesInDirection(this, Direction.UpRight)) yield return upRightTiles;
            foreach (var downLeftTiles in MoveHelper.GetMovesInDirection(this, Direction.DownLeft)) yield return downLeftTiles;
            foreach (var downRightTiles in MoveHelper.GetMovesInDirection(this, Direction.DownRight)) yield return downRightTiles;
        }
    }
}