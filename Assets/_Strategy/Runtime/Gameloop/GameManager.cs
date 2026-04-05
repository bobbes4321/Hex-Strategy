using System;
using _Strategy.Runtime.Board;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Components;
using Reflex.Attributes;
using Runtime.Messaging;
using UnityEngine;

namespace _Strategy.Runtime.Gameloop
{
    public class GameManager : MonoBehaviour
    {
        [ClearOnReload(resetValue: Team.White)]
        private static Team _currentTeam = Team.White;

        public static Team CurrentTeam => _currentTeam;
        public static bool IsPlayersTurn => _currentTeam == Team.White;
        public string TurnText => _currentTeam == Team.White ? "White To Play" : "Black To Play";

        [Inject] private Board.Board _board;
        [Inject] private LevelManager _levelManager;
        [Inject] private MessagingManager _messagingManager;
        [Inject] private LevelSettings _levelSettings;

        private void Awake() => _currentTeam = Team.White;

        public void FinishTurn()
        {
            _currentTeam = _currentTeam.GetOther();
            if (CheckIfGameOver()) return;

            _messagingManager.Publish(new TurnFinishedMessage());
            SignalPayload payload = new()
            {
                streamId = new(nameof(UIButtonId.Strategy), "TurnFinished"),
            };

            payload.SendSignal();
        }

        public bool CheckIfGameOver()
        {
            bool isGameOver = IsGameOver(out var winner);
            if (isGameOver)
                _messagingManager.Publish(new GameOverMessage(winner));

            return isGameOver;
        }

        private bool IsGameOver(out Team winner)
        {
            if (_board.AreAllPiecesTaken(Team.White))
            {
                winner = Team.Black;
                return true;
            }

            if (_board.AreAllPiecesTaken(Team.Black))
            {
                winner = Team.White;
                return true;
            }

            winner = Team.None;
            return false;
        }
    }

    public struct TurnFinishedMessage : IMessage
    {
        
    }
}