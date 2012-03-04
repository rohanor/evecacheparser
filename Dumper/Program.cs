using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EveCacheParser;
using EveCacheParser.STypes;

namespace Dumper
{
    internal class Program
    {
        private static void Main()
        {
            CachedFilesFinder.SetMethodFilter("GetOrders", "GetOldPriceHistory");

            FileInfo cachedFile = CachedFilesFinder.GetMachoCachedFiles().First();
            CachedFileReader file = CachedFileReader.Read(cachedFile);
            //SType parser = CachedFileParser.Parse(file);
            KeyValuePair<Key, CachedObjects> parsedFile = CachedFileParser.Parse(file);
            SType.DumpTypes(Path.ChangeExtension(cachedFile.Name, ".structure"));
            //DebugASCII.Read(file);

            Console.ReadLine();
        }
    }
}
