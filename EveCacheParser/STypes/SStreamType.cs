namespace EveCacheParser.STypes
{
    internal class SStreamType : SType
    {
        private readonly StreamType m_streamType;
        #region Constructors

        internal SStreamType(StreamType streamType)
            : base(streamType)
        {
            m_streamType = streamType;
        }

        #endregion 


        #region Methods

        internal override SType Clone()
        {
            return (SStreamType)MemberwiseClone();
        }

        public override string ToString()
        {
            return m_streamType == StreamType.StreamStart ? "<SStreamType>" : "<SSubStreamType>";
        }

        #endregion Methods
    }
}
