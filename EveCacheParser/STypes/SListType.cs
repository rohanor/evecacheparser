using System;

namespace EveCacheParser.STypes
{
    internal sealed class SListType : SType
    {
        #region Constructors

        internal SListType(uint len)
            : base(StreamType.List)
        {
            GivenLength = len;
        }

        internal SListType(SListType source)
            : base(source)
        {
            GivenLength = source.GivenLength;
        }

        #endregion


        #region Properties

        public uint GivenLength { get; set; }

        #endregion


        #region Methods

        public override void AddMember(SType node)
        {
            if (!(Members.Length < GivenLength))
                throw new SystemException();

            Members.Add(node);
        }

        public override SType Clone()
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
