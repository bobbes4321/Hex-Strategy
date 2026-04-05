using System.Collections.Generic;
using System.Linq;
using _Strategy.Runtime.Util;
using Cysharp.Threading.Tasks;
using Doozy.Runtime.Common.Extensions;
using PrimeTween;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace _Strategy.Runtime.Deck
{
    public abstract class CardSelectionBase<TCard, TCardData> : MonoBehaviour where TCard : MonoBehaviour
    {
        [SerializeField] protected List<TCardData> _cards;
        [SerializeField] protected TCard _cardPrefab;
        [SerializeField] protected Transform _parent;
        [SerializeField] protected int _cardsToSpawn = 3;
        [SerializeField] protected string _messageToSendOnFinish;

        protected List<TCard> _spawnedCards = new();

        protected virtual void OnEnable() => ShowCards();
        protected virtual void OnDisable() => HideCards();

        protected virtual void ShowCards()
        {
            var cardDatas = GetCards(_cardsToSpawn);

            foreach (var cardData in cardDatas)
            {
                var spawnedCard = Instantiate(_cardPrefab, _parent);
                InitializeSpawnedCard(spawnedCard, cardData);
                _spawnedCards.Add(spawnedCard);
            }
        }

        public virtual IEnumerable<TCardData> GetCards(int amount) => _cards.GetRandomItems(amount);

        protected abstract void InitializeSpawnedCard(TCard card, TCardData cardData);

        protected virtual void HideCards()
        {
            foreach (var card in _spawnedCards)
            {
                Destroy(card.gameObject);
            }

            _spawnedCards.Clear();
        }

        public virtual void FinishCardSelection() => StrategySignal.Send(_messageToSendOnFinish);
    }

    public class Deck : CardSelectionBase<DeckCard, CardData>
    {
        [SerializeField] private TMP_Text _amountOfSelectedCardsText;
        [SerializeField] private ShakeSettings _textScaleSettings;

        [Inject] private Hand _hand;
        [Inject] private RaritySettings _raritySettings;

        private List<DeckCard> _selectedCards = new();

        private void Awake()
        {
#if UNITY_EDITOR
            UpdateCardDataList();
#endif
        }

#if UNITY_EDITOR
        [Button]
        private void UpdateCardDataList()
        {
            string[] guids = AssetDatabase.FindAssets("t:CardData");
            List<CardData> allCardData = new List<CardData>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                CardData cardData = AssetDatabase.LoadAssetAtPath<CardData>(path);
                if (cardData != null)
                {
                    allCardData.Add(cardData);
                }
            }

            bool hasChanges = false;
            foreach (CardData cardData in allCardData)
            {
                if (!_cards.Contains(cardData))
                {
                    Debug.Log($"[Deck.UpdateCardDataList] Added {cardData.Name}", this);
                    _cards.Add(cardData);
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
                if (prefabRoot != null)
                {
                    EditorUtility.SetDirty(prefabRoot);
                    string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabRoot);
                    if (!string.IsNullOrEmpty(prefabPath))
                    {
                        PrefabUtility.SaveAsPrefabAssetAndConnect(prefabRoot, prefabPath, InteractionMode.AutomatedAction);
                    }
                }
            }
        }
#endif

        public override IEnumerable<CardData> GetCards(int amount)
        {
            if (_raritySettings == null)
            {
                Debug.LogWarning("RaritySettings not assigned, using random selection");
                return _cards.GetRandomItems(amount);
            }

            var selectedCards = new List<CardData>();

            for (int i = 0; i < amount; i++)
            {
                var rarity = _raritySettings.GetRandomRarity();
                var card = GetCardByRarity(rarity);

                if (card != null)
                {
                    selectedCards.Add(card);
                }
                else
                {
                    Debug.LogWarning("No cards available for any rarity. Check your card collection.");
                }
            }

            return selectedCards;
        }

        protected virtual CardData GetCardByRarity(Rarity targetRarity)
        {
            // Try to get cards of the target rarity first
            var cardsOfRarity = GetCardsByRarity(targetRarity).ToList();
            if (cardsOfRarity.Any())
            {
                return cardsOfRarity.GetRandomItem();
            }

            // If no cards found for target rarity, try lower rarities
            var allRarities = System.Enum.GetValues(typeof(Rarity)).Cast<Rarity>().OrderBy(r => (int)r).ToList();
            int targetIndex = allRarities.IndexOf(targetRarity);

            // Go down the rarity list (higher to lower rarity)
            for (int i = targetIndex - 1; i >= 0; i--)
            {
                cardsOfRarity = GetCardsByRarity(allRarities[i]);
                if (cardsOfRarity.Any())
                {
                    Debug.Log($"No cards found for rarity {targetRarity}, falling back to {allRarities[i]}");
                    return cardsOfRarity.GetRandomItem();
                }
            }

            // If still no cards found, try higher rarities
            for (int i = targetIndex + 1; i < allRarities.Count; i++)
            {
                cardsOfRarity = GetCardsByRarity(allRarities[i]);
                if (cardsOfRarity.Any())
                {
                    Debug.Log($"No cards found for rarity {targetRarity}, falling back to {allRarities[i]}");
                    return cardsOfRarity.GetRandomItems(1).FirstOrDefault();
                }
            }

            // Final fallback - return any random card if no rarity-specific cards are found
            if (_cards.Any())
            {
                Debug.LogWarning($"No cards found for any specific rarity, returning random card");
                return _cards.GetRandomItems(1).FirstOrDefault();
            }

            return null;
        }

        private List<CardData> GetCardsByRarity(Rarity targetRarity)
        {
            return _cards.Where(card => card.Rarity == targetRarity).ToList();
        }

        protected override void ShowCards()
        {
            _selectedCards.Clear();

            if (!CanSelectCard())
            {
                WaitAndFinishCardSelection().Forget();
                return;
            }

            base.ShowCards();
            UpdateSelectedCardsText();
        }

        private async UniTask WaitAndFinishCardSelection()
        {
            await UniTask.DelayFrame(1);
            FinishCardSelection();
        }

        protected override void HideCards()
        {
            base.HideCards();
            _selectedCards.Clear();
        }

        protected override void InitializeSpawnedCard(DeckCard card, CardData cardData) => card.Initialize(cardData);

        public void SelectCard(DeckCard deckCard)
        {
            _selectedCards.Add(deckCard);
            UpdateSelectedCardsText();
        }

        public void UnSelectCard(DeckCard deckCard)
        {
            _selectedCards.Remove(deckCard);
            UpdateSelectedCardsText();
        }

        private void UpdateSelectedCardsText()
        {
            Tween.ShakeScale(_amountOfSelectedCardsText.transform, _textScaleSettings);
            _amountOfSelectedCardsText.SetText($"{_selectedCards.Count + _hand.CardsInHand}/{_hand.MaxCardsInHand}");
        }

        public bool CanSelectCard() => _selectedCards.Count + _hand.CardsInHand < _hand.MaxCardsInHand;

        public override void FinishCardSelection()
        {
            foreach (var selectedCard in _selectedCards)
            {
                _hand.Add(selectedCard.CardData);
            }

            base.FinishCardSelection();
        }
    }
}