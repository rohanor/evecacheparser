namespace EveCacheParser.STypes
{
    internal sealed class SDBHeader : SType
    {
        #region Constructors

        internal SDBHeader()
            : base(StreamType.CompressedDBRow)
        {
        }

        #endregion


        #region Methods

        public override SType Clone()
        {
            return (SDBHeader)MemberwiseClone();
        }

        public override string ToString()
        {
            return "<SDBHeaderType>";
        }

        #endregion
    }
}
