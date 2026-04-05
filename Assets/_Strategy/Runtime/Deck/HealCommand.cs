using System;
using System.Collections.Generic;
using _Strategy.Runtime.Board;
using _Strategy.Runtime.Gameloop;
using _Strategy.Runtime.Piece;
using UnityEngine;

namespace _Strategy.Runtime.Deck
{
    [Serializable]
    public class HealCommand : ICommand
    {
        [SerializeField] private float _healAmount = 10f;
        
        public IEnumerable<Hex> GetHexes(IPiece piece)
        {
            var board = DI.Resolve<Board.Board>();
            return board.GetHexesOfTeam(GameManager.CurrentTeam);
        }

        public IEnumerable<Hex> GetHoverHexes(Hex hoveredHex)
        {
            yield break;
        }

        public void Execute(IPiece piece, Hex selectedHex)
        {
            piece.Health.Heal(_healAmount);
        }
    }
}