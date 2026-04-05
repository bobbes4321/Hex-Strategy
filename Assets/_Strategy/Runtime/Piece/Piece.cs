using System;
using _Strategy.Runtime.Board;
using Cysharp.Threading.Tasks;
using Doozy.Runtime.UIManager.Containers;
using PrimeTween;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using Teo.AutoReference;
using UnityEngine;

namespace _Strategy.Runtime.Piece
{
    [SelectionBase]
    public class Piece : MonoBehaviour, IPiece
    {
        [SerializeField] private ShakeSettings _punchScaleSettings;
        [SerializeField] private TweenSettings _attackToMoveSettings;
        [SerializeField] private float _attackStayDuration = 0.5f;
        [SerializeField] private TweenSettings _attackFromMoveSettings;
        [SerializeField] private TweenSettings _deathScaleSettings;
        [SerializeField, GetInChildren] private UIContainer _details;

        [SerializeField] private Team _team;
        [ShowInInspector] public Hex Hex => Board.Board.WorldToHex(transform.position);
        public string Id { get; set; }

        [SerializeField] private Health _health;

        public Health Health
        {
            get => _health;
            set => _health = value;
        }

        public float HealthAmount => Health.HealthPercentage;
        public string HealthText => Health.HealthString;

        [Inject] private Board.Board _board;

        private MeshRenderer _meshRenderer;
        private Color _defaultColor;

        public event Action<Piece> OnDied;

        public Team Team
        {
            get => _team;
            set => _team = value;
        }

        protected virtual void Awake()
        {
            Health.Rejuvenate();
        }

        protected virtual void Start()
        {
            Id = name.Replace("(Clone)", "");
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            _defaultColor = _meshRenderer.material.color;
            _board.AddPiece(this, Hex);
            Health.OnDied += Die;
            Health.OnTookDamage += TakeDamage;
        }

        private void OnDestroy()
        {
            Health.OnDied -= Die;
            Health.OnTookDamage -= TakeDamage;
        }

        public void MoveTo(Hex hex)
        {
            var newPosition = Board.Board.HexToWorld(hex);
            Tween.PositionAtSpeed(transform, newPosition, 5f);
        }

        public void Attack(Hex hexToAttack)
        {
            DoAttackTween(hexToAttack).Forget();
        }

        private async UniTask DoAttackTween(Hex hexToAttack)
        {
            var currentPosition = transform.position;
            var toPosition = Board.Board.HexToWorld(hexToAttack);
            var direction = (toPosition - currentPosition).normalized;
            var endPosition = toPosition - (direction * (Board.Board.HexSize * 0.5f));
            await Tween.LocalPosition(transform, endPosition, _attackToMoveSettings);
            await UniTask.WaitForSeconds(_attackStayDuration);
            await Tween.LocalPosition(transform, currentPosition, _attackFromMoveSettings);
        }

        private void TakeDamage(float damage)
        {
            Tween.PunchScale(transform, _punchScaleSettings);
            Sequence.Create()
                .Chain(Tween.Custom(_meshRenderer.material.color, Color.red, duration: 0.2f, onValueChange: newVal => _meshRenderer.material.color = newVal))
                .Chain(Tween.Custom(Color.red, _defaultColor, duration: 0.2f, onValueChange: newVal => _meshRenderer.material.color = newVal));
        }

        public void Die()
        {
            OnDied?.Invoke(this);
            DoDieTween().Forget();
        }

        private async UniTask DoDieTween()
        {
            _board.RemoveAt(Hex);
            await Tween.Scale(transform, Vector3.zero, _deathScaleSettings);
            gameObject.SetActive(false);
        }
        
        public void ShowDetails()
        {
            _details.Show();
        }

        public void HideDetails()
        {
            _details.Hide();
        }

        public override string ToString() => $"(Type: {GetType().Name} | Hex: {Hex} | Team: {Team})";
    }
}