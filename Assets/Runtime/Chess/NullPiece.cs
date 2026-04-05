using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Chess
{
    [Serializable]
    public struct NullPiece : IPiece
    {
        public List<IPieceComponent> Components => new();

        public Team Team
        {
            get => Team.None;
            set => Debug.Log(value);
        }

        public bool HasMoved { get; }
        public IEnumerable<Move> GetMoves() => Enumerable.Empty<Move>();
        public void MoveTo(Tile tile) => Debug.Log("[NullPiece.Move]");
        public void GetCaptured() { }
    }
}