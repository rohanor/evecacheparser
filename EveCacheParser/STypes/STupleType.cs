using System;

namespace EveCacheParser.STypes
{
    internal sealed class STupleType : SType
    {
        #region Constructors

        internal STupleType(uint len)
            : base(StreamType.Tuple)
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
                throw new ArgumentOutOfRangeException();

            Members.Add(node);
        }

        internal override SType Clone()
        {
            return (STupleType)MemberwiseClone();
        }

        internal override string ToString()
        {
            return "<STupleType>";
        }

        #endregion
    }
}
