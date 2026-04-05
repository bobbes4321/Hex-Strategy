using PrimeTween;
using Reflex.Attributes;
using Runtime.Messaging;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace _Strategy.Runtime.Deck
{
    public class APTextView : MonoBehaviour, IMessageHandler<TriedToUseCardThatWasTooExpensiveMessage>
    {
        [Inject] private AP _ap;
        [Inject] private MessagingManager _messagingManager;
        
        [SerializeField] private ShakeSettings _shakeSettings;

        private TMP_Text _text;
        private Sequence _sequence;
        private void Awake() => _text = GetComponent<TMP_Text>();
        private void OnEnable()
        {
            _messagingManager.RegisterHandler(this);
            _ap.OnCurrentApChanged += HandleApChanged;
        }

        private void OnDisable()
        {
            _messagingManager.UnregisterHandler(this);
            _ap.OnCurrentApChanged -= HandleApChanged;
        }

        [Button]
        private void HandleApChanged()
        {
            _text.SetText($"{_ap.Current}/{_ap.Max}");
            Tween.ShakeLocalPosition(_text.transform, _shakeSettings);
        }

        public void Handle(TriedToUseCardThatWasTooExpensiveMessage message)
        {
            Tween.ShakeLocalPosition(_text.transform, _shakeSettings);

            _sequence.Stop();
            _text.color = Color.white;
            _sequence = Sequence.Create(cycles: 2, CycleMode.Yoyo)
                    .Chain(Tween.Color(_text, Color.red, 0.5f)
                    .Chain(Tween.Color(_text, Color.white, 0.5f)));
        }
    }
}