using System.Collections.Generic;

namespace _Strategy.Runtime.Deck
{
    public class UpgradeSelection : CardSelectionBase<UpgradeCard, UpgradeCardData>
    {
        protected override void InitializeSpawnedCard(UpgradeCard card, UpgradeCardData cardData)
        {
            card.Initialize(cardData);
        }
    }
}