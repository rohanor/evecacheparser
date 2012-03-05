using System.Collections.ObjectModel;

namespace EveCacheParser.STypes
{
    public abstract partial class SType
    {
        private readonly StreamType m_streamType;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SType"/> class.
        /// </summary>
        /// <param name="streamType">Type of the stream.</param>
        internal SType(StreamType streamType)
        {
            DebugID = s_count++;
            s_type.Add(this);

            Members = new Collection<SType>();
            m_streamType = streamType;
        }

        #endregion


        #region Properties

        internal Collection<SType> Members { get; private set; }

        #endregion


        #region Methods

        /// <summary>
        /// Adds the member.
        /// </summary>
        /// <param name="type">The type.</param>
        internal virtual void AddMember(SType type)
        {
            Members.Add(type);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal virtual SType Clone()
        {
            return (SType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("<SType [{0}]>", m_streamType);
        }

        #endregion
    }
}
