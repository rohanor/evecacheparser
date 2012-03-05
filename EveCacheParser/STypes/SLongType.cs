namespace EveCacheParser.STypes
{
    internal class SLongType : SType
    {
        #region Constructors

        internal SLongType(StreamType type)
            : base(type)
        {
        }

        internal SLongType(long value)
            : base(StreamType.Long)
        {
            Value = value;
        }

        #endregion


        #region Properties

        protected internal long Value { get; protected set; }

        #endregion 


        #region Methods

        internal override SType Clone()
        {
            return (SLongType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SLongType '{0}'>", Value);
        }

        #endregion
    }
}
