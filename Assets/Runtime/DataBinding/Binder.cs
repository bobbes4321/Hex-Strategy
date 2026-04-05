using System;

namespace Runtime.DataBinding
{
    public interface IReflectedValue
    {
        object GetValue();
        void SetValue(object value);
        Type GetValueType();
    }

    public enum ValueType
    {
        Float,
        String,
        Int,
        Color,
        Vector3,
        Vector2,
    }
}