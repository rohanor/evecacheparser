using System.Globalization;

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
        /// Returns a <see cref="System.Object"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Object"/> that represents this instance.
        /// </returns>
        internal override object ToObject()
        {
            return DoubleValue;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A memberwise clone of this instance.</returns>
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
            return string.Format(CultureInfo.InvariantCulture, "<SDoubleType '{0}'>", DoubleValue);
        }

        #endregion
    }
}
