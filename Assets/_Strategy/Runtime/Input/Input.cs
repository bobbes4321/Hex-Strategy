using UnityEngine;
using UnityEngine.InputSystem;

namespace _Strategy.Runtime.Input
{
    public class Input : MonoBehaviour
    {
        private ISelectable _lastSelected = new NullSelected();
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (!Mouse.current.leftButton.wasPressedThisFrame) return;
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit)) return;
            if (!hit.transform.TryGetComponent(out ISelectable selectable))
            {
                _lastSelected.Toggle();
                _lastSelected = new NullSelected();
                return;
            }

            if (selectable != _lastSelected)
            {
                _lastSelected.Toggle();
                selectable.Toggle();
                _lastSelected = selectable;
            }
            else
            {
                selectable.Toggle();
                _lastSelected = new NullSelected();
            }
        }
    }
}