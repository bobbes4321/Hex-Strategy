using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Runtime.CaC_UI
{
    public class VerticalScrollPriority : MonoBehaviour
    {
        public ScrollRect MainVerticalScrollRect;
        public float ScrollSensitivity = 0.1f;
        public float TweenSpeed = 0.1f;
        public Ease EaseType = Ease.OutQuad;
    
        private Tween _activeTween;
    
        private void Update()
        {
            float scrollDelta = Mouse.current.scroll.ReadValue().y;

            if (scrollDelta == 0) 
                return;
        
            float currentPos = MainVerticalScrollRect.verticalNormalizedPosition;
            float targetPos = Mathf.Clamp01(currentPos + scrollDelta * ScrollSensitivity);
        
            _activeTween = MainVerticalScrollRect.DOVerticalNormalizedPos(targetPos, TweenSpeed).SetEase(EaseType);
        }

        private void OnDestroy()
        {
            _activeTween?.Kill();
        }
    }
}