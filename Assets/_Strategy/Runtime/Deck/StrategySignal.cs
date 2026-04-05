using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager;

namespace _Strategy.Runtime.Deck
{
    public static class StrategySignal
    {
        public static void Send(string id)
        {
            SignalPayload payload = new()
            {
                streamId = new(nameof(UIButtonId.Strategy), id),
            };

            payload.SendSignal();
        }
    }
}