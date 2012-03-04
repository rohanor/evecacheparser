namespace EveCacheParser.STypes
{
    internal sealed class SFloatType : SType
    {
        #region Constructors

        internal SFloatType(float value)
            : base(StreamType.Float)
        {
            Value = value;
        }

        #endregion


        #region Properties

        private float Value { get; set; }

        #endregion


        #region Methods

        internal override SType Clone()
        {
            return (SFloatType)MemberwiseClone();
        }

        internal override string ToString()
        {
            return string.Format("<SFloatType '{0}'>", Value);
        }

        #endregion
    }
}
