namespace EveCacheParser.STypes
{
    internal sealed class SStreamType : SType
    {
        #region Constructors

        internal SStreamType()
            : base(StreamType.StreamStart)
        {
        }

        internal SStreamType(StreamType streamType)
            : base(streamType)
        {
        }

        #endregion 


        #region Methods

        public override SType Clone()
        {
            return (SStreamType)MemberwiseClone();
        }

        public override string ToString()
        {
            return "<SStreamType>";
        }

        #endregion Methods
    }
}
