﻿namespace EveCacheParser.STypes
{
    internal class SLongType : SType
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SLongType"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        internal SLongType(StreamType type)
            : base(type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SLongType"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal SLongType(long value)
            : base(StreamType.Long)
        {
            Value = value;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        protected internal long Value { get; protected set; }

        #endregion 


        #region Methods

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal override SType Clone()
        {
            return (SLongType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("<SLongType '{0}'>", Value);
        }

        #endregion
    }
}
