using _Strategy.Runtime.Board;

namespace _Strategy.Runtime.Piece
{
    public interface IPiece
    {
        public string Id { get; set; }
        public Health Health { get; set; }
        public Team Team { get; set; }
        public Hex Hex { get; }
        void MoveTo(Hex hex);
        void Attack(Hex hexToAttack);
        void Die();
    }
    
    public static class PieceExtensions
    {
        public static bool IsNull(this IPiece piece) => piece is NullPiece;
        public static bool AreEnemies(this IPiece piece, IPiece other)
        { 
            if (other.Team == Team.None || piece.Team == Team.None) return true;
            return other.Team != piece.Team;
        }
    }
}