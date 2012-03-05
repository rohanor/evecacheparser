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

        internal override void AddMember(SType type)
        {
            if (Members.Count > m_length)
                throw new IndexOutOfRangeException("Members exceed collection capacity");

            base.AddMember(type);
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
