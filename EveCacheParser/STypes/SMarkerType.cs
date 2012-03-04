namespace EveCacheParser.STypes
{
    internal sealed class SMarkerType : SType
    {
        #region Constructors

        internal SMarkerType(byte id)
            : base(StreamType.Marker)
        {
            ID = id;
            Name = StringsTable.GetStringByID(id);

            if (string.IsNullOrWhiteSpace(Name))
                Name = "Unknown";
        }

        #endregion


        #region Properties

        public byte ID { get; set; }

        public string Name { get; set; }

        #endregion Properties


        #region Methods

        public override SType Clone()
        {
            return (SMarkerType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SMarkerType ID: {0} '{1}'>", ID, Name);
        }

        #endregion
    }
}
