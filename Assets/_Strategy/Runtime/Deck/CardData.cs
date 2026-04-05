using System.Collections.Generic;
using _Strategy.Runtime.Board;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Strategy.Runtime.Deck
{
    [CreateAssetMenu(menuName = "Strategy/Create CardData", fileName = "CardData", order = 0)]
    public class CardData : Settings.Settings
    {
        [BoxGroup("Text")]
        public string Name;
        [BoxGroup("Text")]
        [TextArea(3, 10)] public string Description;

        [PreviewField(ObjectFieldAlignment.Center, Height = 150)]
        public Sprite Icon;

        [BoxGroup("Settings")]
        public int Cost = 1;
        [BoxGroup("Settings")]
        public Rarity Rarity;

        [FoldoutGroup("Data")]
        public ICommand Command;

        [FoldoutGroup("Data")]
        public CardProcessor Processor;
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public static class RarityExtensions
    {
        public static Color GetColor(this Rarity rarity)
        {
            return DI.Resolve<RaritySettings>().GetColor(rarity);
        }
    }
}