using System.Collections.Generic;
using Runtime.Messaging;
using UnityEngine;

namespace Runtime.Chess
{
    public abstract class Piece : MonoBehaviour, IPiece
    {
        [SerializeField] private Team _team;
        public bool HasMoved { get; private set; }
        public Tile Tile => ChessBoard.WorldToTile(transform.position);

        [SerializeField] private List<IPieceComponent> _components = new List<IPieceComponent>();
        public List<IPieceComponent> Components => _components;

        public Team Team
        {
            get => _team;
            set => _team = value;
        }

        private void Start() => ServiceLocator.Get<ChessBoard>().AddPiece(this, Tile);
        public abstract IEnumerable<Move> GetMoves();

        public void MoveTo(Tile tile)
        {
            var newPosition = ChessBoard.TileToWorld(tile);
            transform.position = newPosition;
            HasMoved = true;
        }

        public virtual void GetCaptured() => gameObject.SetActive(false);
        public override string ToString() => $"(Type: {GetType().Name} | Tile: {Tile} | Team: {Team})";
    }
}