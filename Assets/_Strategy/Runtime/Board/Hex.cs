using System;
using System.Collections.Generic;
using _Strategy.Runtime.Piece;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Strategy.Runtime.Board
{
    [Serializable]
    public struct Hex : IEquatable<Hex>
    {
        [ReadOnly]
        public int Q, R;
        [ShowInInspector, ReadOnly]
        public int S => -Q - R;

        public Hex(int q, int r)
        {
            Q = q;
            R = r;
        }

        public int Magnitude => (Mathf.Abs(Q) + Mathf.Abs(R) + Mathf.Abs(S)) / 2;
        public static event Action<Hex> OnHexSelected;
        public void Select() => OnHexSelected?.Invoke(this);
        public bool IsValid => DI.Resolve<Board>().IsHexAt(this);
        public bool IsEmpty => DI.Resolve<Board>().PieceAt(this) is NullPiece;
        public bool Equals(Hex other) => Q == other.Q && R == other.R && S == other.S;

        public int CompareTo(Hex other)
        {
            if (other.Magnitude < Magnitude) return -1;
            if (other.Magnitude > Magnitude) return 1;
            return 0;
        }

        public override bool Equals(object obj) => obj is Hex other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Q, R, S);
        public override string ToString() => $"{Q}, {R}, {S}";
        public static bool operator ==(Hex left, Hex right) => left.Equals(right);
        public static bool operator !=(Hex left, Hex right) => !left.Equals(right);
        public static Hex operator +(Hex left, Hex right) => new(left.Q + right.Q, left.R + right.R);
        public static Hex operator -(Hex left, Hex right) => new(left.Q - right.Q, left.R - right.R);
        public static Hex operator *(Hex left, int factor) => new(left.Q * factor, left.R * factor);
        public Hex RotateLeft() => new( -Q, -R);
        public Hex RotateRight() => new(-R, -Q);
        public static Hex Direction(Direction direction) => Directions[(int)direction];
        public Hex Neighbor(Direction direction, int magnitude = 1) => this + Direction(direction) * magnitude;
        public Hex Neighbor(Hex direction, int magnitude = 1) => this + direction * magnitude;

        public IEnumerable<Hex> AllNeighbors()
        {
            foreach (var direction in Directions)
            {
                yield return Neighbor(direction);
            }
        }
        public Hex DiagonalNeighbor(Direction direction, int magnitude = 1) => this + Diagonals[(int)direction] * magnitude;
        public int Distance(Hex b) => Magnitude - b.Magnitude;
        public static List<Hex> Directions = new List<Hex> { new Hex(1, 0), new Hex(1, -1), new Hex(0, -1), new Hex(-1, 0), new Hex(-1, 1), new Hex(0, 1) };
        public static List<Hex> Diagonals = new List<Hex> { new Hex(2, -1), new Hex(1, -2), new Hex(-1, -1), new Hex(-2, 1), new Hex(-1, 2), new Hex(1, 1) };
    }
}