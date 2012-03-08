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
            //args = new[] { "--ascii" };
            //args = new[] { "--structure" };
            //CachedFilesFinder.SetIncludeMethodsFilter("GetOrders", "GetOldPriceHistory", "GetNewPriceHistory");
            CachedFilesFinder.SetIncludeMethodsFilter("GetChannels");
            //CachedFilesFinder.SetIncludeMethodsFilter("GetBookmarks");
            //CachedFilesFinder.SetExcludeMethodsFilter("GetBookmarks", "GetSolarSystem");

            //FileInfo cachedFile = CachedFilesFinder.GetMachoNetCachedFiles().First();
            foreach (FileInfo cachedFile in CachedFilesFinder.GetMachoNetCachedFiles()/*.Where(x => x.Name == "2f79.cache" &&x.Name == "6c3b.cache") */)
            {
                Console.WriteLine("Processing file: {0}", cachedFile.Name);
                if (args.Any() && args.First() == "--ascii")
                    CachedFileParser.ShowAsASCII(cachedFile);
                else if (args.Any() && args.First() == "--structure")
                    CachedFileParser.DumpStructure(cachedFile);
                else
                    CachedFileParser.Parse(cachedFile);

                Console.WriteLine("Done...");
            }

            Console.WriteLine("Successfully parsed all files");
            Console.ReadLine();
        }
    }
}
