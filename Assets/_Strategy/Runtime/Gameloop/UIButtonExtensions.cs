using Cysharp.Threading.Tasks;
using Doozy.Runtime.UIManager.Components;

namespace _Strategy.Runtime.Gameloop
{
    public static class UIButtonExtensions
    {
        public static UniTask OnClickAsync(this UIButton button)
        {
            return new AsyncUnityEventHandler(button.onClickEvent, button.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }
    }
}