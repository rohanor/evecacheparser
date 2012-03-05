namespace EveCacheParser.STypes
{
    internal sealed class SByteType : SLongType
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SByteType"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal SByteType(byte value)
            : base(StreamType.Byte)
        {
            LongValue = value;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal override SType Clone()
        {
            return (SByteType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("<SByteType '{0}'>", LongValue);
        }

        #endregion
    }
}
