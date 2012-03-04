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

        protected internal STypeCollection Members { get; private set; }

        protected StreamType StreamType { get; set; }

        #endregion


        #region Methods

        public virtual void AddMember(SType type)
        {
            Members.Add(type);
        }

        public virtual SType Clone()
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
