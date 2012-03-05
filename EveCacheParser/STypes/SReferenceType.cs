namespace EveCacheParser.STypes
{
    internal sealed class SReferenceType : SType
    {
        private readonly byte m_id;
        private readonly string m_name;


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SReferenceType"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        internal SReferenceType(byte id)
            : base(StreamType.StringRef)
        {
            m_id = id;
            m_name = StringsTable.GetStringByID(id);

            if (string.IsNullOrWhiteSpace(m_name))
                m_name = "Unknown";
        }

        #endregion


        #region Methods

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
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
            return string.Format("<SReferenceType ID: {0} '{1}'>", m_id, m_name);
        }

        #endregion
    }
}
