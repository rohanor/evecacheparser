namespace EveCacheParser.STypes
{
    internal sealed class SShortType : SLongType
    {
        #region Constructors

        internal SShortType(short value)
            : base(StreamType.Short)
        {
            Value = value;
        }

        #endregion


        #region Methods

        internal override SType Clone()
        {
            return (SShortType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SShortType '{0}'>", Value);
        }

        #endregion
    }
}
