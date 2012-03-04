using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EveCacheParser.STypes
{
    public partial class SType
    {
        #region Static Fields

        private static int s_count;
        private static readonly Dictionary<int, bool> s_nodeConsumed;
        private static readonly List<SType> s_nodes;

        #endregion


        #region Fields

        internal readonly int DebugID;

        #endregion Fields


        #region Constructors

        static SType()
        {
            s_count = 0;
            s_nodeConsumed = new Dictionary<int, bool>();
            s_nodes = new List<SType>();
        }

        #endregion


        #region Static Methods

        public static void DumpTypes(string fileName)
        {
            s_nodes.ForEach(node => s_nodeConsumed[node.DebugID] = false);

            StringBuilder fileContents = new StringBuilder();
            foreach (SType n in s_nodes.Where(n => !s_nodeConsumed[n.DebugID]))
            {
                if (n.StreamType == StreamType.StreamStart)
                {
                    s_nodeConsumed[n.DebugID] = true;
                    continue;
                }

                fileContents.Append(n.ToString());
                fileContents.AppendFormat("[{0:00}]\n", n.DebugID);
                fileContents.Append(DumpType(n, 1));

                s_nodeConsumed[n.DebugID] = true;
            }
            File.WriteAllText(Path.ChangeExtension(fileName, ".structure"), fileContents.ToString());
        }

        private  string DumpType(SType node, int offset)
        {
            if (node.Members.Length == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (SType n in node.Members)
            {
                sb.Append(n.ToString().PadLeft((2 * offset) + n.ToString().Length));
                sb.AppendFormat("[{0:00}]\n", n.DebugID);
                if (n.Members.Length > 0)
                    sb.Append(DumpType(n, offset + 1));
                s_nodeConsumed[n.DebugID] = true;
            }

            return sb.ToString();
        }

        #endregion
    }
}
