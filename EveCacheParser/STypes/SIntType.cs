namespace EveCacheParser.STypes
{
    internal sealed class SIntType : SLongType
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SIntType"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal SIntType(int value)
            : base(StreamType.Int)
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
            return (int)Value;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A memberwise clone of this instance.</returns>
        internal override SType Clone()
        {
            return (SIntType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("<SIntType '{0}'>", Value);
        }

        #endregion
    }
}
