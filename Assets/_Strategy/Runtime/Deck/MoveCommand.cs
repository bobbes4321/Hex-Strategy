using System.Collections.Generic;
using _Strategy.Runtime.Board;
using _Strategy.Runtime.Piece;
using UnityEngine;

namespace _Strategy.Runtime.Deck
{
    public abstract class MoveCommand : ICommand
    {
        public abstract IEnumerable<Hex> GetHexes(IPiece piece);
        public IEnumerable<Hex> GetHoverHexes(Hex hoveredHex)
        {
            yield break;
        }

        public void Execute(IPiece piece, Hex selectedHex)
        {
            DI.Resolve<Board.Board>().Move(piece, selectedHex);
        }
    }
}