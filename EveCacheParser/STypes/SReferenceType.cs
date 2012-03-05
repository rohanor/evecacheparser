namespace EveCacheParser.STypes
{
    internal sealed class SReferenceType : SType
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
        
        
        #region Properties

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        internal string Text { get; private set; }

        #endregion Properties


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
            return string.Format("<SReferenceType ID: {0} '{1}'>", m_id, Text);
        }

        #endregion
    }
}
