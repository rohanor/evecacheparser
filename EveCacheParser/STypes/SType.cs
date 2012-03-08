using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace EveCacheParser.STypes
{
    internal abstract class SType
    {
        #region Fields

        internal readonly int DebugID;

        private readonly StreamType m_streamType;

        private static readonly Dictionary<int, bool> s_typeConsumed = new Dictionary<int, bool>();
        private static readonly List<SType> s_type = new List<SType>();
        private static int s_count;

        #endregion


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

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; protected set; }

        /// <summary>
        /// Gets or sets the long value.
        /// </summary>
        /// <value>The long value.</value>
        public long LongValue { get; protected set; }

        /// <summary>
        /// Gets or sets the double value.
        /// </summary>
        /// <value>The double value.</value>
        public double DoubleValue { get; protected set; }

        /// <summary>
        /// Gets or sets a boolean.
        /// </summary>
        /// <value>The boolean value.</value>
        public bool Boolean { get; protected set; }

        /// <summary>
        /// Gets or sets the members.
        /// </summary>
        /// <value>The members.</value>
        internal Collection<SType> Members { get; private set; }

        #endregion


        #region Static Methods

        /// <summary>
        /// Resets this instance.
        /// </summary>
        internal static void Reset()
        {
            s_type.Clear();
            s_typeConsumed.Clear();
            s_count = 0;
        }

        /// <summary>
        /// Dumps the structure of the stream types.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        internal static void DumpTypes(string fileName)
        {
            s_type.ForEach(node => s_typeConsumed[node.DebugID] = false);

            StringBuilder fileContents = new StringBuilder();
            foreach (SType n in s_type.Where(n => !s_typeConsumed[n.DebugID]))
            {
                if (n.m_streamType == StreamType.StreamStart)
                {
                    s_typeConsumed[n.DebugID] = true;
                    continue;
                }

                fileContents.Append(n.ToString());
                fileContents.AppendFormat("[{0:00}]\n", n.DebugID);
                fileContents.Append(DumpType(n, 1));

                s_typeConsumed[n.DebugID] = true;
            }
            File.WriteAllText(Path.ChangeExtension(fileName, ".structure"), fileContents.ToString());
        }

        /// <summary>
        /// Dumps the structure of the stream types.
        /// </summary>
        /// <param name="type">The stream type.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        private static string DumpType(SType type, int offset)
        {
            if (type.Members.Count == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (SType n in type.Members)
            {
                sb.Append(n.ToString().PadLeft((2 * offset) + n.ToString().Length));
                sb.AppendFormat("[{0:00}]\n", n.DebugID);
                if (n.Members.Count > 0)
                    sb.Append(DumpType(n, offset + 1));
                s_typeConsumed[n.DebugID] = true;
            }

            return sb.ToString();
        }

        #endregion


        #region Methods

        /// <summary>
        /// Adds the member.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal virtual void AddMember(SType obj)
        {
            if (obj != null)
                Members.Add(obj);
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
