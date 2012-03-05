namespace EveCacheParser.STypes
{
    internal sealed class SSubStreamType : SStreamType
    {
        #region Constructors

        internal SSubStreamType()
            : base(StreamType.SubStream)
        {
        }

        #endregion


        #region Methods

        internal override SType Clone()
        {
            return (SSubStreamType)MemberwiseClone();
        }

        public override string ToString()
        {
            return "<SSubStreamType>";
        }

        #endregion
    }
}
