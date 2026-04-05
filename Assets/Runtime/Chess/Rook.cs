using System.Collections.Generic;
using System.Linq;

namespace Runtime.Chess
{
    public class Rook : Piece
    {
        private static Direction[] _directions = { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

        public override IEnumerable<Move> GetMoves()
        {
            return _directions.SelectMany(dir => MoveHelper.GetMovesInDirection(this, dir));
        }
    }
}