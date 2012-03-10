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
            Value = value;
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
            return (byte)Value;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A memberwise clone of this instance.</returns>
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
            return string.Format("<SByteType '{0}'>", Value);
        }

        #endregion
    }
}
