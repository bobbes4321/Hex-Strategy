using System.Collections.Generic;
using _Strategy.Runtime.Board;
using _Strategy.Runtime.Piece;
using Runtime.Messaging;
using UnityEngine;

namespace _Strategy.Runtime.Deck
{
    public abstract class AttackCommand : ICommand
    {
        [SerializeField] public float _damage;

        public abstract IEnumerable<Hex> GetHexes(IPiece piece);
        public virtual IEnumerable<Hex> GetHoverHexes(Hex hoveredHex)
        {
            yield break;
        }

        public virtual void Execute(IPiece piece, Hex selectedHex)
        {
            var board = DI.Resolve<Board.Board>();
            var pieceToAttack = board.PieceAt(selectedHex);

            if (pieceToAttack is not NullPiece)
            {
                piece.Attack(selectedHex);
                pieceToAttack.Health.DoDamage(_damage);
            }
            else
                Debug.Log($"[Deck.Execute] You missed.");
        }
    }
}