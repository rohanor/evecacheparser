namespace EveCacheParser.STypes
{
    internal sealed class SIdentType : SType
    {
        #region Constructors

        internal SIdentType(string name)
            : base(StreamType.Buffer)
        {
            Value = name;
        }

        #endregion


        #region Properties

        public string Value { get; set; }

        #endregion Properties


        #region Methods

        public override SType Clone()
        {
            return (SIdentType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SIdent '{0}'>", Value);
        }

        #endregion
    }
}
