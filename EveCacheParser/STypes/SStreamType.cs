﻿namespace EveCacheParser.STypes
{
    internal sealed class SStreamType : SType
    {
        #region Constructors

        internal SStreamType()
            : base(StreamType.StreamStart)
        {
        }

        internal SStreamType(StreamType streamType)
            : base(streamType)
        {
        }

        #endregion 


        #region Methods

        internal override SType Clone()
        {
            return (SStreamType)MemberwiseClone();
        }

        internal override string ToString()
        {
            return "<SStreamType>";
        }

        #endregion Methods
    }
}
