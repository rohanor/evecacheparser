namespace EveCacheParser.STypes
{
    internal class SNoneType : SType
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNoneType"/> class.
        /// </summary>
        internal SNoneType()
            : base(StreamType.None)
        {
        }

        #endregion Constructors


        #region Methods

        /// <summary>
        /// Returns a <see cref="System.Object"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Object"/> that represents this instance.
        /// </returns>
        internal override object ToObject()
        {
            return "None";
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A memberwise clone of this instance.</returns>
        internal override SType Clone()
        {
            return (SNoneType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "<SNone>";
        }

        #endregion Methods
    }
}
