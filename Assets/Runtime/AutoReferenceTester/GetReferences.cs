using Teo.AutoReference;
using UnityEngine;

namespace Runtime.AutoReferenceTester
{
    public class GetReferences : MonoBehaviour
    {
        [Get, SerializeField] private MeshRenderer _meshRenderer;

        private void Awake()
        {
            Debug.Log($"[GetReferences.Awake] Here is my meshRenderer: {_meshRenderer}", this);
        }
    }
}
