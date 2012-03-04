using System;

namespace EveCacheParser.STypes
{
    internal sealed class SBooleanType : SType
    {
        #region Constructors

        internal SBooleanType(byte value)
            : base(StreamType.Byte)
        {
            Value = Convert.ToBoolean(value);
        }

        #endregion


        #region Properties

        private bool Value { get; set; }

        #endregion


        #region Methods

        internal override SType Clone()
        {
            return (SBooleanType)MemberwiseClone();
        }

        internal override string ToString()
        {
            return string.Format("<SBooleanType '{0}'>", Value);
        }

        #endregion
    }
}
