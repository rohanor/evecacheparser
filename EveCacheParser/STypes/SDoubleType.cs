namespace EveCacheParser.STypes
{
    internal sealed class SDoubleType : SType
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SDoubleType"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal SDoubleType(double value)
            : base(StreamType.DoubleZero)
        {
            DoubleValue = value;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal override SType Clone()
        {
            return (SDoubleType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("<SDoubleType '{0}'>", DoubleValue);
        }

        #endregion
    }
}
