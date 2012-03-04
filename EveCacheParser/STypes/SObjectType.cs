namespace EveCacheParser.STypes
{
    internal sealed class SObjectType : SType
    {
        #region Constructors

        internal SObjectType()
            : base(StreamType.ClassObject)
        {
        }

        #endregion


        #region Properties

        internal string Name
        {
            get
            {
                SType current = this;
                while (current.Members.Length > 0)
                    current = current.Members[0];

                SStringType str = current as SStringType;

                return str != null ? str.Value : string.Empty;
            }
        }

        #endregion


        #region Methods

        internal override SType Clone()
        {
            return (SObjectType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SObjectType '{0}' [{1:X4}]>", Name, DebugID);
        }

        #endregion
    }
}
