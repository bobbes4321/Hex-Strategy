using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Signals;
using Runtime.Messaging;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Chess
{
    public class GameManager : MonoBehaviour, IMessageHandler<MoveMessage>, IMessageHandler<GameOverMessage>
    {
        [ClearOnReload(resetValue: Team.White)]
        private static Team _currentTeam = Team.White;
        public static Team CurrentTeam => _currentTeam;
        public string TurnText => _currentTeam == Team.White ? "Your turn" : "Enemy turn";

        private void Awake()
        {
            _currentTeam = Team.White;
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.Get<MessagingManager>().RegisterHandler(this);
        }

        private void OnDestroy()
        {
            var messagingManager = ServiceLocator.Get<MessagingManager>();
            
            if (messagingManager)
                messagingManager.UnregisterHandler(this);
            
            ServiceLocator.Unregister<GameManager>();
        }

        public void Handle(MoveMessage message)
        {
            if (_currentTeam == Team.White)
                _currentTeam = Team.Black;
            else _currentTeam = Team.White;
            
            CheckGameState();
        }
        
        private void CheckGameState()
        {
            if (IsGameOver(out var winner))
            {
                SignalPayload payload = new()
                {
                    streamId = new StreamId("Strategy", "GameOver"),
                    booleanValue = winner == Team.White
                };
                payload.SendSignal();
            }
        }

        private bool IsGameOver(out Team winner)
        {
            var board = ServiceLocator.Get<ChessBoard>();
            if (board.AreAllPiecesTaken(Team.White))
            {
                winner = Team.Black;
                return true;
            }

            if (board.AreAllPiecesTaken(Team.Black))
            {
                winner = Team.White;
                return true;
            }

            winner = Team.None;
            return false;
        }

        public void Handle(GameOverMessage message)
        {
            Debug.Log($"[GameManager.Handle] The {message.WinningTeam} king was taken. The other player is victorious!", this);
            _currentTeam = Team.None;
        }
    }
}