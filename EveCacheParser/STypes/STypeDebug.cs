using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EveCacheParser.STypes
{
    public abstract partial  class SType
    {
        #region Static Fields

        private static int s_count;
        private static readonly Dictionary<int, bool> s_typeConsumed = new Dictionary<int, bool>();
        private static readonly List<SType> s_type = new List<SType>();

        #endregion


        #region Fields

        internal readonly int DebugID;

        #endregion Fields


        #region Static Methods

        /// <summary>
        /// Dumps the structure of the stream types.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public static void DumpTypes(string fileName)
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
    }
}
