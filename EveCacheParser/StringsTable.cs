﻿using System.Collections.Generic;
using System.Linq;

namespace EveCacheParser
{
    internal static class StringsTable
    {
        static readonly List<string> s_stringList;
        static StringsTable()
        {
            s_stringList = Properties.Resources.StringsTable.Split(',').ToList();
        }

        internal static string GetStringByID(byte id)
        {
            return s_stringList[id];
        }
    }
}
