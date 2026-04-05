using Runtime.Attributes;
using Runtime.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.UI
{
    public class BetterButton : MonoBehaviour, IPointerClickHandler
    {
        [IdentifierDropdown] public Identifier GoTo;

        public void OnPointerClick(PointerEventData eventData)
        {
            var messagingManager = ServiceLocator.Get<MessagingManager>();
            messagingManager.Publish(new ButtonClickedMessage(GoTo));
        
            Debug.Log($"[UIButton.OnPointerClick] {GoTo}", this);
        }
    }

    public struct ButtonClickedMessage : IMessage
    {
        public Identifier Identifier;

        public ButtonClickedMessage(Identifier identifier)
        {
            Identifier = identifier;
        }
    }
}