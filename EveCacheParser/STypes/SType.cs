using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EveCacheParser.STypes
{
    internal abstract class SType
    {
        #region Fields

        protected readonly int DebugID;

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
        protected SType(StreamType streamType)
        {
            DebugID = s_count++;
            s_type.Add(this);

            Members = new List<SType>();
            m_streamType = streamType;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        internal string Text { get; set; }

        /// <summary>
        /// Gets or sets the integral value.
        /// </summary>
        /// <value>The integral value.</value>
        internal long Value { get; set; }

        /// <summary>
        /// Gets or sets the double value.
        /// </summary>
        /// <value>The double value.</value>
        internal double DoubleValue { get; set; }

        /// <summary>
        /// Gets or sets a boolean.
        /// </summary>
        /// <value>The boolean value.</value>
        internal bool Boolean { get; set; }

        /// <summary>
        /// Gets or sets the members.
        /// </summary>
        /// <value>The members.</value>
        internal List<SType> Members { get; private set; }

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
            s_type.ForEach(type => s_typeConsumed[type.DebugID] = false);

            StringBuilder fileContents = new StringBuilder();
            foreach (SType type in s_type.Where(type => !s_typeConsumed[type.DebugID]))
            {
                if (type.m_streamType == StreamType.StreamStart)
                {
                    s_typeConsumed[type.DebugID] = true;
                    continue;
                }

                fileContents.Append(type.ToString());
                fileContents.AppendFormat("[{0:00}]\n", type.DebugID);
                fileContents.Append(DumpType(type, 1));

                s_typeConsumed[type.DebugID] = true;
            }
            File.WriteAllText(Path.ChangeExtension(fileName, ".structure"), fileContents.ToString());
        }

        /// <summary>
        /// Dumps the structure of the stream types.
        /// </summary>
        /// <param name="sType">The stream type.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        private static string DumpType(SType sType, int offset)
        {
            if (!sType.Members.Any())
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (SType type in sType.Members)
            {
                sb.Append(type.ToString().PadLeft((2 * offset) + type.ToString().Length));
                sb.AppendFormat("[{0:00}]\n", type.DebugID);
                if (type.Members.Any())
                    sb.Append(DumpType(type, offset + 1));
                s_typeConsumed[type.DebugID] = true;
            }

            return sb.ToString();
        }

        internal static Dictionary<object, object> ToDictionary(IList<SType> members, int sortCriteria)
        {
            Dictionary<object, object> dictionary = new Dictionary<object, object>();
            object key = null;
            object value = null;
            foreach (SType member in members)
            {
                // Odd members are keys
                if (members.IndexOf(member) % 2 == sortCriteria)
                    key = member.ToObject();
                else
                    // Even members are values
                    value = member.ToObject();

                // Keep iterating till we have a pair
                if (key == null || value == null)
                    continue;

                // Add to dictionary
                dictionary.Add(key, value);

                // Reset
                key = null;
                value = null;
            }

            return dictionary;

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
        /// Returns a <see cref="System.Object"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Object"/> that represents this instance.
        /// </returns>
        internal abstract object ToObject();

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A memberwise clone of this instance.</returns>
        internal abstract SType Clone();

        #endregion
    }
}
