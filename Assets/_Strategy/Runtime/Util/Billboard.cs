using System;
using UnityEngine;

namespace _Strategy.Runtime.Util
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField] private bool _keepScaleRegardlessOfCameraDistance = true;
        
        private Camera _camera;
        private float _baseScale = 1f;
        private float _referenceDistance;

        private void Awake()
        {
            _baseScale = transform.localScale.x;
            _camera = Camera.main;
            _referenceDistance = Vector3.Distance(_camera.transform.position, transform.position);
        }

        private void Update()
        {
            var direction = _camera.transform.position - transform.position;
            
            // Calculate only the X rotation (tilt) based on the camera's elevation
            float distance = new Vector2(direction.x, direction.z).magnitude;
            float angle = Mathf.Atan2(direction.y, distance) * Mathf.Rad2Deg;
            
            // Apply only X rotation, keep Y and Z rotation at 0
            transform.rotation = Quaternion.Euler(angle, 0, 0);
            
            if (!_keepScaleRegardlessOfCameraDistance) return;
            
            // Scale based on distance to maintain constant visual size
            float totalDistance = direction.magnitude;
            float scale = (totalDistance / _referenceDistance) * _baseScale;
            transform.localScale = Vector3.one * scale;
        }
    }
}