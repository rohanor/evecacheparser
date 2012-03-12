namespace EveCacheParser.STypes
{
    internal class SStreamType : SType
    {
        private readonly StreamType m_streamType;


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SStreamType"/> class.
        /// </summary>
        /// <param name="streamType">Type of the stream.</param>
        internal SStreamType(StreamType streamType)
            : base(streamType)
        {
            m_streamType = streamType;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Returns a <see cref="System.Object"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Object"/> that represents this instance.
        /// </returns>
        internal override object ToObject()
        {
            return "StreamData";
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A memberwise clone of this instance.</returns>
        internal override SType Clone()
        {
            return (SStreamType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return m_streamType == StreamType.StreamStart ? "<SStreamType>" : "<SSubStreamType>";
        }

        #endregion Methods
    }
}
