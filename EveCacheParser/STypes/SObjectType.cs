using System.Diagnostics;
using System.Globalization;
using System.Linq;

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


        #region Internal Properties

        /// <summary>
        /// Gets a value indicating whether this object is a 'DBRowDescriptor'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'DBRowDescriptor'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsDBRowDescriptor
        {
            get { return Name == "blue.DBRowDescriptor"; }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'RowList'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'RowList'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsRowList
        {
            get { return Name == "dbutil.RowList"; }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'RowDict'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'RowDict'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsRowDict
        {
            get { return Name == "dbutil.RowDict"; }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'CRowset'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'CRowset'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCRowset
        {
            get { return Name == "dbutil.CRowset"; }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'CFilterRowset'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'CFilterRowset'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCFilterRowset
        {
            get { return Name == "dbutil.CFilterRowset"; }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'CIndexedRowset'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'CIndexedRowset'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCIndexedRowset
        {
            get { return Name == "dbutil.CIndexedRowset"; }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'KeyVal'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'KeyVal'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsKeyVal
        {
            get { return Name == "util.KeyVal"; }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'CachedMethodCallResult'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'CachedMethodCallResult'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCachedMethodCallResult
        {
            get { return Name == "objectCaching.CachedMethodCallResult"; }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'CachedObject'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'CachedObject'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCachedObject
        {
            get { return Name == "util.CachedObject"; }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a 'CachedObject'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is a 'CachedObject'; otherwise, <c>false</c>.
        /// </value>
        internal bool IsObjectCachingCachedObject
        {
            get { return Name == "objectCaching.CachedObject"; }
        }

        #endregion


        #region Private Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
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

        #endregion


        #region Methods

        /// <summary>
        /// Returns a <see cref="System.Object"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Object"/> that represents this instance.
        /// </returns>
        internal override object ToObject()
        {
            if (IsRowList || IsCRowset)
            {
                return Members.Where(member => member != Members.First()).Select(
                    member => member.Members).SelectMany(obj => obj, (obj, type) => new { obj, type }).Where(
                        obj => !(obj.type is SObjectType)).Select(
                            obj => obj.type.ToObject()).ToList();
            }

            if (IsCFilterRowset || IsRowDict || IsCIndexedRowset)
                return ToDictionary(Members.Where(member => member != Members.First()).ToList());

            if (IsKeyVal || IsCachedObject || IsCachedMethodCallResult)
                return Members.Where(member => member != Members.First()).Select(member => member.ToObject()).ToList();
            
            if (IsObjectCachingCachedObject)
                return Members.Where(member => member != Members.First()).Select(member => member.ToObject()).ToList();

            if (IsDBRowDescriptor)
                return null;

            // 'Clone()' is returned for debugging purposes
            return Debugger.IsAttached ? Clone() : null;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A memberwise clone of this instance.</returns>
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
            return string.Format(CultureInfo.InvariantCulture, "<SObjectType '{0}' [{1:X4}]>", Name, DebugID);
        }

        #endregion
    }
}
