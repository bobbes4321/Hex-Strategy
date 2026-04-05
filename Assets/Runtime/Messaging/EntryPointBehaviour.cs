using UnityEngine;

namespace Runtime.Messaging
{
    public class EntryPointBehaviour
    {
       // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
       // private static void Initialize()
       // {
       // //    new GameObject("MessagingManager").AddComponent<MessagingManager>(); 
       // }
    }

    public interface IMessage {}

    public interface IMessageHandler
    {
    }

    public interface IMessageHandler<in T> : IMessageHandler
    {
        public void Handle(T message);
    }
}