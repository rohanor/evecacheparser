﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EveCacheParser;

namespace Dumper
{
    internal static class Program
    {
        private static void Main(params string[] args)
        {
            /* Switch to force showing the cache file in ASCII format */
            //args = new[] { "-ascii" };

            /* Switch to force dumping the structure of the cache file */
            //args = new[] { "-structure" };

            /* Example of method usage */
            //Parser.SetCachedFilesFolders("CachedMethodCalls", "CachedObjects");
            //Parser.SetCachedFilesFolders("CachedMethodCalls");
            //Parser.SetCachedFilesFolders("CachedObjects");

            /* Example of method usage */
            //Parser.SetIncludeMethodsFilter("GetOrders", "GetOldPriceHistory", "GetNewPriceHistory");
            //Parser.SetIncludeMethodsFilter("GetOldPriceHistory");
            //Parser.SetIncludeMethodsFilter("GetBookmarks");
            //Parser.SetIncludeMethodsFilter("GetMarketGroups");

            /* Example of method usage */
            //Parser.SetExcludeMethodsFilter("GetBookmarks", "GetSolarSystem");
            //Parser.SetExcludeMethodsFilter("GetMarketGroups");

            /* Example of method usage */
            //FileInfo[] cachedFiles = Parser.GetBulkDataCachedFiles(@"E:\CCP\EVE");

            /* Example of method usage */
            //FileInfo[] cachedFiles = Parser.GetMachoNetCachedFiles();
            //FileInfo cachedFile = Parser.GetMachoNetCachedFiles().First();
            //FileInfo[] cachedFiles = Parser.GetMachoNetCachedFiles(@"E:\CCP\EVE");

            /* Code snippet of testing the parsing of the cached files */
            //int count = 0;
            //foreach (FileInfo cachedFile in cachedFiles/*.Where(x => x.Name == "9d34.cache")*/)
            //{
            //    try
            //    {
            //        Console.WriteLine("Processing file: {0}", cachedFile.Name);
            //        if (args.Any() && args.First() == "-ascii")
            //            Parser.ShowAsAscii(cachedFile);
            //        else if (args.Any() && args.First() == "-structure")
            //            Parser.DumpStructure(cachedFile);
            //        else
            //        {
            //            KeyValuePair<object, object> result = Parser.Parse(cachedFile);
            //            CheckResult(result);
            //        }
            //        count++;
            //        Console.WriteLine("Parsing succeeded");
            //    }
            //    catch (ParserException ex)
            //    {
            //        Console.WriteLine("Parsing failed: {0}", ex.Message);
            //    }
            //    //catch (Exception ex)
            //    //{
            //    //    Console.WriteLine("Parsing failed: {0}", ex.Message);
            //    //}
            //}
            //Console.WriteLine("Successfully parsed {0} of {1} files", count, cachedFiles.Count());
            //Console.ReadLine();

            /* Code snippet of extracting market orders */
            //Parser.SetIncludeMethodsFilter("GetOrders");
            //FileInfo cachedFile = Parser.GetMachoNetCachedFiles().First();
            //KeyValuePair<object, object> result = Parser.Parse(cachedFile);
            //List<object> key = (List<object>)((Tuple<object>)result.Key).Item1;
            //long regionID = (long)key[2];
            //short typeID = (short)key[3];
            //List<object> value = (List<object>)lret;
            //List<MarketOrder> orders = value.Cast<List<object>>().SelectMany(
            //    obj => obj.Cast<Dictionary<object, object>>(), (obj, order) => new MarketOrder(order)).ToList();
            //List<MarketOrder> sellOrders = orders.Where(order => !order.Bid).ToList();
            //List<MarketOrder> buyOrders = orders.Where(order => order.Bid).ToList();

            /* Code snippet of extracting market history */
            //Parser.SetIncludeMethodsFilter("GetOldPriceHistory", "GetNewPriceHistory");
            //FileInfo cachedFile = Parser.GetMachoNetCachedFiles().First();
            //KeyValuePair<object, object> result = Parser.Parse(cachedFile);
            //List<object> key = (List<object>)((Tuple<object>)result.Key).Item1;
            //long regionID = (long)key[2];
            //short typeID = (short)key[3];
            //List<object> value = (List<object>)lret;
            //List<PriceHistoryEntry> priceHistory = value.Cast<Dictionary<object, object>>().Select(
            //    entry => new PriceHistoryEntry(entry)).ToList();
        }

        private static void CheckResult(KeyValuePair<object, object> result)
        {
            if (result.Key == null || result.Value == null)
            {
                Console.WriteLine("Parsing failed: Yielded no result");
                return;
            }

            object value;
            object id = result.Key as string ??
                        ((List<object>)((Tuple<object>)result.Key).Item1).First() as string ??
                        ((List<object>)((Tuple<object>)((List<object>)((Tuple<object>)result.Key).Item1).First()).Item1)
                            .First() as string;

            Dictionary<object, object> resultValue = result.Value as Dictionary<object, object>;
            if (resultValue == null)
            {
                value = ((List<object>)Parser.GetObject(((List<object>)result.Value).First())).First();
                return;
            }

            object lret = resultValue["lret"];
            object method = ((List<object>)((Tuple<object>)result.Key).Item1).Skip(1).First() as string;
            switch ((string)id)
            {
                case "config":
                {
                    switch ((string)method)
                    {
                        case "GetAverageMarketPricesForClient":
                            value = ((Dictionary<object, object>)lret).FirstOrDefault();
                            break;
                        default:
                            value = ((List<object>)lret).FirstOrDefault();
                            break;
                    }
                }
                    break;
                case "marketProxy":
                {
                    switch ((string)method)
                    {
                        case "GetMarketGroups":
                        case "GetRegionBest":
                            value = (((Dictionary<object, object>)lret).ToList()).FirstOrDefault();
                            break;
                        case "StartupCheck":
                            value = lret;
                            break;
                        default:
                            value = ((List<object>)lret).FirstOrDefault();
                            break;
                    }
                }
                    break;
                case "stationSvc":
                {
                    switch ((string)method)
                    {
                        case "GetAllianceSystems":
                        case "GetStation":
                            value = ((List<object>)lret).First();
                            break;
                        default:
                            value = ((List<object>)((Tuple<object>)lret).Item1).FirstOrDefault();
                            break;
                    }
                }
                    break;
                case "charMgr":
                {
                    switch ((string)method)
                    {
                        case "GetCloneTypeID":
                        case "GetHomeStation":
                        case "GetImageServerLink":
                            value = lret;
                            break;
                        default:
                            value = ((List<object>)lret).FirstOrDefault();
                            break;
                    }
                }
                    break;
                case "certificateMgr":
                {
                    switch ((string)method)
                    {
                        case "GetCertificateClasses":
                        case "GetCertificateCategories":
                            value = (((Dictionary<object, object>)lret).ToList()).FirstOrDefault();
                            break;
                        default:
                            value = ((List<object>)lret).FirstOrDefault();
                            break;
                    }
                }
                    break;
                case "map":
                case "beyonce":
                case "corporationSvc":
                case "bookmark":
                    value = ((List<object>)((Tuple<object>)lret).Item1).FirstOrDefault();
                    break;
                case "agentMgr":
                case "corpStationMgr":
                    value = lret;
                    break;
                case "dogma":
                case "rewardMgr":
                case "facWarMgr":
                case "dungeonExplorationMgr":
                {
                    switch ((string)method)
                    {
                        case "GetFacWarSystems":
                            value = ((List<object>)lret).FirstOrDefault();
                            break;
                        default:
                            value = (((Dictionary<object, object>)lret).ToList()).FirstOrDefault();
                            break;
                    }
                }
                    break;
                default:
                    value = ((List<object>)lret).FirstOrDefault();
                    break;
            }
        }
    }
}
