﻿using System;

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

        internal STupleType(STupleType source)
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
            return (STupleType)MemberwiseClone();
        }

        public override string ToString()
        {
            return "<STupleType>";
        }

        #endregion
    }
}
