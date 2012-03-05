namespace EveCacheParser.STypes
{
    internal sealed class SReferenceType : SType
    {
        #region Constructors

        internal SReferenceType(byte id)
            : base(StreamType.StringRef)
        {
            ID = id;
            Name = StringsTable.GetStringByID(id);

            if (string.IsNullOrWhiteSpace(Name))
                Name = "Unknown";
        }

        #endregion


        #region Properties

        private byte ID { get; set; }

        private string Name { get; set; }

        #endregion


        #region Methods

        internal override SType Clone()
        {
            return (SReferenceType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SReferenceType ID: {0} '{1}'>", ID, Name);
        }

        #endregion
    }
}
