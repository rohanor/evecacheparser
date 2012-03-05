using System;

namespace EveCacheParser.STypes
{
    internal sealed class SBooleanType : SLongType
    {
        #region Constructors

        internal SBooleanType(byte value)
            : base(StreamType.Byte)
        {
            Value = value;
        }

        #endregion


        #region Methods

        internal override SType Clone()
        {
            return (SBooleanType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SBooleanType '{0}'>", Convert.ToBoolean(Value));
        }

        #endregion
    }
}
