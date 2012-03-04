using System;
using System.IO;
using EveCacheParser;
using EveCacheParser.STypes;

namespace Dumper
{
    internal class Program
    {
        private static void Main()
        {
            CachedFilesFinder.SetMethodFilter("GetOrders", "GetOldPriceHistory");

            foreach (FileInfo cachedFile in CachedFilesFinder.GetMachoCachedFiles())
            {
                CachedFileReader file = CachedFileReader.Read(cachedFile);
                //SType parser = CachedFileParser.Parse(file);
                DebugASCII.Read(file);
            }
            Console.ReadLine();
        }
    }
}
