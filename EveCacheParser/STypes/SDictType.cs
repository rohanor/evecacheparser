using System;

namespace EveCacheParser.STypes
{
    internal sealed class SDictType : SType
    {
        #region Constructors

        internal SDictType(uint length)
            : base(StreamType.Dict)
        {
            GivenLength = length;
        }

        #endregion


        #region Properties

        private uint GivenLength { get; set; }

        #endregion


        #region Methods

        internal override void AddMember(SType type)
        {
            if (!(Members.Length < GivenLength))
                throw new SystemException();

            Members.Add(type);
        }

        internal override SType Clone()
        {
            return (SDictType)MemberwiseClone();
        }

        internal SType GetByName(string target)
        {
            if (Members.Length < 2 || (Members.Length & 1) > 0)
                return null;

            for (int i = 1; i < Members.Length; i += 2)
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
