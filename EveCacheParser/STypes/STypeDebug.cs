using System;
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

        protected readonly int DebugID;

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

        internal static void DumpNodes(string fileName)
        {
            foreach (SType node in s_nodes)
            {
                s_nodeConsumed[node.DebugID] = false;
            }

            StringBuilder fileContents = new StringBuilder();
            foreach (SType n in s_nodes.Where(n => !s_nodeConsumed[n.DebugID]))
            {
                if (n.StreamType == StreamType.StreamStart)
                {
                    s_nodeConsumed[n.DebugID] = true;
                    continue;
                }

                fileContents.Append(n.ToString());
                fileContents.Append(String.Format("[{0:00}]\n", n.DebugID));
                fileContents.Append(DumpNode(n, 1));

                s_nodeConsumed[n.DebugID] = true;
            }
            File.WriteAllText(Path.ChangeExtension(fileName, ".structure"), fileContents.ToString());
        }

        private static string DumpNode(SType node, int offset)
        {
            if (node.Members.Length == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("(\n".PadLeft(2 * offset));
            foreach (SType n in node.Members)
            {
                sb.Append(n.ToString().PadLeft((2 * offset) + n.ToString().Length));
                sb.Append(String.Format("[{0:00}]\n", n.DebugID));
                if (n.Members.Length > 0)
                    sb.Append(DumpNode(n, offset + 1));
                s_nodeConsumed[n.DebugID] = true;
            }
            sb.Append(")\n".PadLeft(2 * offset));

            return sb.ToString();
        }

        #endregion
    }
}
