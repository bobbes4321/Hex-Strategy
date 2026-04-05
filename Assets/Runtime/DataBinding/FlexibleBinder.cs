using System.Collections.Generic;
using UnityEngine;

namespace Runtime.DataBinding
{
    public class FlexibleBinder : MonoBehaviour
    {
        [Header("Input")] public BindableValue Input;
        public List<BindableValue> Outputs = new();

        private object _cachedInput;

        private void Update()
        {
            if (Input == null) return;

            object currentValue = GetInputValue();

            if (_cachedInput == null || !currentValue.Equals(_cachedInput))
            {
                foreach (var output in Outputs)
                {
                    object valueForOutput = ApplyOutputTransformers(currentValue, output);

                    var outputValueType = output.GetReflectedValueType();
                    if (outputValueType.GetValueType() == valueForOutput.GetType())
                    {
                        outputValueType.SetValue(valueForOutput);
                    }
                    else if (outputValueType.GetValueType() == typeof(string))
                    {
                        outputValueType.SetValue(valueForOutput.ToString());
                    } 
                    else 
                        Debug.LogWarning($"No binding happened because the types did not match and the output was not to string. Types: {outputValueType.GetValueType().Name} | {valueForOutput.GetType().Name}");
                }

                _cachedInput = currentValue;
            }
        }

        private static object ApplyOutputTransformers(object currentValue, BindableValue output)
        {
            var valueForOutput = currentValue;
            foreach (var outputTransformer in output.Transformers)
            {
                var outputValue = output.GetValue();
                if (!outputTransformer.IsCompatible(valueForOutput, outputValue))
                {
                    Debug.LogWarning($"Not transforming using {outputTransformer.name} because the types are not compatible." +
                                     $"\nThe types are: Input: {valueForOutput.GetType().Name} | Output: {outputValue.GetType().Name}, " +
                                     $"\nExpected types were: Input: {outputTransformer.InputValueType.GetAssociatedType().Name} | Output: {outputTransformer.OutputValueType.GetAssociatedType().Name}");
                    continue;
                }
                        
                valueForOutput = outputTransformer.Transform(valueForOutput, outputValue);
            }

            return valueForOutput;
        }

        private object GetInputValue()
        {
            var currentValue = Input.GetValue();
            foreach (var inputTransformer in Input.Transformers)
            {
                if (!inputTransformer.IsCompatible(currentValue, null))
                    continue;
                
                currentValue = inputTransformer.Transform(currentValue, null);
            }

            return currentValue;
        }
    }
}