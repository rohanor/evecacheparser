using System.Globalization;

namespace EveCacheParser.STypes
{
    internal sealed class SShortType : SLongType
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SShortType"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal SShortType(short value)
            : base(StreamType.Short)
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
            return (short)Value;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A memberwise clone of this instance.</returns>
        internal override SType Clone()
        {
            return (SShortType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "<SShortType '{0}'>", Value);
        }

        #endregion
    }
}
