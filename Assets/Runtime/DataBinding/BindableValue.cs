using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.DataBinding
{
    [Serializable]
    public class BindableValue
    {
        public ValueType ValueType;

        public ReflectedValue<float> FloatValue;
        public ReflectedValue<string> StringValue;
        public ReflectedValue<int> IntValue;
        public ReflectedValue<Color> ColorValue;
        public ReflectedValue<Vector2> Vector2Value;
        public ReflectedValue<Vector3> Vector3Value;
    
        public List<Transformer> Transformers;

        public IReflectedValue GetReflectedValueType()
        {
            return ValueType switch
            {
                ValueType.Float => FloatValue,
                ValueType.String => StringValue,
                ValueType.Int => IntValue,
                ValueType.Color => ColorValue,
                ValueType.Vector2 => Vector2Value,
                ValueType.Vector3 => Vector3Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public object GetValue()
        {
            return GetReflectedValueType().GetValue();
        }
    }
}