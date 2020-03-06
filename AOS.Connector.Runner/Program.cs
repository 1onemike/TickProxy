using AOS.Connector.TickProxy;
using AOS.Connector.TickProxy.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS.Connector.Runner
{
    class Program
    {
        private static Client _client;
        static void Main(string[] args)
        {
            string adminEndpoint = "tcp://10.11.41.51:5550";
            string rpcEndpoint = "tcp://10.11.41.51:5556";
            string streamEndpoint = "tcp://10.11.41.51:5555";
            string histEndpoint = "tcp://10.11.41.51:5558";

            _client = new Client(adminEndpoint, rpcEndpoint, streamEndpoint, histEndpoint);

            _client.OnStreamingQuote += TickProxyClient_OnStreamingQuote;
            //_client.OnStreamingBar += TickProxyClient_OnStreamingBar;
            _client.OnExpiredSymbols += TickProxyClient_OnExpiredSymbols;
            _client.OnHeartBeat += TickProxyClient_OnHeartBeat;
            _client.OnStreamClientMissedHeartBeat += TickProxyClient_OnMissetHeartBeat;
            _client.OnStreamClientReconnect += TickProxyClient_OnTickProxyReconnect;

            var request = new QuotesRequestInfo() { Symbols = new List<string> { "SPY", "IWM" }, Requester = "OutsideClient" };



            try
            {


                Task.Run(() => _client.GetDataRequestAndSubscribeAsync(request)
                   .ContinueWith(r =>
                   {

                       Console.WriteLine(string.Join("\n", r.Result.ResponsePayload));
                   }));
            }
            catch
            {
                Console.WriteLine("Couldn't get feed");

            }


            Console.ReadLine();
        }

        private static void TickProxyClient_OnTickProxyReconnect(Guid clientId)
        {

        }

        private static void TickProxyClient_OnMissetHeartBeat(Guid clientId)
        {

        }

        private static void TickProxyClient_OnHeartBeat(AdminMessage beat)
        {
            Console.WriteLine(beat);
        }

        private static void TickProxyClient_OnExpiredSymbols(List<string> expiredSymbols)
        {

        }

        private static void TickProxyClient_OnStreamingQuote(LiteQuote quote)
        {
            Console.WriteLine(quote);
        }
    }
}
