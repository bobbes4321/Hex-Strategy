using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Runtime.Attributes;
using Runtime.Messaging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.UI
{
    public class FlowManager : MonoBehaviour, IMessageHandler<ButtonClickedMessage>
    {
        [SerializeField] private UIPanel _currentPanel;
        [SerializeField] private Flow _flow;

        private void Awake()
        {
            var messageManager = ServiceLocator.Get<MessagingManager>();
            messageManager.RegisterHandler(this);

            _flow = new(_currentPanel);
        }

        private void Start()
        {
            if (_currentPanel != null)
                _currentPanel.Show();
        }

        [Button("Receive signal")]
        private void HandleButtonClick(Identifier goTo)
        {
            _flow.ReceiveSignal(goTo);
        }

        public void Handle(ButtonClickedMessage message)
        {
            _flow.ReceiveSignal(message.Identifier);
        }
    }

    [Serializable]
    public class Flow
    {
        private UIPanel _currentPanel;

        public Flow(UIPanel currentPanel)
        {
            _currentPanel = currentPanel;
        }

        public void ReceiveSignal(Identifier receivedSignal)
        {
            var connections = _currentPanel.GetConnections();

            var result = connections.FirstOrDefault(x => x.From == receivedSignal);

            if (result != null)
            {
                var panel = UIPanelDatabase.Get(result.To);

                if (panel)
                    GoTo(panel);
            }
        }

        private void GoTo(UIPanel panel)
        {
            if (_currentPanel)
                _currentPanel.Hide();

            _currentPanel = panel;
            _currentPanel.Show();
        }
    }

    [Serializable]
    public class Connection
    {
        [IdentifierDropdown] public Identifier From;
        [IdentifierDropdown] public Identifier To;
    }

    public static class UIPanelDatabase
    {
        [ClearOnReload(newInstance:true)]
        private static List<UIPanel> _uiPanels = new List<UIPanel>();

        public static List<UIPanel> UIPanels => _uiPanels;

        public static void Add(UIPanel uiPanel)
        {
            if (!_uiPanels.Contains(uiPanel))
                _uiPanels.Add(uiPanel);
        }

        public static void Remove(UIPanel uiPanel)
        {
            if (_uiPanels.Contains(uiPanel))
                _uiPanels.Remove(uiPanel);
        }

        public static UIPanel Get(Identifier id)
        {
            return _uiPanels.FirstOrDefault(x => x.Identifier == id);
        }
    }
}