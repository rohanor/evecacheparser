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

        public int Value { get; private set; }

        #endregion


        #region Methods

        public override SType Clone()
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
