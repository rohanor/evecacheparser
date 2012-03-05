using System;

namespace EveCacheParser.STypes
{
    internal sealed class STupleType : SType
    {
        private readonly uint m_length;


        #region Constructors

        internal STupleType(uint length)
            : base(StreamType.Tuple)
        {
            m_length = length;
        }

        #endregion


        #region Methods

        internal override void AddMember(SType node)
        {
            if (Members.Count > m_length)
                throw new IndexOutOfRangeException("Members exceed collection capacity");

            Members.Add(node);
        }

        internal override SType Clone()
        {
            return (STupleType)MemberwiseClone();
        }

        public override string ToString()
        {
            return "<STupleType>";
        }

        #endregion
    }
}
