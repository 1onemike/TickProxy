using MessagePack;
using ProtoBuf;
using System.Collections.Generic;

namespace AOS.Connector.TickProxy.DTO
{
    [ProtoContract]
    [MessagePackObject]
    public class ResponseInfo<T>
    {
        [ProtoMember(1)]
        [Key(1)]
        public string Requester { get; set; }

        [ProtoMember(2)]
        [Key(2)]
        public double? ProcessingTime { get; set; }

        IEnumerable<T> _payload;
        [ProtoMember(3)]
        [Key(3)]
        public IEnumerable<T> ResponsePayload
        {
            get { return _payload ?? (_payload = new List<T>()); }
            set { _payload = value; }
        }

        /// <summary>
        /// Thread id from the sender thread id
        /// </summary>
        [ProtoMember(4)]
        [Key(4)]
        public long IdentityID { get; set; }

        /// <summary>
        /// Requester ID where request is made from (used for Dealer-Router pattern)
        /// </summary>
        [ProtoMember(5)]
        [Key(5)]
        public byte[] RequesterAddress { get; set; }

        /// <summary>
        /// Indicates if response is successful
        /// </summary>
        [ProtoMember(6)]
        [Key(6)]
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Parameterless constructor is required for ProtoBuf serialization
        /// </summary>
        public ResponseInfo()
        { }

        public ResponseInfo(string requester)
        {
            Requester = requester;
        }

        public ResponseInfo(string requester, IEnumerable<T> list)
        {
            Requester = requester;
            ResponsePayload = list;
        }

        public override string ToString()
        {
            return string.Format("Requester: {0} ID: {1}", Requester, IdentityID);
        }
    }
}
