using System;
using UnityEngine;

namespace Runtime.DataBinding
{
    public static class ValueTypeExtensions
    {
        public static Type GetAssociatedType(this ValueType valueType)
        {
            return valueType switch
            {
                ValueType.Float => typeof(float),
                ValueType.String => typeof(string),
                ValueType.Int => typeof(int),
                ValueType.Color => typeof(Color),
                ValueType.Vector3 => typeof(Vector3),
                ValueType.Vector2 => typeof(Vector2),
                _ => throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null)
            };
        }
    }
}