namespace EveCacheParser.STypes
{
    internal sealed class SObjectType : SType
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SObjectType"/> class.
        /// </summary>
        internal SObjectType()
            : base(StreamType.ClassObject)
        {
        }

        #endregion


        #region Properties

        private string Name
        {
            get
            {
                SType current = this;
                while (current.Members.Count > 0)
                {
                    current = current.Members[0];
                }

                SStringType stringType = current as SStringType;

                return stringType != null ? stringType.Text : string.Empty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this objects name is a valid 'RowList' name.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this objects name is a valid 'RowList' name; otherwise, <c>false</c>.
        /// </value>
        internal bool IsValidRowListName
        {
            get { return Name == "dbutil.RowList"; }
        }

        /// <summary>
        /// Gets a value indicating whether this objects name is a valid 'DBRowDescriptor' name.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this objects name is a valid 'DBRowDescriptor' name; otherwise, <c>false</c>.
        /// </value>
        internal bool IsValidDBRowDescriptorName
        {
            get { return Name == "blue.DBRowDescriptor"; }
        }

        #endregion


        #region Methods

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal override SType Clone()
        {
            return (SObjectType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("<SObjectType '{0}' [{1:X4}]>", Name, DebugID);
        }

        #endregion
    }
}
