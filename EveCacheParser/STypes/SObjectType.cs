namespace EveCacheParser.STypes
{
    internal sealed class SObjectType : SType
    {


        #region Constructors

        internal SObjectType()
            : base(StreamType.ClassObject)
        {
        }

        internal SObjectType(SType source)
            : base(source)
        {
        }

        #endregion


        #region Properties

        public string Name
        {
            get
            {
                SType current = this;
                while (current.Members.Length > 0)
                    current = current.Members[0];

                SStringType str = current as SStringType;

                if (str != null)
                    return str.Value;

                return string.Empty;
            }
        }

        #endregion


        #region Methods

        public override SType Clone()
        {
            return (SObjectType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SObject '{0}' [{1:X4}]>", Name, DebugID);
        }

        #endregion
    }
}
