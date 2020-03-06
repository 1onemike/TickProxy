using ProtoBuf;

namespace AOS.Connector.TickProxy.DTO
{
    [ProtoContract]
    public enum enumAssetType
    {
        [ProtoEnum(Name = "Undefined", Value = 0)]
        Undefined = 0,
        [ProtoEnum(Name = "Equity", Value = 1)]
        Equity = 1,
        [ProtoEnum(Name = "Option", Value = 2)]
        Option = 2,
        [ProtoEnum(Name = "MutualFund", Value = 3)]
        MutualFund = 3,
        [ProtoEnum(Name = "MoneyMarket", Value = 4)]
        MoneyMarket = 4,
        [ProtoEnum(Name = "Bond", Value = 5)]
        Bond = 5,
        [ProtoEnum(Name = "Index", Value = 6)]
        Index = 6
    }

    /// <summary>
    /// Indicates the DivdendType applicable for given Symbol
    /// </summary>
    [ProtoContract]
    public enum DividendType
    {
        [ProtoEnum(Name = "Special", Value = 0)]
        Special = 0,
        [ProtoEnum(Name = "Regular", Value = 1)]
        Regular = 1
    }

    [ProtoContract]
    public enum RPCRequestType
    {
        [ProtoEnum(Name = "LastQuote", Value = 1)]
        LastQuote = 1,
        [ProtoEnum(Name = "WatchList", Value = 2)]
        WatchList = 2,
        [ProtoEnum(Name = "RemoveSymbolFromWatchList", Value = 3)]
        RemoveSymbolFromWatchList = 3,
        [ProtoEnum(Name = "OptionQuotesForUnderlying", Value = 4)]
        OptionQuotesForUnderlying = 4,
        [ProtoEnum(Name = "HistoricalBars", Value = 5)]
        HistoricalBars = 5,
        [ProtoEnum(Name = "ClearInternalCache", Value = 8)]
        ClearInternalCache = 8
    }

    [ProtoContract]
    public enum enumProviderType
    {
        [ProtoEnum(Name = "SecurityMaster", Value = 1)]
        SecurityMaster = 1,

        [ProtoEnum(Name = "Undefined", Value = 4)]
        Undefined = 4,

        [ProtoEnum(Name = "TickerPlant", Value = 7)]
        TickerPlant = 7
    }
}
