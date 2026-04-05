using Runtime.Messaging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime
{
    public class ServiceLocatorRetriever : MonoBehaviour, IMessageHandler<ServiceLocatorMessage>
    {
        private void Start()
        {
            var messagingManager = ServiceLocator.Get<MessagingManager>();
            messagingManager.RegisterHandler(this);
        }

        [Button]
        private void RetrieveServiceLocatorTests()
        {
            var serviceLocatorTests = ServiceLocator.Get<ServiceLocatorTests>();
            Debug.Log($"[ServiceLocatorTests.RetrieveServiceLocatorTests] {serviceLocatorTests}", this);
        }

        public void Handle(ServiceLocatorMessage message)
        {
            Debug.Log($"[ServiceLocatorRetriever.Handle] {message.Message}", this);
        }
    }
}