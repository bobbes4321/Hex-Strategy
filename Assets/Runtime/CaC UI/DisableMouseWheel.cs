using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Runtime.CaC_UI
{
    public class DisableMouseWheel : ScrollRect
    {
        public override void OnScroll(PointerEventData data)
        {
            // Block scroll input from affecting horizontal ScrollRects
            // (Let the VerticalScrollPriority script handle it globally)
        }
    }
}