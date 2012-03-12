using System;
using System.Collections.Generic;
using System.Linq;

namespace EveCacheParser.STypes
{
    internal sealed class STupleType : SType
    {
        private readonly uint m_length;


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="STupleType"/> class.
        /// </summary>
        /// <param name="length">The length.</param>
        internal STupleType(uint length)
            : base(StreamType.Tuple)
        {
            m_length = length;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Adds the member.
        /// </summary>
        /// <param name="type">The type.</param>
        internal override void AddMember(SType type)
        {
            if (Members.Count > m_length)
                throw new IndexOutOfRangeException("Members exceed collection capacity");

            base.AddMember(type);
        }

        /// <summary>
        /// Returns a <see cref="System.Object"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Object"/> that represents this instance.
        /// </returns>
        internal override object ToObject()
        {
            return new Tuple<object>(Members.Select(
                member => member.ToObject()).Where(member => member != null).ToList());
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A memberwise clone of this instance.</returns>
        internal override SType Clone()
        {
            return (STupleType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("<STupleType [{0}]>", m_length);
        }

        #endregion
    }
}
