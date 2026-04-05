using UnityEngine;

namespace Runtime.DataBinding
{
    [CreateAssetMenu(menuName = "Transformers/Create FormatFloatTransformer", fileName = "FormatFloatTransformer", order = 0)]
    public class FormatFloatTransformer : Transformer
    {
        [SerializeField] private string _formatting;
    
        protected override object DoTransform(object inputValue, object outputValue)
        {
            return ((float)inputValue).ToString(_formatting);
        }
    }
}