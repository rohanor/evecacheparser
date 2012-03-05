namespace EveCacheParser.STypes
{
    internal sealed class SIntType : SLongType
    {
        #region Constructors

        internal SIntType(int value)
            : base(StreamType.Int)
        {
            Value = value;
        }

        #endregion


        #region Methods

        internal override SType Clone()
        {
            return (SIntType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SIntType '{0}'>", Value);
        }

        #endregion
    }
}
