namespace EveCacheParser.STypes
{
    internal sealed class SDoubleType : SType
    {
        #region Constructors

        internal SDoubleType(double value)
            : base(StreamType.Double)
        {
            Value = value;
        }

        #endregion


        #region Properties

        private double Value { get; set; }

        #endregion


        #region Methods

        internal override SType Clone()
        {
            return (SDoubleType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SDoubleType '{0}'>", Value);
        }

        #endregion
    }
}
