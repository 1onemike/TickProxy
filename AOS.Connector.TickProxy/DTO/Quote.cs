using MessagePack;
using ProtoBuf;
using System;
using System.Text;

namespace AOS.Connector.TickProxy.DTO
{
    [ProtoContract]
    [MessagePackObject]
    public sealed class Quote : LiteQuote
    {
        private double _bidAskMidPrice;

        /// <summary>
        /// Ticker symbol for the underlying deliverable, if applicable
        /// </summary>
        [ProtoMember(1)]
        [Key(1)]
        public string UnderlyingSymbol { get; set; }

        [ProtoMember(2)]
        [Key(2)]
        public enumAssetType AssetClass { get; set; }

        /// <summary>
        /// The primary exchange (marketplace) in which the security is traded
        /// </summary>
        [ProtoMember(3)]
        [Key(3)]
        public string Exchange { get; set; }

        [Obsolete("")]
        [ProtoMember(4)]
        [Key(4)]
        public double BidAskMidPrice
        {
            get { return _bidAskMidPrice; }
            set { _bidAskMidPrice = Math.Round(value, 2); }
        }

        /// <summary>
        /// The final price at which the security traded on the the given CloseDate
        /// </summary>
        [ProtoMember(5)]
        [Key(5)]
        public double ClosePrice { get; set; }

        /// <summary>
        /// For options, the number of shares of the deliverable per contract
        /// </summary>
        [ProtoMember(6)]
        [Key(6)]
        public double Multiplier { get; set; }

        /// <summary>
        /// Extended quote details about the given security, if applicable
        /// </summary>
        [ProtoMember(8)]
        [Key(8)]
        public QuoteExtension ExtendedQuote { get; set; }

        [ProtoMember(9)]
        [Key(9)]
        public double Open { get; set; }

        [ProtoMember(10)]
        [Key(10)]
        public string Description { get; set; }

        [ProtoMember(11)]
        [Key(11)]
        public OptionExtension OptionExtend { get; set; }

        [ProtoMember(12)]
        [Key(12)]
        public DateTime? CloseDate { get; set; }

        /// <summary>
        /// Last known price for the options underlying (blank if this is not an option)
        /// </summary>
        [ProtoMember(14)]
        [Key(14)]
        public double UnderlyingPrice { get; set; }

        [ProtoMember(15)]
        [Key(15)]
        public bool IsError { get; set; }

        [ProtoMember(17)]
        [Key(17)]
        public double PreviousClose { get; set; }

        [ProtoMember(18)]
        [Key(18)]
        public uint OpenInterest { get; set; }

        public Quote()
        { }

        public override string ToString()
        {
            StringBuilder sbLog = new StringBuilder();

            sbLog.Append("Q,");
            sbLog.Append(QuoteTime.ToString("MM/dd/yyyy HH:mm:ss.fff")).Append(",");
            sbLog.Append(Symbol).Append(",");
            sbLog.Append(BidPrice).Append(",");
            sbLog.Append(AskPrice).Append(",");
            sbLog.Append(ClosePrice).Append(",");
            sbLog.Append(LastTradePrice).Append(",");
            sbLog.Append(NetChange).Append(",");
            sbLog.Append(base.Volume).Append(",");
            sbLog.Append(Description);

            return sbLog.ToString();
        }


        /// <summary>
        /// DO NOT CALL THIS OFTEN. REALLY BAD PERFORMANCE
        /// </summary>
        /// <returns></returns>
        public string PrintAllFields()
        {
            string fields = string.Empty;
            foreach (System.ComponentModel.PropertyDescriptor descriptor in System.ComponentModel.TypeDescriptor.GetProperties(this))
            {
                fields += string.Format("{0}={1}; ", descriptor.Name, descriptor.GetValue(this));
            }
            return fields;
        }
    }
}
