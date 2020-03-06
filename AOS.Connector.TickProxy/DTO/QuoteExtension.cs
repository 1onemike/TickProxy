using MessagePack;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS.Connector.TickProxy.DTO
{
    [ProtoContract]
    [MessagePackObject]
    public sealed class QuoteExtension
    {
        [ProtoMember(1)]
        [Key(1)]
        public double PreviousAsk { get; set; }
        [ProtoMember(2)]
        [Key(2)]
        public double PreviousBid { get; set; }
        [ProtoMember(3)]
        [Key(3)]
        public double PreviousClose { get; set; }
        [Obsolete("")]
        [ProtoMember(4)]
        [Key(4)]
        public DateTime CloseDate { get; set; }

        [ProtoMember(5)]
        [Key(5)]
        public double NetAssetValue { get; set; }
        [ProtoMember(6)]
        [Key(6)]
        public double YTDReturn { get; set; }
        [ProtoMember(7)]
        [Key(7)]
        public double OfferPrice { get; set; }
        ///////////
        [ProtoMember(8)]
        [Key(8)]
        public string SymbolName { get; set; }

        [ProtoMember(9)]
        [Key(9)]
        public double Low52Week { get; set; }
        [ProtoMember(10)]
        [Key(10)]
        public double High52Week { get; set; }

        /// <summary>
        /// Average 30 day volume
        /// </summary>
        [ProtoMember(11)]
        [Key(11)]
        public long AvgVolume { get; set; }
        [ProtoMember(12)]
        [Key(12)]
        public long Shares { get; set; }
        [ProtoMember(13)]
        [Key(13)]
        public DividendType DividendType { get; set; }
        [ProtoMember(14)]
        [Key(14)]
        public DateTime ExDivDate { get; set; }
        [ProtoMember(15)]
        [Key(15)]
        public DateTime DivDate { get; set; }
        [ProtoMember(16)]
        [Key(16)]
        public uint RegDivFrequency { get; set; }
        [ProtoMember(17)]
        [Key(17)]
        public double RegDivAmount { get; set; }
        [ProtoMember(18)]
        [Key(18)]
        public double SpecialDivAmount { get; set; }
        [ProtoMember(19)]
        [Key(19)]
        public double SpecialDivFrequency { get; set; }
        [ProtoMember(20)]
        [Key(20)]
        public DateTime SpecialDivDate { get; set; }
        [ProtoMember(21)]
        [Key(21)]
        public DateTime SpecialDivExDate { get; set; }
        /// <summary>
        /// Earnings Per Share
        /// </summary>
        [ProtoMember(22)]
        [Key(22)]
        public double EPS { get; set; }
        [ProtoMember(23)]
        [Key(23)]
        public string Issuer { get; set; }

        ////////////
        [ProtoMember(24)]
        [Key(24)]
        public double MarketCap { get; set; }
        [ProtoMember(25)]
        [Key(25)]
        public double Yield { get; set; }
        [ProtoMember(26)]
        [Key(26)]
        public double PERatio { get; set; }
        [ProtoMember(27)]
        [Key(27)]
        public DateTime High52WeekDate { get; set; }
        [ProtoMember(28)]
        [Key(28)]
        public DateTime Low52WeekDate { get; set; }
        [ProtoMember(29)]
        [Key(29)]
        public long AvgVolume10Day { get; set; }
        [ProtoMember(30)]
        [Key(30)]
        public double Beta { get; set; }
        [ProtoMember(31)]
        [Key(31)]
        public long ShortInterest { get; set; }
        [ProtoMember(32)]
        [Key(32)]
        public int SicCode { get; set; }
        [ProtoMember(33)]
        [Key(33)]
        public string PrimaryExchange { get; set; }
        [ProtoMember(34)]
        [Key(34)]
        public int IssueType { get; set; }

        private const double Epsilon = 0.0000001;


        public override string ToString()
        {
            return string.Format("{0}, {1} ,{2}", Issuer, Low52Week, High52Week);
        }

        /// <summary>
        /// DO NOT CALL THIS OFTEN. VERY BAD PERFORMANCE
        /// </summary>
        /// <returns></returns>
        public string PrintAllFields()
        {
            string fields = string.Empty;
            foreach (System.ComponentModel.PropertyDescriptor descriptor in System.ComponentModel.TypeDescriptor.GetProperties(this))
            {
                fields += string.Format("{0} = {1};\n", descriptor.Name, descriptor.GetValue(this));
            }
            return fields;
        }
    }
}
