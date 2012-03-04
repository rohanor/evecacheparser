﻿namespace EveCacheParser.STypes
{
    internal sealed class SDBHeader : SType
    {
        #region Constructors

        internal SDBHeader()
            : base(StreamType.CompressedDBRow)
        {
        }

        #endregion


        #region Methods

        internal override SType Clone()
        {
            return (SDBHeader)MemberwiseClone();
        }

        internal override string ToString()
        {
            return "<SDBHeaderType>";
        }

        #endregion
    }
}
