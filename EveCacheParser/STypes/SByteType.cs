namespace EveCacheParser.STypes
{
    internal sealed class SByteType : SLongType
    {
        #region Constructors

        internal SByteType(byte value)
            : base(StreamType.Byte)
        {
            Value = value;
        }

        #endregion


        #region Methods

        internal override SType Clone()
        {
            return (SByteType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SByteType '{0}'>", Value);
        }

        #endregion
    }
}
