using System;
using System.Linq;
using _Strategy.Runtime.Board;
using Cysharp.Threading.Tasks;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Components;
using Reflex.Attributes;
using Runtime.Messaging;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Strategy.Runtime.Gameloop
{
    public class LevelManager : MonoBehaviour, IMessageHandler<GameOverMessage>
    {
        [Inject] private LevelSettings _levelSettings;
        [Inject] private MessagingManager _messagingManager;
        [ShowInInspector] public LevelData CurrentLevel => _levelSettings.Levels[_currentLevelIndex];
        [SerializeField, ReadOnly] private int _currentLevelIndex = 0;
        [SerializeField] private PlayerData _defaultPlayerData = new(5, 5, 2);
        [ShowInInspector, ReadOnly] public PlayerData PlayerData { get; private set; }

        private void Awake() => ResetPlayerData();

        private void Start()
        {
            DontDestroyOnLoad(this);
            _messagingManager.RegisterHandler(this);
        }

        private void OnDestroy() => _messagingManager.UnregisterHandler(this);

        private void LoadLevel(int newLevelIndex)
        {
            _currentLevelIndex = newLevelIndex;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log($"[GameManager.NextLevel] Loading level {_currentLevelIndex}", this);
        }

        public void Handle(GameOverMessage message)
        {
            bool hasPlayerWon = message.WinningTeam == Team.White;
            //Option 1: Player Lost
            if (message.WinningTeam != Team.White)
            {
                WaitUntilRestart().Forget();
                SendGameOverSignal(hasPlayerWon);
            }
            //Option 2: Player Won, but not at final level yet
            else if (_currentLevelIndex < _levelSettings.Levels.Count - 1)
            {
                WaitUntilNextLevel().Forget();
                SendGameOverSignal(hasPlayerWon);
            }
            //Option 3: Player Won at final level, so show game won screen
            else
            {
                WaitUntilRestart().Forget();
                SignalPayload payload = new(streamId: new(nameof(UIButtonId.Strategy), "GameWon"));
                payload.SendSignal();
            }
        }

        private static void SendGameOverSignal(bool hasPlayerWon)
        {
            SignalPayload payload = new()
            {
                streamId = new(nameof(UIButtonId.Strategy), "GameOver"),
                booleanValue = hasPlayerWon
            };

            payload.SendSignal();
        }

        private async UniTask WaitUntilNextLevel()
        {
            var nextLevelButton = UIButton.GetButton(nameof(UIButtonId.Strategy), nameof(UIButtonId.Strategy.Continue));
            await nextLevelButton.OnClickAsync();
            LoadLevel(_currentLevelIndex + 1);
        }

        private async UniTask WaitUntilRestart()
        {
            var restartButtons = UIButton.GetButtons(nameof(UIButtonId.Strategy), nameof(UIButtonId.Strategy.Restart));
            await UniTask.WhenAny(restartButtons.Select(button => button.OnClickAsync()).ToArray());
            ResetPlayerData();
            LoadLevel(0);
        }
        
        private void ResetPlayerData() => PlayerData = _defaultPlayerData.Copy();
    }

    [Serializable]
    public class PlayerData
    {
        public int MaxCardsInHand;
        public int MaxAP;
        public int PieceCount;
        
        public PlayerData Copy() => new(MaxCardsInHand, MaxAP, PieceCount);

        public PlayerData(int maxCardsInHand, int maxAP, int pieceCount)
        {
            MaxCardsInHand = maxCardsInHand;
            MaxAP = maxAP;
            PieceCount = pieceCount;
        }
    }
}