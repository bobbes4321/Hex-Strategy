using System;
using System.Collections.Generic;
using Runtime.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class UIPanel : MonoBehaviour
    {
        [IdentifierDropdown]
        public Identifier Identifier;

        [SerializeField] private List<Connection> _connections;
    
        private CanvasGroup _canvasGroup;
        private Canvas _canvas;

        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvas = GetComponent<Canvas>();

            UIPanelDatabase.Add(this);
            Hide();
        }

        private void OnDestroy()
        {
            UIPanelDatabase.Remove(this);
        }

        public List<Connection> GetConnections()
        {
            return _connections;
        }
    }

    [Serializable]
    public class Identifier
    {
        public string Category;
        public string Value;
        public override bool Equals(object obj)
        {
            if (obj is not Identifier identifier)
                return false;

            if (identifier.Category != Category)
                return false;

            if (identifier.Value != Value)
                return false;

            return true;
        }
    
        public static bool operator ==(Identifier a, Identifier b)
        {
            // Handle null cases explicitly
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;

            return a.Equals(b);
        }

        public static bool operator !=(Identifier a, Identifier b)
        {
            if (a == null)
                return b == null;
        
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Category, Value);
        }
        public override string ToString()
        {
            return $"{Category} ({Value})";
        }
    }
}