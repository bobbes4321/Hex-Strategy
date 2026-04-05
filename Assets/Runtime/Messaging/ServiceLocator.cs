using System;
using System.Collections.Generic;
using Doozy.Runtime.Common.Attributes;

namespace Runtime.Messaging
{
    public static class ServiceLocator
    {
        [ClearOnReload(true)]
        private static Dictionary<Type, object> _instances = new();

        public static T Get<T>() where T : class
        {
            if (_instances.ContainsKey(typeof(T)))
                return _instances[typeof(T)] as T;

            return null;
        }

        public static bool TryGet<T>(out T obj) where T : class
        {
            obj = Get<T>();
            return obj == null;
        }

        public static void Register<T>(T instance) where T : class
        {
            _instances.TryAdd(instance.GetType(), instance);
        }

        public static void Unregister<T>() where T : class
        {
            if (_instances.ContainsKey(typeof(T)))
                _instances.Remove(typeof(T));
        }
    }
}