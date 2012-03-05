namespace EveCacheParser.STypes
{
    internal sealed class SIdentType : SType
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SIdentType"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        internal SIdentType(string text)
            : base(StreamType.StringIdent)
        {
            Text = text;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal override SType Clone()
        {
            return (SIdentType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("<SIdentType '{0}'>", Text);
        }

        #endregion
    }
}
