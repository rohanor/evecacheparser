using System;
using System.Collections.Generic;
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
            //Parser.SetIncludeMethodsFilter("GetSolarSystem");
            //Parser.SetIncludeMethodsFilter("GetMarketGroups");
            //Parser.SetExcludeMethodsFilter("GetSolarSystem");
            //Parser.SetExcludeMethodsFilter("GetBookmarks", "GetSolarSystem");
            //Parser.SetCachedFilesFolders("CachedObjects");
            //Parser.SetCachedFilesFolders("CachedMethodCalls", "CachedObjects");

            //FileInfo cachedFile = Parser.GetMachoNetCachedFiles().First();
            int count = 0;
            IEnumerable<FileInfo> cachedFiles = Parser.GetMachoNetCachedFiles();
            foreach (FileInfo cachedFile in cachedFiles/*.Where(x => x.Name == "67b3.cache")*/)
            {
                try
                {
                    Console.WriteLine("Processing file: {0}", cachedFile.Name);
                    if (args.Any() && args.First() == "--ascii")
                        Parser.ShowAsASCII(cachedFile);
                    else if (args.Any() && args.First() == "--structure")
                        Parser.DumpStructure(cachedFile);
                    else
                        Parser.Parse(cachedFile);

                    count++;
                    Console.WriteLine("Parsing succeeded");
                }
                catch
                {
                    Console.WriteLine("Parsing failed");
                }
            }

            Console.WriteLine("Successfully parsed {0} of {1} files", count, cachedFiles.Count());
            Console.ReadLine();
        }
    }
}
