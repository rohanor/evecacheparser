namespace EveCacheParser.STypes
{
    internal sealed class SIdentType : SType
    {
        #region Constructors

        internal SIdentType(string name)
            : base(StreamType.IdentString)
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
            return string.Format("<SIdentType '{0}'>", Value);
        }

        #endregion
    }
}
