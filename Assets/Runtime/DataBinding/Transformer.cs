using UnityEngine;

namespace Runtime.DataBinding
{
    public abstract class Transformer : ScriptableObject
    {
        public ValueType InputValueType;
        public ValueType OutputValueType;

        public object Transform(object inputValue, object outputValue)
        {
            if (!IsCompatible(inputValue, outputValue))
                return null;
        
            return DoTransform(inputValue, outputValue);
        }

        protected abstract object DoTransform(object inputValue, object outputValue);

        public bool IsCompatible(object inputValue, object outputValue)
        {
            if (inputValue.GetType() != InputValueType.GetAssociatedType())
                return false;

            if (outputValue != null && outputValue.GetType() != OutputValueType.GetAssociatedType())
                return false;

            return true;
        }
    }
}