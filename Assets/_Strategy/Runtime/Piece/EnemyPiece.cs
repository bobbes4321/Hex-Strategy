using System;
using _Strategy.Runtime.Deck;
using _Strategy.Runtime.Settings;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine.EventSystems;

namespace _Strategy.Runtime.Piece
{
    public class EnemyPiece : Piece
    {
        public EnemySettings Settings;

        protected override void Awake()
        {
            Health = new Health();
            Health.SetMaxHealth(Settings.Health);
            base.Awake();
        }
    }
}