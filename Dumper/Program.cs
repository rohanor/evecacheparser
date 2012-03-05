using System;
using System.IO;
using System.Linq;
using EveCacheParser;
using EveCacheParser.STypes;

namespace Dumper
{
    internal class Program
    {

        private static void Main(string[] args)
        {
            CachedFilesFinder.SetMethodFilter("GetOrders", "GetOldPriceHistory", "GetNewPriceHistory");

            FileInfo cachedFile = CachedFilesFinder.GetMachoNetCachedFiles().First();
            Console.WriteLine("Processing file: {0}", cachedFile.Name);
            Console.WriteLine("Reading...");
            CachedFileReader file = CachedFileReader.Read(cachedFile);

            if (args.Any() && args.First() == "ascii")
                CachedFileParser.ShowAsASCII(file);
            else
            {
                Console.WriteLine("Parsing...");
                CachedFileParser.Parse(file);
                Console.WriteLine("Dumping...");
                SType.DumpTypes(cachedFile.Name);
                Console.WriteLine("Done...");
            }

            Console.ReadLine();
        }
    }
}
