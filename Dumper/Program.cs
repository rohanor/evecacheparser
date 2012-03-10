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
            //Parser.SetIncludeMethodsFilter("GetOrders", "GetOldPriceHistory", "GetNewPriceHistory");
            //Parser.SetIncludeMethodsFilter("GetBookmarks", "GetSolarSystem");
            //Parser.SetIncludeMethodsFilter("GetMarketGroups");
            //Parser.SetExcludeMethodsFilter("GetBookmarks");
            //Parser.SetExcludeMethodsFilter("GetBookmarks", "GetSolarSystem");
            //Parser.SetCachedFilesFolders("CachedObjects");
            //Parser.SetCachedFilesFolders("CachedMethodCalls", "CachedObjects");

            //FileInfo cachedFile = CachedFilesFinder.GetMachoNetCachedFiles().First();
            foreach (FileInfo cachedFile in Parser.GetMachoNetCachedFiles()/*.Where(x => x.Name == "67b3.cache")*/)
            {
                Console.WriteLine("Processing file: {0}", cachedFile.Name);
                if (args.Any() && args.First() == "--ascii")
                    Parser.ShowAsASCII(cachedFile);
                else if (args.Any() && args.First() == "--structure")
                    Parser.DumpStructure(cachedFile);
                else
                    Parser.Parse(cachedFile);

                Console.WriteLine("Done...");
            }

            Console.WriteLine("Successfully parsed all files");
            Console.ReadLine();
        }
    }
}
