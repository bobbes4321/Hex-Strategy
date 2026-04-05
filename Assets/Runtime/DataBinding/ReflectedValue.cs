using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime.DataBinding
{
    [Serializable]
    public class ReflectedValue<T> : IReflectedValue
    {
        public TargetType TargetType;

        public Object Target;
        public Component TargetComponent;
        public string MemberName;

        /// <summary>
        /// This caching is about the actual value. If set to true, we get the value once via reflection, and then simply keep track of that value. It will never change again.
        /// Only use this for values that you know won't change, ever.
        /// </summary>
        public bool IsConst = false;

        [NonSerialized] private T _cachedValue;
        [NonSerialized] private bool _isValueCached;

        /// <summary>
        /// These cached fields are for the reflection part: we only need to do the actual reflection to get the field/property, as well as the target type and object once.
        /// This always gets cached for performancer reasons, and is not to be confused with the caching of the value which bypasses one more reflection check (.GetValue());
        /// </summary>
        private PropertyInfo _cachedProperty;

        private FieldInfo _cachedField;
        private object _cachedTarget;
        private Type _cachedType;

        public T GetValue()
        {
            //First we check if we've cached the actual value: if so, just return that
            if (IsConst && _isValueCached)
                return _cachedValue;

            //If we haven't cached the reflected types and objects yet, do that first.
            if (_cachedTarget == null || _cachedType == null)
                CacheAllReflectedValues();

            if (_cachedTarget != null && _cachedType != null)
            {
                if (_cachedField != null)
                    return (T)_cachedField.GetValue(_cachedTarget);

                if (_cachedProperty != null)
                    return (T)_cachedProperty.GetValue(_cachedTarget);
            }

            return default;
        }

        public void SetValue(object value)
        {
            //If no caching has happened yet, cache before continuing
            if (_cachedTarget == null || _cachedType == null)
                CacheAllReflectedValues();

            //If even after caching we don't have a target or type, we can't set the value
            if (_cachedTarget == null || _cachedType == null)
                return;

            if (_cachedField != null)
            {
                _cachedField.SetValue(_cachedTarget, value);
                return;
            }

            if (_cachedProperty != null)
                _cachedProperty.SetValue(_cachedTarget, value);
        }

        object IReflectedValue.GetValue() => GetValue();
        void IReflectedValue.SetValue(object value) => SetValue(value);
        Type IReflectedValue.GetValueType() => typeof(T);

        //public BindDirection Direction { get; set; }

        private void CacheAllReflectedValues()
        {
            object target = null;
            Type type = null;

            //If we couldn't find the target and/or type, just return, we can't cache any of the reflected values
            if (!TryGetTargetAndType(ref target, ref type))
                return;

            if (type == null)
                return;

            //Try to see if the reflected value we want is a field, and if so, cache those values
            FieldInfo field = type.GetField(MemberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(T))
            {
                _cachedField = field;
                _cachedValue = (T)field.GetValue(target);
                _isValueCached = true;
                return;
            }

            //Try to see if the reflected value we want is a property, and if so, cache those values
            PropertyInfo property = type.GetProperty(MemberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null && property.PropertyType == typeof(T) && property.CanRead)
            {
                _cachedProperty = property;
                _cachedValue = (T)property.GetValue(target);
                _isValueCached = true;
            }
        }

        private bool TryGetTargetAndType(ref object target, ref Type type)
        {
            switch (TargetType)
            {
                case TargetType.GameObject:
                    if (TargetComponent == null || string.IsNullOrEmpty(MemberName))
                        return false;

                    target = TargetComponent;
                    _cachedTarget = target;

                    type = TargetComponent.GetType();
                    _cachedType = type;

                    break;

                case TargetType.ScriptableObject:
                    if (string.IsNullOrEmpty(MemberName))
                        return false;

                    target = Target;
                    _cachedTarget = target;
                    type = Target.GetType();
                    _cachedType = type;

                    break;
                case TargetType.Object:
                    target = Target;
                    _cachedTarget = target;
                    type = Target.GetType();
                    _cachedType = type;

                    break;
            }

            return true;
        }
    }
}