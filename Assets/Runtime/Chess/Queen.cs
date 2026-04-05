using System.Collections.Generic;

namespace Runtime.Chess
{
    public class Queen : Piece
    {
        public override IEnumerable<Move> GetMoves()
        {
            foreach (var upLeftTiles in MoveHelper.GetMovesInDirection(this, Direction.UpLeft)) yield return upLeftTiles;
            foreach (var upRightTiles in MoveHelper.GetMovesInDirection(this, Direction.UpRight)) yield return upRightTiles;
            foreach (var downLeftTiles in MoveHelper.GetMovesInDirection(this, Direction.DownLeft)) yield return downLeftTiles;
            foreach (var downRightTiles in MoveHelper.GetMovesInDirection(this, Direction.DownRight)) yield return downRightTiles;
            foreach (var upTiles in MoveHelper.GetMovesInDirection(this, Direction.Up)) yield return upTiles;
            foreach (var downTiles in MoveHelper.GetMovesInDirection(this, Direction.Down)) yield return downTiles;
            foreach (var leftTiles in MoveHelper.GetMovesInDirection(this, Direction.Left)) yield return leftTiles;
            foreach (var rightTiles in MoveHelper.GetMovesInDirection(this, Direction.Right)) yield return rightTiles;
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
    }
}