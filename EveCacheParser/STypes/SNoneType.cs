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

        public override SType Clone()
        {
            return (SNoneType)MemberwiseClone();
        }

        public override string ToString()
        {
            return "<SNone>";
        }

        #endregion Methods
    }
}
