using System.Collections.Generic;

namespace Dumper
{
    class PriceHistoryEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PriceHistoryEntry"/> class.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public PriceHistoryEntry(IDictionary<object, object> entry)
        {
            HistoryDate = (long)entry["historyDate"];
            LowPrice = (decimal)(double)entry["lowPrice"];
            HighPrice = (decimal)(double)entry["highPrice"];
            AveragePrice = (decimal)(double)entry["avgPrice"];
            Volume = (long)entry["volume"];
            Orders = (int)entry["orders"];
        }

        internal long HistoryDate { get; private set; }

        internal decimal LowPrice { get; private set; }

        internal decimal HighPrice { get; private set; }

        internal decimal AveragePrice { get; private set; }

        internal long Volume { get; private set; }

        internal int Orders { get; private set; }

    }
}