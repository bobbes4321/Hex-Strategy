using _Strategy.Runtime.Gameloop;
using Reflex.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Strategy.Runtime.Deck
{
    [CreateAssetMenu(menuName = "Strategy/Create UpgradeCardData", fileName = "UpgradeCardData", order = 0)]
    public class UpgradeCardData : ScriptableObject
    {
        public string Name;
        [TextArea(3, 10)] public string Description;

        [PreviewField(ObjectFieldAlignment.Center, Height = 150)]
        public Sprite Icon;

        [SerializeReference] public IUpgrade Upgrade;
    }

    public class UpgradePiece : IUpgrade
    {
        public void Execute(Container container) => container.Resolve<PlayerData>().PieceCount++;
    }
    
    public class UpgradeHandSize : IUpgrade
    {
        public void Execute(Container container) => container.Resolve<PlayerData>().MaxCardsInHand++;
    }
    
    public class UpgradeAbilityPoint : IUpgrade
    {
        public void Execute(Container container) => container.Resolve<PlayerData>().MaxAP++;
    }
}