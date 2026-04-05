using System;
using Runtime.Messaging;
using Sirenix.OdinInspector;

namespace Runtime.Chess
{
    [Serializable]
    public struct Tile : IEquatable<Tile>
    {
        [HorizontalGroup]
        public int X;
        [HorizontalGroup]
        public int Y;

        public static event Action<Tile> OnTileSelected;

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public bool IsEmpty => ServiceLocator.Get<ChessBoard>().PieceAt(this) is NullPiece;

        public bool Equals(Tile other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Tile other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public static bool operator ==(Tile left, Tile right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Tile left, Tile right)
        {
            return !left.Equals(right);
        }

        public static Tile operator +(Tile left, Tile right)
        {
            return new(left.X + right.X, left.Y + right.Y);
        }

        public void Select()
        {
            OnTileSelected?.Invoke(this);
        }
    }
}