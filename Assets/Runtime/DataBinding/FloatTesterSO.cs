using UnityEngine;
#pragma warning disable CS0414 // Field is assigned but its value is never used

namespace Runtime.DataBinding
{
    [CreateAssetMenu(menuName = "Create FloatTesterSO", fileName = "FloatTesterSO", order = 0)]
    public class FloatTesterSO : ScriptableObject
    {
        public float PublicFloat;
        private float _privateFloat = 0.1f;

        public float PublicFloatProperty;
        private float _privateFloatProperty = 8.4f;
    }
}