namespace EveCacheParser.STypes
{
    internal sealed class SStringType : SType
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SStringType"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal SStringType(string value)
            : base(StreamType.StringGlobal)
        {
            Value = value;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        internal string Value { get; private set; }

        #endregion


        #region Methods

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal override SType Clone()
        {
            return (SStringType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("<SStringType '{0}'>", Value);
        }

        #endregion
    }
}
