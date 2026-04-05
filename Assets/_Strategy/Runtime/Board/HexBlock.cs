using PrimeTween;
using UnityEngine;

namespace _Strategy.Runtime.Board
{
    public class HexBlock
    {
        public int TurnsLeft;
        public GameObject SpawnedBlock;
        public Hex Hex;

        public HexBlock(int turnsLeft, GameObject blockVisuals, Hex hex)
        {
            TurnsLeft = turnsLeft;
            SpawnedBlock = blockVisuals;
            Hex = hex;
        }

        public void DecrementTurnsLeft()
        {
            TurnsLeft--;
            Debug.Log($"[BlockedHexes.DecrementTurnsLeft] Turns left: {TurnsLeft} for {Hex}", SpawnedBlock);
        }
    }
}