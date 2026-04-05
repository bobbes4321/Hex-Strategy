using System;
using System.Collections.Generic;
using System.Linq;
using _Strategy.Runtime.Piece;
using Doozy.Runtime.Common.Extensions;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Strategy.Runtime.Board
{
    public class Board : MonoBehaviour
    {
        public const float HexSize = 0.7f;

        [Inject] private IHexGenerator _hexGenerator;

        private readonly List<IPiece> _enemyPieces = new();
        private readonly Dictionary<Hex, IPiece> _pieces = new();
        private static Vector3 _bottomLeftPosition = Vector3.zero;
        public List<Hex> Hexes => _pieces.Keys.ToList();
        public event Action<Hex> OnHexGenerated;

        public int Width => Hexes.Max(x => x.Q) + 1;
        public int Height => Hexes.Max(x => x.R) + 1;
        public int Bottom => Hexes.Min(x => x.R) + 1;

        private void Start() => CreateBoard(_hexGenerator);

        private void CreateBoard(IHexGenerator hexGenerator)
        {
            _pieces.Clear();

            var hexes = hexGenerator.Generate();
            Debug.Log($"[Board.CreateBoard] Generated {hexes.Length} hexes", this);
            foreach (var hex in hexes)
            {
                _pieces.Add(hex, new NullPiece());
                OnHexGenerated?.Invoke(hex);
            }
        }

        public void AddPiece(IPiece piece, Hex hex)
        {
            if (_pieces.ContainsKey(hex)) _pieces[hex] = piece;
        }

        public IPiece PieceAt(Hex hex) => _pieces.TryGetValue(hex, out var piece) ? piece : new NullPiece();

        public Hex HexOf(IPiece piece)
        {
            foreach (var kvp in _pieces)
            {
                if (kvp.Value.Equals(piece))
                {
                    return kvp.Key;
                }
            }

            Debug.LogError($"[Board.HexOf] Could not find the hex for the given piece: {piece}");
            return new Hex(-100, -100);
        }

        public static Hex WorldToHex(Vector3 worldPositionHex)
        {
            Vector3 localPosition = worldPositionHex - _bottomLeftPosition;

            var x = localPosition.x / HexSize;
            var y = localPosition.z / HexSize;
            var q = (Mathf.Sqrt(3) / 3 * x - 1f / 3 * y);
            var r = (2f / 3 * y);
            return new Hex(Mathf.RoundToInt(q), Mathf.RoundToInt(r));
        }

        public static Vector3 HexToWorld(Hex hex)
        {
            var x = Mathf.Sqrt(3) * hex.Q + Mathf.Sqrt(3) / 2 * hex.R;
            var y = 3f / 2 * hex.R;
            x *= HexSize;
            y *= HexSize;
            return _bottomLeftPosition + new Vector3(x, 0, y);
        }

        public bool IsHexAt(Hex hex) => _pieces.TryGetValue(hex, out _);

        public void Move(IPiece piece, Hex hex)
        {
            var fromHex = HexOf(piece);

            RemoveAt(fromHex);
            AddPiece(piece, hex);

            piece.MoveTo(hex);
        }

        [Button]
        public void RemoveAt(Hex hex)
        {
            if (_pieces.ContainsKey(hex))
                _pieces[hex] = new NullPiece();
        }

        public void RemoveHex(Hex hex) => _pieces.Remove(hex);
        public void AddHex(Hex hex) => _pieces.Add(hex, new NullPiece());

        public List<IPiece> GetPiecesOfTeam(Team pieceTeam)
        {
            if (pieceTeam == Team.None)
            {
                Debug.LogError("Can't get pieces of team None!", this);
                return null;
            }

            _enemyPieces.Clear();
            foreach (var kvp in _pieces)
            {
                var piece = kvp.Value;
                if (piece.Team != Team.None && piece.Team == pieceTeam)
                {
                    _enemyPieces.Add(piece);
                }
            }

            return _enemyPieces;
        }

        public IEnumerable<Hex> GetHexesOfTeam(Team pieceTeam) => GetPiecesOfTeam(pieceTeam).Select(HexOf);

        public bool AreAllPiecesTaken(Team team) => GetPiecesOfTeam(team) is not { Count: > 0 };
        public List<IPiece> GetEnemyPieces(Team pieceTeam) => GetPiecesOfTeam(pieceTeam.GetOther());

        public Hex GetRandomHexOfTeam(Team team)
        {
            int highestY = Hexes.Max(x => x.Q);
            var hexesOfTeam = new List<Hex>();
            if (team == Team.Black)
                hexesOfTeam = Hexes.Where(x => x.R > highestY / 2).ToList();
            else if (team == Team.White)
                hexesOfTeam = Hexes.Where(x => x.R < highestY / 2).ToList();

            return hexesOfTeam.GetRandomItem();
        }

        public IEnumerable<Hex> GetEmptyHexes()
        {
            return Hexes.Where(x => x.IsEmpty);
        }
    }
}