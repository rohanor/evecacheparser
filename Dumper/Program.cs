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
            Console.WriteLine("Reading...");
            CachedFileReader file = CachedFileReader.Read(cachedFile);
            //SType parser = CachedFileParser.Parse(file);
            Console.WriteLine("Parsing...");
            KeyValuePair<Key, CachedObjects> parsedFile = CachedFileParser.Parse(file);
            Console.WriteLine("Dumping...");
            SType.DumpTypes(Path.ChangeExtension(cachedFile.Name, ".structure"));
            //DebugASCII.Read(file);
            Console.WriteLine("Done...");
            Console.ReadLine();
        }
    }
}
