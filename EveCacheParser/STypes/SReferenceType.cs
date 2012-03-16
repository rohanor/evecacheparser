using System.Globalization;

namespace EveCacheParser.STypes
{
    internal sealed class SReferenceType : SStringType
    {
        private readonly byte m_id;


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SReferenceType"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        internal SReferenceType(byte id)
            : base(StreamType.StringRef)
        {
            m_id = id;
            Text = StringsTable.GetStringByID(id);

            if (string.IsNullOrWhiteSpace(Text))
                Text = "Unknown";
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
            return Text;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A memberwise clone of this instance.</returns>
        internal override SType Clone()
        {
            return (SReferenceType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "<SReferenceType ID: {0} '{1}'>", m_id, Text);
        }

        #endregion
    }
}
