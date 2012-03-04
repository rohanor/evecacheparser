namespace EveCacheParser.STypes
{
    internal sealed class SDBRecordsType : SType
    {
        #region Constructors

        internal SDBRecordsType()
            : base(StreamType.CompressedDBRow)
        {
        }

        #endregion


        #region Methods

        public override SType Clone()
        {
            return (SDBRecordsType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Empty;
        }

        #endregion
    }
}
