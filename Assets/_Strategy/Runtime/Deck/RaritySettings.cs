using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Strategy.Runtime.Deck
{
    [CreateAssetMenu(menuName = "Strategy/Create RarityColors", fileName = "RarityColors", order = 0)]
    public class RaritySettings : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<Rarity, Color> _rarityColors;
        [SerializeField] private Dictionary<Rarity, float> _rarityProbabilities;

        public Color GetColor(Rarity rarity)
        {
            return _rarityColors[rarity];
        }
        
        public Rarity GetRandomRarity()
        {
            if (_rarityProbabilities == null || _rarityProbabilities.Count == 0)
            {
                Debug.LogWarning("Rarity probabilities dictionary is empty or null!");
                return default(Rarity);
            }

            float totalWeight = _rarityProbabilities.Values.Sum();
            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;

            foreach (var kvp in _rarityProbabilities)
            {
                currentWeight += kvp.Value;
                if (randomValue <= currentWeight)
                {
                    return kvp.Key;
                }
            }

            // Fallback - return the last rarity in case of floating point precision issues
            return _rarityProbabilities.Keys.Last();
        }

    }
}