using System;
using _Strategy.Runtime.Gameloop;
using Reflex.Attributes;
using Runtime.Messaging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Strategy.Runtime.Deck
{
    public class AP : MonoBehaviour, IMessageHandler<TurnFinishedMessage>
    {
        [Inject] private MessagingManager _messagingManager;
        [Inject] private PlayerData _playerData;
        private int _current = 5;

        public int Current
        {
            get => _current;
            private set
            {
                _current = value;
                OnCurrentApChanged?.Invoke();
            }
        }

        public int Max => _playerData.MaxAP;

        public event Action OnCurrentApChanged;

        private void Start()
        {
            Current = Max;
        }

        private void OnEnable() => _messagingManager.RegisterHandler(this);
        private void OnDisable() => _messagingManager.UnregisterHandler(this);

        [Button]
        public void Reset() => Current = Max;

        [Button]
        public void Remove(int amount) => Current -= amount;

        public void Handle(TurnFinishedMessage message) => Reset();
    }
}