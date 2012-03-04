namespace EveCacheParser.STypes
{
    internal sealed class SMarkerType : SType
    {
        #region Constructors

        internal SMarkerType(byte id)
            : base(StreamType.Marker)
        {
            ID = id;
        }

        #endregion


        #region Properties

        public byte ID { get; set; }

        #endregion Properties


        #region Methods

        public override SType Clone()
        {
            return (SMarkerType)MemberwiseClone();
        }

        public override string ToString()
        {
            string name = StringsTable.GetStringByID(ID);
            if (name == string.Empty)
                name = string.Format("UNKNOWN: {0}", ID);

            return string.Format("<SMarker ID: {0} '{1}'>", ID, name);
        }

        #endregion
    }
}
