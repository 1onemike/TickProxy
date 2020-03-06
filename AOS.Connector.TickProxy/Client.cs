using AOS.Connector.TickProxy.DTO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AOS.Connector.TickProxy
{
    public delegate void OnStreamingQuoteDelegate(LiteQuote quote);
    //public delegate void OnStreamingBarDelegate(Bar bar);
    public delegate void OnExpiredSymbolsDelegate(List<string> expiredSymbols);
    public delegate void OnHeartBeatDelegate(AdminMessage beat);

    public delegate void OnStreamClientReconnectDelegate(Guid clientId);
    public delegate void OnStreamClientMissedHeartBeatDelegate(Guid clientId);

    public class Client : IDisposable
    {
        private readonly RPC.Handler _rpcClient;
        private readonly RPC.Handler _histClient;
        private readonly Streaming.Handler _streamClient;
        private readonly Admin.Handler _adminClient;

        private OnStreamingQuoteDelegate _onStreamingQuote;
        //private OnStreamingBarDelegate _onStreamingBar;
        private OnExpiredSymbolsDelegate _onExpiredSymbols;
        private OnHeartBeatDelegate _onHeartBeat;
        private OnStreamClientReconnectDelegate _onClientReconnect;
        private OnStreamClientMissedHeartBeatDelegate _onMissedHeartBeat;

        private CancellationTokenSource _streamCancelToken = new CancellationTokenSource();
        private readonly CancellationTokenSource _shutDownToken = new CancellationTokenSource();

        private readonly System.Timers.Timer _hbTimer;

        public Guid ClientId { get; } = Guid.NewGuid();

        public Client(string adminEndpoint, string rpcEndpoint, string streamEndpoint, string histEndpoint)
        {
            int requestRetries = 3;
            TimeSpan requestTimeout = new TimeSpan(0, 0, 15);

            if (!string.IsNullOrEmpty(adminEndpoint))
            {
                _adminClient = new Admin.Handler(adminEndpoint, this);

                Task.Factory.StartNew(() =>
                {
                    _adminClient.Subscribe(_shutDownToken.Token);
                }, _shutDownToken.Token);

                _hbTimer = new System.Timers.Timer(1500);
                _hbTimer.Elapsed += DisconnectCheckTimerEvent;
                _hbTimer.Start();
            }

            if (!string.IsNullOrEmpty(rpcEndpoint))
            {
                _rpcClient = new RPC.Handler(rpcEndpoint, requestRetries, requestTimeout);
            }

            if (!string.IsNullOrEmpty(histEndpoint))
            {
                _histClient = new RPC.Handler(histEndpoint, requestRetries, requestTimeout);
            }

            if (!string.IsNullOrEmpty(streamEndpoint))
            {
                _streamClient = new Streaming.Handler(streamEndpoint, this);

                Task.Factory.StartNew(() =>
                {
                    _streamClient.Subscribe(null, _streamCancelToken.Token);
                }, _shutDownToken.Token);
            }
        }

        internal void ExpiredSymbols(List<string> symbols)
        {
            _onExpiredSymbols?.Invoke(symbols);
        }

        internal void HeartBeat(AdminMessage beat)
        {
            _missedHBCounter = 0;
            //process heartbeat from TickProxy
            _onHeartBeat?.Invoke(beat);

            if (_isHBMissed)
            {
                //this means that Tickproxy just went online and started sending heart beats
                _isHBMissed = false;

                if (_onClientReconnect != null)
                {
                    _onClientReconnect(ClientId);
                }
            }
        }

        private volatile int _missedHBCounter = 0; //counts missed heart beats
        private volatile bool _isHBMissed = false;

        private void DisconnectCheckTimerEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (++_missedHBCounter > 2)
            {
                _isHBMissed = true;

                if (_onMissedHeartBeat != null)
                    _onMissedHeartBeat(ClientId);
            }
        }

        public async Task<ResponseInfo<Quote>> RequestDataAsync(QuotesRequestInfo request)
        {
            try
            {
                if (request != null && _rpcClient != null)
                    return await _rpcClient.SendRequestAsync<QuotesRequestInfo, ResponseInfo<Quote>>(request, _shutDownToken.Token);
            }
            catch (TaskCanceledException) { } // The task has been canceled.
            catch (ObjectDisposedException) { } // The System.Threading.CancellationTokenSource associated with cancellationToken was disposed.

            return default(ResponseInfo<Quote>);
        }

        public async Task<ResponseInfo<Quote>> GetDataRequestAndSubscribeAsync(QuotesRequestInfo req)
        {
            Task<ResponseInfo<Quote>> request = RequestDataAsync(req);

            await request.ContinueWith
                (
                    result =>
                    {
                        _streamClient.Subscribe(req.Symbols, _streamCancelToken.Token);
                    }
                );

            return await request;
        }

        internal void StreamingQuote(LiteQuote quote)
        {
            _onStreamingQuote?.Invoke(quote);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_hbTimer != null)
                        _hbTimer.Dispose();

                    _streamCancelToken.Cancel();
                    _streamCancelToken.Dispose();

                    _shutDownToken.Cancel();
                    _shutDownToken.Dispose();

                    if (_adminClient != null) _adminClient.Dispose();
                    if (_streamClient != null) _streamClient.Dispose();
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

        #region Events
        private readonly object _lock = new object();
        public event OnStreamingQuoteDelegate OnStreamingQuote
        {
            add
            {
                lock (_lock)
                    _onStreamingQuote += value;
            }
            remove
            {
                lock (_lock)
                    _onStreamingQuote -= value;
            }
        }

        //public event OnStreamingBarDelegate OnStreamingBar
        //{
        //    add
        //    {
        //        lock (_lock)
        //            _onStreamingBar += value;
        //    }
        //    remove
        //    {
        //        lock (_lock)
        //            _onStreamingBar -= value;
        //    }
        //}

        public event OnExpiredSymbolsDelegate OnExpiredSymbols
        {
            add
            {
                lock (_lock)
                    _onExpiredSymbols += value;
            }
            remove
            {
                lock (_lock)
                    _onExpiredSymbols -= value;
            }
        }

        public event OnHeartBeatDelegate OnHeartBeat
        {
            add
            {
                lock (_lock)
                    _onHeartBeat += value;
            }
            remove
            {
                lock (_lock)
                    _onHeartBeat -= value;
            }
        }

        public event OnStreamClientMissedHeartBeatDelegate OnStreamClientMissedHeartBeat
        {
            add
            {
                lock (_lock)
                    _onMissedHeartBeat += value;
            }
            remove
            {
                lock (_lock)
                    _onMissedHeartBeat -= value;
            }
        }

        public event OnStreamClientReconnectDelegate OnStreamClientReconnect
        {
            add
            {
                lock (_lock)
                    _onClientReconnect += value;
            }
            remove
            {
                lock (_lock)
                    _onClientReconnect -= value;
            }
        }
        #endregion
    }
}
