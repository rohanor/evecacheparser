namespace EveCacheParser.STypes
{
    internal sealed class SByteType : SType
    {
        #region Constructors

        internal SByteType(byte value)
            : base(StreamType.Byte)
        {
            Value = value;
        }

        #endregion


        #region Properties

        public byte Value { get; private set; }

        #endregion


        #region Methods

        public override SType Clone()
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
