using System.Collections.Generic;

namespace Dumper
{
    internal class PriceHistoryEntry
    {
        public PriceHistoryEntry(IDictionary<object, object> entry)
        {
            HistoryDate = (long)entry["historyDate"];
            LowPrice = (long)entry["lowPrice"];
            HighPrice = (long)entry["highPrice"];
            AveragePrice = (long)entry["avgPrice"];
            Volume = (long)entry["volume"];
            Orders = (int)entry["orders"];
        }

        internal long HistoryDate { get; private set; }

        internal long LowPrice { get; private set; }

        internal long HighPrice { get; private set; }

        internal long AveragePrice { get; private set; }

        internal long Volume { get; private set; }

        internal int Orders { get; private set; }

    }
}