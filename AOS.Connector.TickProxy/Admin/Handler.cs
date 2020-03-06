using AOS.Connector.TickProxy.DTO;
using MessagePack;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AOS.Connector.TickProxy.Admin
{
    internal class Handler : IDisposable
    {
        private SubscriberSocket _clientSocket;
        private readonly string _binding;
        private readonly Client _listener;
        private CancellationToken _cancelToken;
        private NetMQPoller _poller;

        internal Handler(string binding, Client listener)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");

            _listener = listener;
            _binding = binding;
        }

        private void ConnectSocket()
        {
            if (string.IsNullOrEmpty(_binding))
                return;

            _clientSocket = new SubscriberSocket();
            _clientSocket.Connect(_binding);
        }

        public void Disconnect()
        {
            if (_clientSocket != null)
            {
                if (_poller.IsRunning) _poller.Stop();
                _clientSocket.Options.Linger = TimeSpan.Zero;
                _clientSocket.Disconnect(_binding);
                _clientSocket.Close();
                _clientSocket.Dispose();
                _clientSocket = null;
            }
        }

        internal void Subscribe(CancellationToken token)
        {
            if (_clientSocket == null)
            {
                _cancelToken = token;
                ConnectSocket();
                _clientSocket.ReceiveReady += ClientSocket_ReceiveReady;
            }

            _clientSocket.Subscribe("TP_HeartBeat");//subscribe to heartbeats only

            if (_poller == null || !_poller.IsRunning)
            {
                _poller = new NetMQPoller();
                _poller.Add(_clientSocket);
                _poller.RunAsync();
            }
        }

        private void ClientSocket_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            if (_cancelToken.IsCancellationRequested)
                return;

            NetMQMessage message = _clientSocket.ReceiveMultipartMessage(3);

            //frame 1: subscribed symbol
            //frame 2: message type
            //frame 3: payload
            if (message.FrameCount != 3)
                return;

            switch (message[1].ConvertToInt32())
            {
                case 3:
                    AdminMessage expSymbols = MessagePackSerializer.Deserialize<AdminMessage>(message[2].Buffer);
                    ExpiredSymbols(expSymbols.ExpiredSymbols);
                    break;
                case 4:
                    AdminMessage heartBeat = MessagePackSerializer.Deserialize<AdminMessage>(message[2].Buffer);
                    HeartBeat(heartBeat);
                    break;
                default:
                    break;
            }
        }

        internal void ExpiredSymbols(List<string> symbols)
        {
            _listener.ExpiredSymbols(symbols);
        }
        internal void HeartBeat(AdminMessage beat)
        {
            _listener.HeartBeat(beat);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disconnect();

                    if (_poller != null)
                        _poller.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
