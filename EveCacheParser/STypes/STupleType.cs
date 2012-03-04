using System;

namespace EveCacheParser.STypes
{
    internal sealed class STuple : SType
    {
        #region Constructors

        internal STuple(uint len)
            : base(StreamType.Tuple)
        {
            GivenLength = len;
        }

        internal STuple(STuple source)
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
            return (STuple)MemberwiseClone();
        }

        public override string ToString()
        {
            return "<STuple>";
        }

        #endregion
    }
}
