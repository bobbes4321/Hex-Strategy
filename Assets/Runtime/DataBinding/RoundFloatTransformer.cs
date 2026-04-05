using UnityEngine;

namespace Runtime.DataBinding
{
    [CreateAssetMenu(menuName = "Transformers/Create RoundFloatTransformer", fileName = "RoundFloatTransformer", order = 0)]
    public class RoundFloatTransformer : Transformer
    {
        protected override object DoTransform(object inputValue, object outputValue)
        {
            return (float)Mathf.RoundToInt((float)inputValue);
        }
    }
}