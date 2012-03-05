using System;

namespace EveCacheParser.STypes
{
    internal sealed class SListType : SType
    {
        private readonly uint m_length;


        #region Constructors

        internal SListType(uint len)
            : base(StreamType.List)
        {
            m_length = len;
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
            return (SListType)MemberwiseClone();
        }

        public override string ToString()
        {
            return "<SListType>";
        }

        #endregion
    }
}
