namespace EveCacheParser.STypes
{
    internal class SStringType : SType
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SStringType"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        internal SStringType(StreamType type)
            : base(type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SStringType"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        internal SStringType(string text)
            : base(StreamType.StringGlobal)
        {
            Text = text;
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
            return string.Format("<SStringType '{0}'>", Text);
        }

        #endregion
    }
}
