using NetMQ;
using NetMQ.Sockets;
using ProtoBuf;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AOS.Connector.TickProxy.RPC
{
    internal class Handler
    {
        private string _binding;
        private TimeSpan _timeout;

        private int _requestTryCount = 3;
        private int RequestTryCount
        {
            get { return _requestTryCount; }
            set { _requestTryCount = Math.Max(1, value); } // If allowing to set < 1, will never send any request
        }

        public Handler(string binding, int maxRetryNum, TimeSpan requestTimeout)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");

            _timeout = requestTimeout;
            RequestTryCount = maxRetryNum; // Really "try" count, not "retry" count as that implies total tries = retryCount + 1
            _binding = binding;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="ObjectDisposedException">The CancellationTokenSource associated with cancellationToken was disposed.</exception>
        /// <returns></returns>
        public virtual async Task<TResponse> SendRequestAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        {
            return await Task.Run(() => SendRequest<TRequest, TResponse>(request), cancellationToken).ConfigureAwait(false);
        }

        private TResponse SendRequest<TRequest, TResponse>(TRequest request)
        {
            TResponse resp = default(TResponse);

            int retriesLeft = RequestTryCount;

            NetMQMessage requestMessage = new NetMQMessage();
            using (MemoryStream memStream = new MemoryStream())
            {
                Serializer.Serialize(memStream, request);
                //requestMessage.AppendEmptyFrame(); //this is a 'delimeter' for the Router-Dealer pattern
                requestMessage.Append(memStream.ToArray());
            }

            bool expectReply = true;

            while (retriesLeft > 0 && expectReply)
            {
                using (NetMQSocket clientSocket = CreateServerSocket())
                {
                    if (clientSocket.TrySendMultipartMessage(requestMessage))
                    {
                        var responseMessage = new NetMQMessage();
                        while (expectReply)
                        {
                            if (!clientSocket.TryReceiveMultipartMessage(_timeout, ref responseMessage))
                            {
                                --retriesLeft;

                                if (retriesLeft < 1)
                                {
                                    //Server seems to be offline, abandoning
                                    retriesLeft = RequestTryCount;
                                    expectReply = false;

                                    KillSocket(clientSocket);

                                    break;
                                }
                            }

                            if (responseMessage != null && responseMessage.FrameCount > 0)
                            {
                                using (MemoryStream respStream = new MemoryStream(responseMessage.Last.Buffer))
                                {
                                    resp = Serializer.Deserialize<TResponse>(respStream);
                                }

                                KillSocket(clientSocket);
                                retriesLeft = RequestTryCount;
                                expectReply = false;
                            }
                        }
                    }
                    else
                    {
                        KillSocket(clientSocket);
                        --retriesLeft;

                        if (retriesLeft == 0)
                        {
                            break;
                        }
                    }
                }
            }
            return resp;
        }

        private void KillSocket(NetMQSocket clientSocket)
        {
            //clientSocket.Disconnect(_binding);
            clientSocket.Close();
        }

        private NetMQSocket CreateServerSocket()
        {
            var client = new RequestSocket();

            client.Options.Identity = Guid.NewGuid().ToByteArray();//_guid;
            client.Options.Linger = TimeSpan.Zero;

            client.Connect(_binding);
            return client;
        }
    }
}
