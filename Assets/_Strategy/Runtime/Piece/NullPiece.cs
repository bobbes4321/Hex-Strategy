using System;
using _Strategy.Runtime.Board;
using UnityEngine;

namespace _Strategy.Runtime.Piece
{
    [Serializable]
    public struct NullPiece : IPiece
    {
        public string Id { get; set; }
        public Health Health { get; set; }

        public Team Team
        {
            get => Team.None;
            set => Debug.Log(value);
        }

        public Hex Hex { get; }
        public void MoveTo(Hex hex) => Debug.Log("[NullPiece.Move]");
        public void Attack(Hex hexToAttack)
        {
            throw new NotImplementedException();
        }

        public void Die() => Debug.Log($"[NullPiece.Die]");
        public override string ToString()
        {
            return "NullPiece";
        }
    }
}