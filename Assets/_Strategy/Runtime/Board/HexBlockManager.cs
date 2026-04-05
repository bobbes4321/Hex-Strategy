using System.Collections.Generic;
using _Strategy.Runtime.Gameloop;
using PrimeTween;
using Reflex.Attributes;
using Runtime.Messaging;
using UnityEngine;

namespace _Strategy.Runtime.Board
{
    public class HexBlockManager : MonoBehaviour, IMessageHandler<TurnFinishedMessage>
    {
        [Inject] private Board _board;
        [Inject] private MessagingManager _messagingManager;

        [SerializeField] private GameObject _blockVisuals;

        private List<HexBlock> _blockedHexes = new();

        private void Start()
        {
            _messagingManager.RegisterHandler(this);
        }

        private void OnDestroy()
        {
            _messagingManager.UnregisterHandler(this);
        }

        public void CreateBlock(Hex hex, int amountOfTurnsToBlock)
        {
            _board.RemoveHex(hex);
            
            var spawnedBlock = Instantiate(_blockVisuals, _board.transform);
            spawnedBlock.transform.position = Board.HexToWorld(hex);
            spawnedBlock.transform.localScale = Vector3.one * Board.HexSize;
            _blockedHexes.Add(new HexBlock(amountOfTurnsToBlock, spawnedBlock, hex));
        }

        public void Handle(TurnFinishedMessage message)
        {
            if (!GameManager.IsPlayersTurn)
                return;
            
            for (int i = _blockedHexes.Count - 1; i >= 0; i--)
            {
                var x = _blockedHexes[i];
                x.DecrementTurnsLeft();

                if (x.TurnsLeft <= 0)
                {
                    var spawnedBlock = x.SpawnedBlock;
                    
                    _blockedHexes.RemoveAt(i);
                    _board.AddHex(x.Hex);
                    
                    Tween.Scale(spawnedBlock.transform, Vector3.zero, 0.5f, Ease.InBounce)
                        .OnComplete(() => Destroy(spawnedBlock));
                }
            }
        }
    }
}