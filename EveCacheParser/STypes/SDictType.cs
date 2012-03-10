using System;

namespace EveCacheParser.STypes
{
    internal sealed class SDictType : SType
    {
        private readonly uint m_length;


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SDictType"/> class.
        /// </summary>
        /// <param name="length">The length.</param>
        internal SDictType(uint length)
            : base(StreamType.Dict)
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
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal override SType Clone()
        {
            return (SDictType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("<SDictType [{0}]>", m_length);
        }

        #endregion Methods
    }
}
