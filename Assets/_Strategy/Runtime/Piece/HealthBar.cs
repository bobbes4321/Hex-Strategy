using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

namespace _Strategy.Runtime.Piece
{
    public class HealthBar : MonoBehaviour
    {
        private Health _health;
        private Image _fill;
        
        private void Awake()
        {
            _fill = GetComponent<Image>();
            _health = GetComponentInParent<Piece>().Health;
            _health.OnHealthChanged += HandleHealthChanged;
            _health.OnTookDamage += HandleTookDamage;
        }

        private void HandleTookDamage(float damageAmount)
        {
            Tween.ShakeCamera(Camera.main, damageAmount * 0.2f);
        }

        private void OnDestroy()
        {
            _health.OnHealthChanged -= HandleHealthChanged;
            _health.OnTookDamage -= HandleTookDamage;
        }

        private void HandleHealthChanged(float _)
        {
           Tween.UIFillAmount(_fill, _health.HealthPercentage, 0.5f);
           Tween.PunchLocalPosition(_fill.transform, Vector3.one, 0.5f);
        }
    }
}