using MessagePack;
using System;
using System.Collections.Generic;

namespace AOS.Connector.TickProxy.DTO
{
    [MessagePackObject]
    public sealed class AdminMessage
    {
        [Key(4)]
        public int IncomingMessageCounter { get; set; }

        [Key(5)]
        public int NumberOfSubscriptions { get; set; }

        [Key(6)]
        public TimeSpan HeartBeatTS { get; set; }

        [Key(7)]
        public List<string> ExpiredSymbols { get; set; }

        [Key(8)]
        public bool IsExpired { get; set; }

        public override string ToString()
        {
            return string.Format("HB: {0} {1} {2}", HeartBeatTS, NumberOfSubscriptions, IncomingMessageCounter);
        }
    }
}
