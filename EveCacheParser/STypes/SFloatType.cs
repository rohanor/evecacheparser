﻿namespace EveCacheParser.STypes
{
    internal sealed class SFloatType : SType
    {
        #region Constructors

        internal SFloatType(float value)
            : base(StreamType.Float)
        {
            Value = value;
        }

        #endregion


        #region Properties

        public float Value { get; private set; }

        #endregion


        #region Methods

        public override SType Clone()
        {
            return (SFloatType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SFloatType '{0}'>", Value);
        }

        #endregion
    }
}
