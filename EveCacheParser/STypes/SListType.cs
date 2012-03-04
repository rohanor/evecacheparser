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

        #endregion


        #region Properties

        private uint GivenLength { get; set; }

        #endregion


        #region Methods

        internal override void AddMember(SType node)
        {
            if (!(Members.Length < GivenLength))
                throw new SystemException();

            Members.Add(node);
        }

        internal override SType Clone()
        {
            return (SListType)MemberwiseClone();
        }

        internal override string ToString()
        {
            return "<SListType>";
        }

        #endregion
    }
}
