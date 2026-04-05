using System.Collections.Generic;

namespace Runtime.Chess
{
    public class Knight : Piece
    {
        private static (int x, int y)[] _knightMoves =
        {
            (2, 1), (2, -1), (-2, 1), (-2, -1),
            (1, 2), (1, -2), (-1, 2), (-1, -2)
        };

        public override IEnumerable<Move> GetMoves()
        {
            foreach (var (dx, dy) in _knightMoves)
            {
                var move = MoveHelper.GetMoveTo(new(dx, dy), this);
                if (move != default)
                    yield return move;
            }
        }
    }
}