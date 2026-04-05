using System;
using System.Collections.Generic;
using System.Linq;
using _Strategy.Runtime.Board;
using _Strategy.Runtime.Piece;
using UnityEngine;

namespace _Strategy.Runtime.Deck
{
    [Serializable]
    public class RangedAttack : AttackCommand
    {
        [SerializeField] private int _range = 3;

        private static Direction[] _directions = { Direction.Left, Direction.Right, Direction.DownLeft, Direction.DownRight, Direction.UpLeft, Direction.UpRight };

        public override IEnumerable<Hex> GetHexes(IPiece piece)
        {
            return _directions.SelectMany(dir => MoveHelper.GetMovesInDirection(piece, dir, _range, true, MoveHelper.IsEmpty, MoveHelper.CanCapture));
        }
    }

    [Serializable]
    public class AoeAttack : AttackCommand
    {
        private static Direction[] _directions = { Direction.Left, Direction.Right, Direction.DownLeft, Direction.DownRight, Direction.UpLeft, Direction.UpRight };
        [SerializeField] private int _range = 3;

        public override IEnumerable<Hex> GetHexes(IPiece piece)
        {
            return _directions.SelectMany(dir => MoveHelper.GetMovesInDirection(piece, dir, _range, true, MoveHelper.IsEmpty, MoveHelper.CanCapture));
        }

        public override IEnumerable<Hex> GetHoverHexes(Hex hoveredHex)
        {
            yield return hoveredHex;
            foreach (var hex in hoveredHex.AllNeighbors())
            {
                yield return hex;
            }
        }

        public override void Execute(IPiece piece, Hex selectedHex)
        {
            var board = DI.Resolve<Board.Board>();
            
            foreach (var neighbors in selectedHex.AllNeighbors())
            {
                InflictDamageOnPiece(board, neighbors);
            }

            InflictDamageOnPiece(board, selectedHex);
            piece.Attack(selectedHex);
        }

        private void InflictDamageOnPiece(Board.Board board, Hex hex)
        {
            var pieceToAttack = board.PieceAt(hex);

            if (pieceToAttack is not NullPiece)
                pieceToAttack.Health.DoDamage(_damage);
        }
    }
}