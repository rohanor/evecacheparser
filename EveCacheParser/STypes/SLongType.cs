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

        public long Value { get; private set; }

        #endregion Properties


        #region Methods

        public override SType Clone()
        {
            return (SLongType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SLong '{0}'>", Value);
        }

        #endregion
    }
}
