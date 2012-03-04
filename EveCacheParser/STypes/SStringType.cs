namespace EveCacheParser.STypes
{
    internal sealed class SStringType : SType
    {
        #region Constructors

        internal SStringType(string value)
            : base(StreamType.StringGlobal)
        {
            Value = value;
        }

        #endregion


        #region Properties

        internal string Value { get; private set; }

        #endregion


        #region Methods

        internal override SType Clone()
        {
            return (SStringType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SStringType '{0}'>", Value);
        }

        #endregion
    }
}
