namespace EveCacheParser.STypes
{
    internal sealed class SShortType : SType
    {
        #region Constructors

        internal SShortType(short value)
            : base(StreamType.Short)
        {
            Value = value;
        }

        #endregion


        #region Properties

        public short Value { get; private set; }

        #endregion


        #region Methods

        public override SType Clone()
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
