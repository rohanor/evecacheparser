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
            //args = new[] { "-ascii" };
            //args = new[] { "-structure" };
            //Parser.SetIncludeMethodsFilter("GetOrders", "GetOldPriceHistory", "GetNewPriceHistory");
            //Parser.SetIncludeMethodsFilter("GetOrders");
            //Parser.SetIncludeMethodsFilter("GetBookmarks");
            //Parser.SetIncludeMethodsFilter("GetMarketGroups");
            //Parser.SetExcludeMethodsFilter("GetMarketGroups");
            //Parser.SetExcludeMethodsFilter("GetBookmarks", "GetSolarSystem");
            //Parser.SetCachedFilesFolders("CachedMethodCalls");
            //Parser.SetCachedFilesFolders("CachedMethodCalls", "CachedObjects");

            //FileInfo cachedFile = Parser.GetMachoNetCachedFiles().First();
            int count = 0;
            KeyValuePair<Tuple<object>, Dictionary<object, object>> result = new KeyValuePair<Tuple<object>, Dictionary<object, object>>();
            IEnumerable<FileInfo> cachedFiles = Parser.GetMachoNetCachedFiles();
            foreach (FileInfo cachedFile in cachedFiles/*.Where(x => x.Name == "1c13.cache")*/)
            {
                try
                {
                    Console.WriteLine("Processing file: {0}", cachedFile.Name);
                    if (args.Any() && args.First() == "-ascii")
                        Parser.ShowAsASCII(cachedFile);
                    else if (args.Any() && args.First() == "-structure")
                        Parser.DumpStructure(cachedFile);
                    else
                    {
                        result = Parser.Parse(cachedFile);
                        CheckResult(result);
                    }

                    count++;
                    Console.WriteLine("Parsing succeeded");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Parsing failed: {0}", ex.Message);
                }
            }


            Console.WriteLine("Successfully parsed {0} of {1} files", count, cachedFiles.Count());
            Console.ReadLine();
        }

        private static void CheckResult(KeyValuePair<Tuple<object>, Dictionary<object, object>> result)
        {
            if (result.Key == null)
            {
                Console.WriteLine("Parsing failed: Yielded no result");
                return;
            }

            string id = ((List<object>)result.Key.Item1)[0] as string ??
                        ((List<object>)((Tuple<object>)((List<object>)result.Key.Item1)[0]).Item1)[0] as string;

            string method = ((List<object>)result.Key.Item1)[1].ToString();
            object value;
            switch (id)
            {
                case "marketProxy":
                    {
                        switch (method)
                        {
                            case "GetMarketGroups":
                            case "GetRegionBest":
                                value = (((Dictionary<object, object>)result.Value["lret"]).ToList())[0];
                                break;
                            case "StartupCheck":
                                value = result.Value["lret"];
                                break;
                            default:
                                value = ((List<object>)result.Value["lret"])[0];
                                break;
                        }
                    }
                    break;
                case "stationSvc":
                    {
                        switch (method)
                        {
                            case "GetStation":
                                value = ((List<object>)result.Value["lret"])[0];
                                break;
                            default:
                                value = ((List<object>)((Tuple<object>)result.Value["lret"]).Item1)[0];
                                break;
                        }
                    }
                    break;
                case "charMgr":
                    {
                        switch (method)
                        {
                            case "GetCloneTypeID":
                            case "GetHomeStation":
                            case "GetImageServerLink":
                                value = result.Value["lret"];
                                break;
                            default:
                                value = ((List<object>)result.Value["lret"])[0];
                                break;
                        }
                    }
                    break;
                case "map":
                case "beyonce":
                case "corporationSvc":
                case "bookmark":
                    value = ((List<object>)((Tuple<object>)result.Value["lret"]).Item1)[0];
                    break;
                case "agentMgr":
                case "corpStationMgr":
                    value = result.Value["lret"];
                    break;
                case "dogma":
                case "facWarMgr":
                    value = (((Dictionary<object, object>)result.Value["lret"]).ToList())[0];
                    break;
                default:
                    value = ((List<object>)result.Value["lret"])[0];
                    break;
            }
        }
    }
}
