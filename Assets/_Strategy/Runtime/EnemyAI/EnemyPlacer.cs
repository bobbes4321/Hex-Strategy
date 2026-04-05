using System.Collections.Generic;
using _Strategy.Runtime.Board;
using _Strategy.Runtime.Piece;
using Doozy.Runtime.Common.Extensions;
using Reflex.Attributes;
using UnityEngine;

public class EnemyPlacer : MonoBehaviour
{
    [Inject] private Board _board;
    [Inject] private List<Piece> _pieces;

    private void Start()
    {
        foreach (var piece in _pieces)
        {
            var hex = new Hex();
            var hasFoundTile = false;
            while (!hasFoundTile)
            {
                hex = GetHex();
                if (hex.IsEmpty)
                    hasFoundTile = true;
            }

            _board.AddPiece(piece, hex);

            var spawnedPiece = Instantiate(piece, _board.transform);
            spawnedPiece.transform.position = Board.HexToWorld(hex);
        }
    }

    private Hex GetHex() => _board.GetRandomHexOfTeam(Team.Black);
}