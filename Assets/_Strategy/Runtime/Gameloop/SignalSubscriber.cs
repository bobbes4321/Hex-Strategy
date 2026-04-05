using System;
using Doozy.Runtime.Signals;

namespace _Strategy.Runtime.Gameloop
{
    public class SignalSubscriber
    {
        private SignalStream _stream;
        private SignalReceiver _receiver;

        private Action<Signal> _onSignalReceived;
        private StreamId.Strategy _streamId;

        public SignalSubscriber(Action<Signal> onSignalReceived, StreamId.Strategy streamId)
        {
            _onSignalReceived = onSignalReceived;
            _streamId = streamId;
        }

        public void Subscribe()
        {
            _receiver = new SignalReceiver().SetOnSignalCallback(ProcessSignal);
            _stream = SignalStream.GetStream(_streamId).ConnectReceiver(_receiver);
        }

        public void Unsubscribe()
        {
            _stream.DisconnectReceiver(_receiver);
        }

        private void ProcessSignal(Signal signal)
        {
            _onSignalReceived?.Invoke(signal);
        }
    }
}