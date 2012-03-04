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

        public bool Value { get; private set; }

        #endregion


        #region Methods

        public override SType Clone()
        {
            return (SBooleanType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SBooleanType '{0}'>", Value);
        }

        #endregion
    }
}
