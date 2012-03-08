using System;
using System.IO;
using System.Linq;
using EveCacheParser;

namespace Dumper
{
    internal class Program
    {

        private static void Main(params string[] args)
        {
            //args = new[] { "ascii" };
            //CachedFilesFinder.SetIncludeMethodsFilter("GetOrders", "GetOldPriceHistory", "GetNewPriceHistory");
            CachedFilesFinder.SetIncludeMethodsFilter("GetSolarSystem");
            //CachedFilesFinder.SetIncludeMethodsFilter("GetBookmarks");

            //FileInfo cachedFile = CachedFilesFinder.GetMachoNetCachedFiles().First();
            foreach (FileInfo cachedFile in CachedFilesFinder.GetMachoNetCachedFiles() /*.Where(x => x.Name == "35f1.cache")*/)
            {
                Console.WriteLine("Processing file: {0}", cachedFile.Name);
                if (args.Any() && args.First() == "ascii")
                    CachedFileParser.ShowAsASCII(cachedFile);
                else
                {
                    CachedFileParser.Parse(cachedFile);
                    //CachedFileParser.DumpStructure(cachedFile);
                }

                Console.WriteLine("Done...");
            }

            Console.WriteLine("Successfully parsed all files");
            Console.ReadLine();
        }
    }
}
