using System;
using _Strategy.Runtime.Board;
using _Strategy.Runtime.Deck;
using _Strategy.Runtime.Gameloop;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Strategy.Runtime.Util
{
    public class Cheats : MonoBehaviour
    {
        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (GameManager.IsPlayersTurn)
                    FindAnyObjectByType<GameManager>().FinishTurn();
            }
            
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                var pieces = FindAnyObjectByType<Board.Board>().GetPiecesOfTeam(Team.Black);
                foreach (var piece in pieces)
                {
                    piece.Health.DoDamage(100000);
                }
            }
            
            if (Keyboard.current.f2Key.wasPressedThisFrame)
            {
                var pieces = FindAnyObjectByType<Board.Board>().GetPiecesOfTeam(Team.White);
                foreach (var piece in pieces)
                {
                    piece.Health.DoDamage(100000);
                }
            }
            
            if (Keyboard.current.f3Key.wasPressedThisFrame)
                FindAnyObjectByType<Hand>().ResetHand();
            
            if (Keyboard.current.f4Key.wasPressedThisFrame)
                FindAnyObjectByType<AP>().Reset();
        }
    }
}