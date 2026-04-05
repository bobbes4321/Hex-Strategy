using Reflex.Core;
using UnityEngine;

public class MonoBehaviourFactory<T> where T : MonoBehaviour
{
    public static T CreateAndRegister(ContainerBuilder builder, bool addToDontDestroyOnLoad = true)
    {
        var instance = new GameObject(typeof(T).Name).AddComponent<T>();
        builder.AddSingleton(instance);
        
        return instance;
    }
}