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

        internal SDictType(SDictType source)
            : base(source)
        {
            GivenLength = source.GivenLength;
        }

        #endregion


        #region Properties

        public uint GivenLength { get; set; }

        #endregion


        #region Methods

        public override void AddMember(SType type)
        {
            if (!(Members.Length < GivenLength))
                throw new SystemException();

            Members.Add(type);
        }

        public override SType Clone()
        {
            return (SDictType)MemberwiseClone();
        }

        public SType GetByName(string target)
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
            return "<SDict>";
        }

        #endregion Methods
    }
}
