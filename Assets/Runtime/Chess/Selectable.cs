using System;
using UnityEngine;

namespace Runtime.Chess
{
    public class Selectable : MonoBehaviour, ISelectable
    {
        private bool _isSelected = false;

        public event Action OnSelected;
        public event Action OnDeselected;

        public void Toggle()
        {
            _isSelected = !_isSelected;

            if (_isSelected) OnSelected?.Invoke();
            else OnDeselected?.Invoke();
        }
    }
}