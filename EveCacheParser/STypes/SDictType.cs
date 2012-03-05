using System;

namespace EveCacheParser.STypes
{
    internal sealed class SDictType : SType
    {
        private readonly uint m_length;


        #region Constructors

        internal SDictType(uint length)
            : base(StreamType.Dict)
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
            return (SDictType)MemberwiseClone();
        }

        public override string ToString()
        {
            return "<SDictType>";
        }

        #endregion Methods
    }
}
