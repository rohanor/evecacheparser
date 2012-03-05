namespace EveCacheParser.STypes
{
    internal class SStreamType : SType
    {
        #region Constructors

        internal SStreamType(StreamType streamType)
            : base(streamType)
        {
        }

        #endregion 


        #region Methods

        internal override SType Clone()
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
