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

        public string Value { get; set; }

        #endregion


        #region Methods

        public override SType Clone()
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
