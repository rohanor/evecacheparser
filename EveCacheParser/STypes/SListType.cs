using System;

namespace EveCacheParser.STypes
{
    internal sealed class SListType : SType
    {
        private readonly uint m_length;


        #region Constructors

        internal SListType(uint length)
            : base(StreamType.List)
        {
            m_length = length;
        }

        #endregion


        #region Methods

        internal override void AddMember(SType type)
        {
            if (Members.Count > m_length)
                throw new IndexOutOfRangeException("Members exceed collection capacity");

            base.AddMember(type);
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
