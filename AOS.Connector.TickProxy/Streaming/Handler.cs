using AOS.Connector.TickProxy.DTO;
using MessagePack;
using MessagePack.Resolvers;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AOS.Connector.TickProxy.Streaming
{
    internal class Handler : IDisposable
    {
        private SubscriberSocket _clientSocket;
        private readonly string _binding;
        private readonly Client _listener;
        private CancellationToken _cancelToken;
        private NetMQPoller _poller;
        private readonly string _subscriptionSuffix = "|";
        private readonly string _subscriptionBarSuffix = "+";
        private volatile bool _isTerminated = false;

        internal Handler(string binding, Client listener)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");

            _listener = listener;
            _binding = binding;

            CompositeResolver.RegisterAndSetAsDefault(
                // Resolve DateTime first
                MessagePack.Resolvers.NativeDateTimeResolver.Instance,
                MessagePack.Resolvers.StandardResolver.Instance
            );
        }

        private void ConnectSocket()
        {
            if (string.IsNullOrEmpty(_binding) || _isTerminated)
                return;

            _clientSocket = new SubscriberSocket();
            _clientSocket.Connect(_binding);
        }

        public void Disconnect()
        {
            if (_clientSocket != null)
            {
                if (_poller != null && _poller.IsRunning) _poller.Stop();
                _clientSocket.Options.Linger = TimeSpan.Zero;
                _clientSocket.Disconnect(_binding);
                _clientSocket.Close();
                _clientSocket.Dispose();
                _clientSocket = null;
            }
        }

        private void ClientSocket_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            if (_cancelToken.IsCancellationRequested)
                return;

            NetMQMessage message = _clientSocket.ReceiveMultipartMessage(3);

            //frame 1: subscribed symbol
            //frame 2: 1 = quote
            //frame 3: payload
            if (message.FrameCount != 3)
                return;

            switch (message[1].ConvertToInt32())
            {
                case 11:
                    //Console.WriteLine("11");
                    LiteQuote quote = MessagePackSerializer.Deserialize<LiteQuote>(message.Last.Buffer);
                    StreamingQuote(quote);
                    break;
            }
        }

        /// <summary>
        /// Subscribe to quotes
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        internal void Subscribe(List<string> request, CancellationToken ct)
        {
            if (_isTerminated)
                return;

            if (_clientSocket == null)
            {
                _cancelToken = ct;
                ConnectSocket();
                _clientSocket.ReceiveReady += ClientSocket_ReceiveReady;
            }

            if (request != null && request.Any())
            {
                foreach (string item in request)
                    _clientSocket.Subscribe(item + _subscriptionSuffix);
            }
            else
                _clientSocket.Subscribe("");//else subscribe to all topics (i.e. Symbols)

            if (_poller == null || !_poller.IsRunning)
            {
                _poller = new NetMQPoller();
                _poller.Add(_clientSocket);
                _poller.RunAsync();
            }
        }

        internal void UnSubscribe(List<string> request)
        {
            if (_clientSocket == null)
                return;

            if (request == null)
            {
                _clientSocket.Unsubscribe("");
                return;
            }

            foreach (string item in request)
                _clientSocket.Unsubscribe(item + _subscriptionSuffix);
        }

        internal void UnsubscribeFromBar(string symbol)
        {
            if (_clientSocket == null)
                return;

            _clientSocket.Unsubscribe(symbol + _subscriptionBarSuffix);
        }

        internal void StreamingQuote(LiteQuote quote)
        {
            _listener.StreamingQuote(quote);
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

                    _listener.Dispose();
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
