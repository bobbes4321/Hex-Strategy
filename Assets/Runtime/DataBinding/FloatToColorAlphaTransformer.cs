using UnityEngine;

namespace Runtime.DataBinding
{
    [CreateAssetMenu(menuName = "Transformers/Create FloatToColorAlphaTransformer", fileName = "FloatToColorAlphaTransformer", order = 0)]
    public class FloatToColorAlphaTransformer : Transformer
    {
        protected override object DoTransform(object inputValue, object outputValue)
        {
            var currentColor = (Color)outputValue;
            return new Color(currentColor.r, currentColor.g, currentColor.b, (float)inputValue);
        }
    }
}