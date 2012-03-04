namespace EveCacheParser.STypes
{
    internal class SNoneType : SType
    {
        #region Constructors

        internal SNoneType()
            : base(StreamType.None)
        {
        }

        #endregion Constructors


        #region Methods

        internal override SType Clone()
        {
            return (SNoneType)MemberwiseClone();
        }

        internal override string ToString()
        {
            return "<SNone>";
        }

        #endregion Methods
    }
}
