using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Messaging
{
    public class MessagingManager : MonoBehaviour
    {
        private List<IMessageHandler> _messageHandlers = new();
        private void Awake()
        {
            DontDestroyOnLoad(this);
            ServiceLocator.Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<MessagingManager>();
        }

        public void Publish(IMessage message)
        {
            foreach (var messageHandler in _messageHandlers)
            {
                // Check if the handler implements IMessageHandler<T> for the specific message type
                var handlerType = messageHandler.GetType()
                    .GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>)
                                                         && i.GetGenericArguments()[0] == message.GetType());

                if (handlerType != null)
                {
                    // Invoke the Handle method on the handler
                    var method = handlerType.GetMethod("Handle");
                    method?.Invoke(messageHandler, new object[] { message });
                }
            }
        }

        public void RegisterHandler(IMessageHandler messageHandler)
        {
            _messageHandlers.Add(messageHandler);
        }
        
        public void UnregisterHandler(IMessageHandler messageHandler)
        {
            _messageHandlers.Remove(messageHandler);
        }
    }
}