namespace EveCacheParser.STypes
{
    public partial class SType
    {
        #region Constructors

        internal SType(StreamType streamType)
        {
            DebugID = s_count++;
            s_nodes.Add(this);

            Members = new STypeCollection();
            StreamType = streamType;
        }

        internal SType(SType source)
        {
            Members = new STypeCollection(source.Members);
            StreamType = source.StreamType;
        }

        #endregion


        #region Properties

        internal STypeCollection Members { get; private set; }

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

        internal override string ToString()
        {
            return string.Format("<SType [{0}]>", StreamType);
        }

        #endregion
    }
}
