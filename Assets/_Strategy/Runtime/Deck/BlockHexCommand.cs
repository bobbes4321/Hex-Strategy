using System;
using System.Collections.Generic;
using _Strategy.Runtime.Board;
using _Strategy.Runtime.Piece;
using UnityEngine;

namespace _Strategy.Runtime.Deck
{
    [Serializable]
    public class BlockHexCommand : ICommand
    {
        [SerializeField] private int _amountOfTurns = 3;
        
        public IEnumerable<Hex> GetHexes(IPiece piece)
        {
            var board = DI.Resolve<Board.Board>();
            return board.GetEmptyHexes();
        }

        public IEnumerable<Hex> GetHoverHexes(Hex hoveredHex)
        {
            yield break;
        }

        public void Execute(IPiece piece, Hex selectedHex)
        {
            var hexBlockManager = DI.Resolve<HexBlockManager>();
            hexBlockManager.CreateBlock(selectedHex, _amountOfTurns);
            Debug.Log($"[HealCommand.Execute] Blocked {selectedHex} for {_amountOfTurns} turns.");
        }
    }
}