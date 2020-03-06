using MessagePack;
using ProtoBuf;
using System;
using System.Text;

namespace AOS.Connector.TickProxy.DTO
{
    [ProtoContract]
    [MessagePackObject]
    public sealed class OptionExtension
    {
        [ProtoMember(1)]
        [Key(1)]
        public double StrikePrice { get; set; }

        [ProtoMember(2)]
        [Key(2)]
        public DateTime Expiration { get; set; }

        [ProtoMember(3)]
        [Key(3)]
        public long SharePerContract { get; set; }

        [ProtoMember(4)]
        [Key(4)]
        public long SharePerContract1 { get; set; }

        [ProtoMember(5)]
        [Key(5)]
        public long SharePerContract2 { get; set; }

        [ProtoMember(6)]
        [Key(6)]
        public long SharePerContract3 { get; set; }

        [ProtoMember(7)]
        [Key(7)]
        public string ExpirationFrequencyCode { get; set; }

        [ProtoMember(8)]
        [Key(8)]
        public string ProductType { get; set; }

        [ProtoMember(9)]
        [Key(9)]
        public long PreviousVolume { get; set; }

        [ProtoMember(10)]
        [Key(10)]
        public long OpenInterest { get; set; }

        /// <summary>
        /// C = Call, P = Put
        /// </summary>
        [ProtoMember(12)]
        [Key(12)]
        public string OptionType { get; set; }

        /// <summary>
        /// AM/PM settlement
        /// </summary>
        [ProtoMember(13)]
        [Key(13)]
        public string SettlementStyle { get; set; }

        public OptionExtension()
        { }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}", StrikePrice, Expiration.Date, SharePerContract, ExpirationFrequencyCode, ProductType, PreviousVolume, OpenInterest);
        }

        /// <summary>
        /// DO NOT CALL THIS OFTEN. BAD PERFORMANCE
        /// </summary>
        /// <returns></returns>
        public string PrintAllFields()
        {
            StringBuilder fields = new StringBuilder();
            foreach (System.ComponentModel.PropertyDescriptor descriptor in System.ComponentModel.TypeDescriptor.GetProperties(this))
            {
                fields.AppendFormat("{0}={1}; ", descriptor.Name, descriptor.GetValue(this));
            }
            return fields.ToString();
        }
    }
}
