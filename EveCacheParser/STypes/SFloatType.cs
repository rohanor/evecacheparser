namespace EveCacheParser.STypes
{
    internal sealed class SFloatType : SType
    {
        #region Constructors

        internal SFloatType(double value)
            : base(StreamType.Float)
        {
            Value = value;
        }

        #endregion


        #region Properties

        public double Value { get; private set; }

        #endregion


        #region Methods

        public override SType Clone()
        {
            return (SFloatType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SFloat '{0}'>", Value);
        }

        #endregion
    }
}
