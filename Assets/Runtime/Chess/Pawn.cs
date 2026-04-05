using System.Collections.Generic;

namespace Runtime.Chess
{
    public class Pawn : Piece
    {
        public override IEnumerable<Move> GetMoves()
        {
            int distance = 1;

            // First move - two squares
            if ((Team == Team.White && Tile.Y == 1) || (Team == Team.Black && Tile.Y == 6))
            {
                distance = 2;
            }

            foreach (var move in MoveHelper.GetMovesInDirection(this, Team == Team.White ? Direction.Up : Direction.Down, distance, MoveHelper.IsEmpty))
                yield return move;

            foreach (var move in MoveHelper.GetMovesInDirection(this, Team == Team.White ? Direction.UpRight : Direction.DownRight, 1, MoveHelper.CanCapture))
                yield return move;

            foreach (var move in MoveHelper.GetMovesInDirection(this, Team == Team.White ? Direction.UpLeft : Direction.DownLeft, 1, MoveHelper.CanCapture))
                yield return move;
        }
    }
}
