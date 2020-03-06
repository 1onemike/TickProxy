using MessagePack;
using ProtoBuf;
using System;
using System.Text;

namespace AOS.Connector.TickProxy.DTO
{
    /// <summary>
    /// NOTE: This will likely be further trimmed down
    /// </summary>
    [ProtoInclude(15, typeof(Quote))]
    [ProtoContract]
    [MessagePackObject]
    public class LiteQuote
    {
        [ProtoMember(1)]
        [Key(1)]
        public DateTime QuoteTime { get; set; }

        [ProtoMember(2)]
        [Key(2)]
        public string Symbol { get; set; }

        [IgnoreMember]
        public string SymbolTopic { get; set; }

        [ProtoMember(3)]
        [Key(3)]
        public double BidPrice { get; set; }

        [ProtoMember(4)]
        [Key(4)]
        public long BidSize { get; set; }

        [ProtoMember(5)]
        [Key(5)]
        public double AskPrice { get; set; }

        [ProtoMember(6)]
        [Key(6)]
        public long AskSize { get; set; }

        [ProtoMember(7)]
        [Key(7)]
        public double LastTradePrice { get; set; }

        [ProtoMember(8)]
        [Key(8)]
        public long Volume { get; set; }

        [ProtoMember(9)]
        [Key(9)]
        public double High { get; set; }

        [ProtoMember(10)]
        [Key(10)]
        public double Low { get; set; }

        [ProtoMember(11)]
        [Key(11)]
        public double NetChange { get; set; }

        public override string ToString()
        {
            StringBuilder sbLog = new StringBuilder();

            sbLog.Append("LQ,");
            sbLog.Append(QuoteTime.ToString("MM/dd/yyyy HH:mm:ss.fff")).Append(",");
            sbLog.Append(Symbol).Append(",");
            sbLog.Append(BidPrice).Append(",");
            sbLog.Append(AskPrice).Append(",");
            sbLog.Append(LastTradePrice).Append(",");
            sbLog.Append(NetChange).Append(",");
            sbLog.Append(Volume);

            return sbLog.ToString();
        }
    }
}
