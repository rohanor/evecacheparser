using System;
using System.Collections.Generic;
using System.Linq;

namespace EveCacheParser
{
    internal static class StringsTable
    {
        private static readonly List<string> s_stringList =
            Properties.Resources.StringsTable.Replace(Environment.NewLine, String.Empty).Split(',').ToList();

        /// <summary>
        /// Gets a string by the specified ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        internal static string GetStringByID(byte id)
        {
            return s_stringList[id];
        }
    }
}
