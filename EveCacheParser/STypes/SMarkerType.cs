namespace EveCacheParser.STypes
{
    internal class SMarkerType : SType
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SMarkerType"/> class.
        /// </summary>
        internal SMarkerType()
            : base(StreamType.Marker)
        {
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
            return "Marker";
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A memberwise clone of this instance.</returns>
        internal override SType Clone()
        {
            return (SMarkerType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "<SMarkerType>";
        }

        #endregion 
    }
}
