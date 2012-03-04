using System;
using System.Collections.Generic;
using System.Text;

namespace EveCacheParser.STypes
{
    internal sealed class SDBRowType : SType
    {
        private readonly IEnumerable<byte> m_data;


        #region Constructors

        internal SDBRowType(IEnumerable<byte> data)
            : base(StreamType.CompressedDBRow)
        {
            m_data = data;
            IsLast = false;
        }

        #endregion


        #region Properties

        public bool IsLast { get; set; }

        #endregion


        #region Methods

        public override SType Clone()
        {
            return (SDBRowType)MemberwiseClone();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<SDBRowType ");

            foreach (byte type in m_data)
            {
                sb.Append(String.Format("{0:X2}", type));
            }

            if (IsLast)
                sb.Append(" LAST");

            sb.Append(">");
            return sb.ToString();
        }

        #endregion
    }
}
