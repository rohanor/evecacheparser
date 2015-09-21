using System.Collections.Generic;

namespace Dumper
{
    sealed class MarketOrder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOrder"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        public MarketOrder(IDictionary<object, object> order)
        {
            Price = (decimal)(double)order["price"];
            VolumeRemaining = (double)order["volRemaining"];
            OrderID = (long)order["orderID"];
            IssueDate = (long)order["issueDate"];
            TypeID = (int)order["typeID"];
            VolumeEntered = (int)order["volEntered"];
            MinVolume = (int)order["minVolume"];
            StationID = (int)order["stationID"];
            RegionID = (int)order["regionID"];
            SolarSystemID = (int)order["solarSystemID"];
            Jumps = (int)order["jumps"];
            Range = (short)order["range"];
            Duration = (short)order["duration"];
            Bid = (bool)order["bid"];
        }

        internal decimal Price { get; private set; }

        internal double VolumeRemaining { get; private set; }

        internal long OrderID { get; private set; }

        internal long IssueDate { get; private set; }

        internal int TypeID { get; private set; }

        internal int VolumeEntered { get; private set; }

        internal int MinVolume { get; private set; }

        internal int StationID { get; private set; }

        internal int RegionID { get; private set; }

        internal int SolarSystemID { get; private set; }

        internal int Jumps { get; private set; }

        internal short Range { get; private set; }

        internal short Duration { get; private set; }

        internal bool Bid { get; private set; }
    }
}