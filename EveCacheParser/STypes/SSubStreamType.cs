namespace EveCacheParser.STypes
{
    internal sealed class SSubStreamType : SType
    {
        #region Constructors

        internal SSubStreamType(int length)
            : base(StreamType.SubStream)
        {
            Length = length;
        }

        #endregion


        #region Properties

        private int Length { get; set; }

        #endregion


        #region Methods

        public override SType Clone()
        {
            return (SSubStreamType)MemberwiseClone();
        }

        public override string ToString()
        {
            return "<SSubStream>";
        }

        #endregion
    }
}
