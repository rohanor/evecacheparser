using System.Collections.ObjectModel;

namespace EveCacheParser.STypes
{
    public abstract partial class SType
    {
        #region Constructors

        internal SType(StreamType streamType)
        {
            DebugID = s_count++;
            s_nodes.Add(this);

            Members = new Collection<SType>();
            StreamType = streamType;
        }

        #endregion


        #region Properties

        internal Collection<SType> Members { get; private set; }

        internal StreamType StreamType { get; set; }

        #endregion


        #region Methods

        internal virtual void AddMember(SType type)
        {
            Members.Add(type);
        }

        internal virtual SType Clone()
        {
            return (SType)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("<SType [{0}]>", StreamType);
        }

        #endregion
    }
}
