using System.Collections.Generic;
using System.Linq;

namespace Runtime.Chess
{
    public interface IPiece
    {
        public List<IPieceComponent> Components { get; }
        public Team Team { get; set; }
        bool HasMoved { get; }
        IEnumerable<Move> GetMoves();
        void MoveTo(Tile tile);
        void GetCaptured();

        bool AreEnemies(IPiece piece)
        {
            if (piece.Team == Team.None || Team == Team.None) return true;
            return piece.Team != Team;
        }

        bool IsNull() => this is NullPiece;
        T Get<T>() where T : class, IPieceComponent => Components.FirstOrDefault(x => x is T) as T;
    }
}