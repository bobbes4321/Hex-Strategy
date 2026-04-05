using Runtime.Messaging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime
{
    public class ServiceLocatorTests : MonoBehaviour
    {
        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        [Button]
        private void Send(string message = "Hello, World!")
        {
            ServiceLocator.Get<MessagingManager>().Publish(new ServiceLocatorMessage(message));
        }
    }

    public class ServiceLocatorMessage : IMessage
    {
        public ServiceLocatorMessage(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}