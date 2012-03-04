namespace EveCacheParser.STypes
{
    internal sealed class SLongType : SType
    {
        #region Constructors

        internal SLongType(long value)
            : base(StreamType.Long)
        {
            Value = value;
        }

        #endregion


        #region Properties

        private long Value { get; set; }

        #endregion Properties


        #region Methods

        internal override SType Clone()
        {
            return (SLongType)MemberwiseClone();
        }

        internal override string ToString()
        {
            return string.Format("<SLongType '{0}'>", Value);
        }

        #endregion
    }
}
