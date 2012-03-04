namespace EveCacheParser.STypes
{
    internal sealed class SIdentType : SType
    {
        #region Constructors

        internal SIdentType(string name)
            : base(StreamType.StringIdent)
        {
            Value = name;
        }

        #endregion


        #region Properties

        internal string Value { get; private set; }

        #endregion Properties


        #region Methods

        internal override SType Clone()
        {
            return (SIdentType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SIdentType '{0}'>", Value);
        }

        #endregion
    }
}
