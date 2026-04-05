using _Strategy.Runtime.Board;
using _Strategy.Runtime.Gameloop;
using _Strategy.Runtime.Piece;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerPiecePlacer : MonoBehaviour
{
    [SerializeField] private Piece _piece;
    [Inject] private Board _board;
    [Inject] private LevelManager _levelManager;
    
    [SerializeField, ReadOnly]
    [Inject] private PlayerData _playerData;
    
    private void Start()
    {
        _playerData = _levelManager.PlayerData;
        
        int boardWidth = _board.Width;
        int boardHeight = _board.Height;
        int pieceCount = _playerData.PieceCount;
        Debug.Log($"[PlayerPiecePlacer.Start] PieceCount: {pieceCount}", this);
        int currentRow = _board.Bottom;

        int availableWidth = boardWidth - 2;
        int startColumn = 1 + (availableWidth - pieceCount) / 2;

        for (int i = 0; i < pieceCount; i++)
        {
            bool piecePlaced = false;

            while (!piecePlaced && currentRow < boardHeight - 1)
            {
                for (int column = startColumn + i; column < boardWidth - 1; column++)
                {
                    var hex = new Hex(column, currentRow);
                    if (hex is { IsValid: true, IsEmpty: true })
                    {
                        Place(_board, _piece, hex);
                        piecePlaced = true;
                        break;
                    }
                }

                if (!piecePlaced)
                {
                    for (int column = 1; column < startColumn + i; column++)
                    {
                        var hex = new Hex(column, currentRow);
                        if (hex is { IsValid: true, IsEmpty: true })
                        {
                            Place(_board, _piece, hex);
                            piecePlaced = true;
                            break;
                        }
                    }
                }

                if (!piecePlaced)
                {
                    currentRow++;
                    startColumn = 1 + (availableWidth - (pieceCount - i)) / 2;
                }
            }

            if (!piecePlaced)
            {
                Debug.LogWarning($"Could not find valid placement for piece {i}");
            }
        }
    }

    public void Place(Board board, Piece piece, Hex hex)
    {
        board.AddPiece(piece, hex);

        var spawnedPiece = Instantiate(piece, board.transform);
        spawnedPiece.transform.position = Board.HexToWorld(hex);

        spawnedPiece.OnDied += HandleDied;
    }

    private void HandleDied(Piece piece)
    {
        piece.OnDied -= HandleDied;
        _playerData.PieceCount--;
    }
}