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

        private byte Value { get; set; }

        #endregion


        #region Methods

        internal override SType Clone()
        {
            return (SByteType)MemberwiseClone();
        }

        internal override string ToString()
        {
            return string.Format("<SByteType '{0}'>", Value);
        }

        #endregion
    }
}
