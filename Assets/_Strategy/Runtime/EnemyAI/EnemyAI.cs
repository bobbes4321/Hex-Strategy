using System;
using System.Linq;
using _Strategy.Runtime.Board;
using _Strategy.Runtime.Deck;
using _Strategy.Runtime.Gameloop;
using _Strategy.Runtime.Piece;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using Runtime.Messaging;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IMessageHandler<TurnFinishedMessage>
{
    [Inject] private Board _board;
    [Inject] private MessagingManager _messagingManager;
    [Inject] private GameManager _gameManager;
    public EnemyPiece CurrentPiece;
    private void OnEnable() => _messagingManager.RegisterHandler(this);
    private void OnDisable() => _messagingManager.UnregisterHandler(this);

    public async UniTask DoMove()
    {
        await UniTask.WaitForSeconds(1f);
        var pieces = _board.GetPiecesOfTeam(Team.Black).ToList();
        foreach (var piece in pieces)
        {
            CurrentPiece = (EnemyPiece)piece;
            await UniTask.WaitForSeconds(0.5f);
            var moveCommand = ((EnemyPiece)piece).Settings.MoveCommand;
            DoMoveCommand(moveCommand, piece);
            await UniTask.WaitForSeconds(1f);

            var attackCommand = ((EnemyPiece)piece).Settings.AttackCommand;
            if (TryDoAttackCommand(attackCommand, piece))
                await UniTask.WaitForSeconds(1f);
        }

        CurrentPiece = null;
        _gameManager.FinishTurn();
    }

    private bool TryDoAttackCommand(ICommand command, IPiece piece)
    {
        var closestEnemyHex = FindClosestEnemyHex(piece);
        var hexes = command.GetHexes(piece).ToList();
        if (hexes.Contains(closestEnemyHex))
        {
            command.Execute(piece, closestEnemyHex);
            return true;
        }

        return false;
    }

    private void DoMoveCommand(ICommand command, IPiece piece)
    {
        var closestEnemyTile = FindClosestEnemyHex(piece);

        var tiles = command.GetHexes(piece).ToList();
        
        if (tiles.Count == 0)
            return;
        
        var closestTile = tiles[0];
        var closestMoveDistance = 1000;
        foreach (var tile in tiles)
        {
            var distance = (closestEnemyTile - tile).Magnitude;
            if (distance < closestMoveDistance)
            {
                closestTile = tile;
                closestMoveDistance = distance;
            }
        }

        command.Execute(piece, closestTile);
    }

    private Hex FindClosestEnemyHex(IPiece piece)
    {
        var enemyPieces = _board.GetEnemyPieces(Team.Black);

        if (enemyPieces.Count == 0)
            return piece.Hex;
        
        var closestEnemyTile = enemyPieces[0].Hex;
        var closestDistance = (closestEnemyTile - piece.Hex).Magnitude;

        foreach (var enemyPiece in enemyPieces)
        {
            var distance = (enemyPiece.Hex - piece.Hex).Magnitude;
            if (distance < closestDistance)
            {
                closestEnemyTile = enemyPiece.Hex;
                closestDistance = distance;
            }
        }

        return closestEnemyTile;
    }

    public void Handle(TurnFinishedMessage message)
    {
        if (GameManager.IsPlayersTurn)
            return;

        DoMove().Forget();
    }
}