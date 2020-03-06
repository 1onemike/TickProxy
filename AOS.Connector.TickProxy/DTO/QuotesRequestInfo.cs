using MessagePack;
using ProtoBuf;
using System.Collections.Generic;

namespace AOS.Connector.TickProxy.DTO
{
    [ProtoContract]
    [MessagePackObject]
    public sealed class QuotesRequestInfo
    {
        private enumProviderType _providerOverride = enumProviderType.Undefined;

        /// <summary>
        /// Collection of symbols to lookup quotes for. Can be null technically, but in practice, ideally should never be null or empty.
        /// </summary>
        [ProtoMember(1)]
        [Key(1)]
        public List<string> Symbols { get; set; }

        /// <summary>
        /// Name of the requester (primarily used for logging). Can be web page URL, service name, etc.
        /// </summary>
        [ProtoMember(3)]
        [Key(3)]
        public string Requester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(6, IsRequired = true)]
        [Key(6)]
        public RPCRequestType RequestType { get; set; }

        //[ProtoMember(7)]
        //[Key(7)]
        //public byte[] RequestID { get; set; }

        [ProtoMember(9, IsRequired = true)]
        [Key(9)]
        public enumProviderType ProviderOverride
        {
            get { return _providerOverride; }
            set { _providerOverride = value; }
        }


        #region Constructors
        /// <summary>
        /// Parameterless constructor is required for ProtoBuf serialization
        /// </summary>
        public QuotesRequestInfo()
        {
            RequestType = RPCRequestType.LastQuote;
        }



        #endregion Constructors
    }
}
