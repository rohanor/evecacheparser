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
            //args = new[] { "ascii" };
            //CachedFilesFinder.SetIncludeMethodsFilter("GetOrders", "GetOldPriceHistory", "GetNewPriceHistory");
            //CachedFilesFinder.SetIncludeMethodsFilter("GetSolarSystem");
            //CachedFilesFinder.SetExcludeMethodsFilter("GetBookmarks");

            //FileInfo cachedFile = CachedFilesFinder.GetMachoNetCachedFiles().First();
            foreach (FileInfo cachedFile in CachedFilesFinder.GetMachoNetCachedFiles()/*.Where(x => x.Name == "1970.cache")*/)
            {
                Console.WriteLine("Processing file: {0}", cachedFile.Name);
                Console.WriteLine("Reading...");
                if (args.Any() && args.First() == "ascii")
                    CachedFileParser.ShowAsASCII(cachedFile);
                else
                {
                    Console.WriteLine("Parsing...");
                    CachedFileParser.Parse(cachedFile);
                    //Console.WriteLine("Dumping...");
                    //SType.DumpTypes(cachedFile.Name);
                    Console.WriteLine("Done...");
                }
            }

            Console.WriteLine("Successfully parsed all files");
            Console.ReadLine();
        }
    }
}
