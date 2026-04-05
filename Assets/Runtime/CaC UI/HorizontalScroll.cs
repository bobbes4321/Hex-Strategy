using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Runtime.CaC_UI
{
    public class HorizontalScroll : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public ScrollRect ParentScrollRect; // Assign the main vertical ScrollRect

        private ScrollRect _myScrollRect;
        private bool _isHorizontal;

        void Start()
        {
            _myScrollRect = GetComponent<ScrollRect>();
            _isHorizontal = Mathf.Abs(_myScrollRect.velocity.x) > Mathf.Abs(_myScrollRect.velocity.y);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            ParentScrollRect.OnBeginDrag(eventData);
            _isHorizontal = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);
            ParentScrollRect.enabled = !_isHorizontal;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isHorizontal)
                _myScrollRect.OnDrag(eventData);
            else
                ParentScrollRect.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ParentScrollRect.OnEndDrag(eventData);
            ParentScrollRect.enabled = true;
        }
    }
}