using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Strategy.Runtime.Piece
{
    [Serializable]
    public class Health
    {
        private float _health;
        public float Current
        {
            get => _health;
            private set
            {
                if (!Mathf.Approximately(_health, value))
                {
                    _health = value;
                    OnHealthChanged?.Invoke(_health);
                }
            }
        }

        [SerializeField]
        private float _maxHealth = 10f;
        public float HealthPercentage => Current / _maxHealth;
        public bool IsDead => _health > 0;
        public event Action OnDied;
        public event Action<float> OnTookDamage;

        public string HealthString => $"{_health}/{_maxHealth}";
        public event Action<float> OnHealthChanged;

        [Button]
        public void DoDamage(float damage = 1)
        {
            Current -= damage;

            if (_health <= 0)
            {
                Current = 0;
                OnDied?.Invoke();
            }
            
            OnTookDamage?.Invoke(damage);

            Debug.Log($"[Health.DoDamage] Took damage, health now {_health}");
        }
        
        public void Heal(float amount) => Current = Mathf.Min(_health + amount, _maxHealth);
        public void Rejuvenate() => Current = _maxHealth;
        public static implicit operator float(Health health) => health._health;
        public void SetMaxHealth(float maxHealth) => _maxHealth = maxHealth;
    }
}