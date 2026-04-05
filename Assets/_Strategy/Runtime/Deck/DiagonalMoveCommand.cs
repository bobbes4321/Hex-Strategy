using System;
using System.Collections.Generic;
using System.Linq;
using _Strategy.Runtime.Board;
using _Strategy.Runtime.Piece;
using UnityEngine;

namespace _Strategy.Runtime.Deck
{
    [Serializable]
    public class DiagonalMoveCommand : MoveCommand
    {
        [SerializeField] private int _range = 5;
        private static Direction[] _directions = { Direction.Left, Direction.Right, Direction.DownLeft, Direction.DownRight, Direction.UpLeft, Direction.UpRight };

        public override IEnumerable<Hex> GetHexes(IPiece piece)
        {
            return _directions.SelectMany(dir => MoveHelper.GetDiagonalMovesInDirection(piece, dir, _range, true, MoveHelper.IsEmpty));
        }
    }
    
    [Serializable]
    public class OmniDirectionalMoveCommand : MoveCommand
    {
        [SerializeField] private int _range = 5;
        private static Direction[] _directions = { Direction.Left, Direction.Right, Direction.UpLeft, Direction.UpRight, Direction.DownLeft, Direction.DownRight };

        public override IEnumerable<Hex> GetHexes(IPiece piece)
        {
            return _directions.SelectMany(dir => MoveHelper.GetMovesInDirection(piece, dir, _range, true, MoveHelper.IsEmpty));
        }
    }
    
    [Serializable]
    public class FlexibleMoveCommand : MoveCommand
    {
        [SerializeField] private List<Movement> _movements;

        public override IEnumerable<Hex> GetHexes(IPiece piece)
        {
            return _movements.SelectMany(movement => MoveHelper.GetMovesInDirection(piece, movement.Direction, movement.Range, true, MoveHelper.IsEmpty));
        }
    }

    [Serializable]
    public class Movement
    {
        public Direction Direction;
        public int Range;
    }
}