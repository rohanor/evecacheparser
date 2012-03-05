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
            if (Members.Count >= m_length)
                throw new IndexOutOfRangeException("Members exceed collection capacity");

            Members.Add(type);
        }

        internal override SType Clone()
        {
            return (SDictType)MemberwiseClone();
        }

        internal SType GetByName(string target)
        {
            if (Members.Count < 2 || (Members.Count & 1) > 0)
                return null;

            for (int i = 1; i < Members.Count; i += 2)
            {
                if (Members[i] is SIdentType && ((SIdentType)Members[i]).Value == target)
                    return Members[i - 1];
            }

            return null;
        }

        public override string ToString()
        {
            return "<SDictType>";
        }

        #endregion Methods
    }
}
