using System;
using System.Globalization;

namespace EveCacheParser.STypes
{
    internal sealed class SBooleanType : SLongType
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SBooleanType"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal SBooleanType(byte value)
            : base(StreamType.Byte)
        {
            Boolean = Convert.ToBoolean(value);
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
            return Boolean;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A memberwise clone of this instance.</returns>
        internal override SType Clone()
        {
            return (SBooleanType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "<SBooleanType '{0}'>", Boolean);
        }

        #endregion
    }
}
