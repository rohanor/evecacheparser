namespace EveCacheParser.STypes
{
    internal sealed class SIntType : SType
    {
        #region Constructors

        internal SIntType(int value)
            : base(StreamType.Int)
        {
            Value = value;
        }

        #endregion


        #region Properties

        internal int Value { get; private set; }

        #endregion


        #region Methods

        internal override SType Clone()
        {
            return (SIntType)MemberwiseClone();
        }

        internal override string ToString()
        {
            return string.Format("<SIntType '{0}'>", Value);
        }

        #endregion
    }
}
