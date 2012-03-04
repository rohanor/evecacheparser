using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EveCacheParser
{
    public static class CachedFilesFinder
    {
        private static List<string> s_methodFilter = new List<string>();

        public static void SetMethodFilter(params string[] args)
        {
            SetMethodFilter(new List<string>(args));
        }

        public static void SetMethodFilter(IEnumerable<string> methods)
        {
            if (methods == null)
            {
                s_methodFilter = new List<string>();
                return;
            }

            s_methodFilter = methods.Where(x => !String.IsNullOrWhiteSpace(x)).ToList();
        }

        public static IEnumerable<FileInfo> GetBulkDataCachedFiles(string folderLocation)
        {
            if (String.IsNullOrWhiteSpace(folderLocation) || !Directory.Exists(folderLocation))
                return null;

            return new DirectoryInfo(folderLocation).GetFiles("*.cache2");
        }

        public static IEnumerable<FileInfo> GetMachoCachedFiles()
        {
            // Get the local appdata folder
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            // Construct the path to the EVE folder
            string eveApplicationDataDir = Path.Combine(localAppData, "CCP", "EVE");

            // Quit if folder not found
            if (!Directory.Exists(eveApplicationDataDir))
                return new List<FileInfo>();

            // Find all eve clients data folders
            DirectoryInfo directory = new DirectoryInfo(eveApplicationDataDir);
            DirectoryInfo[] foldersIn = directory.GetDirectories("*_tranquility");

            // Get the path to the cache folder of each eve client
            IEnumerable<string> cacheFoldersPath = foldersIn.Select(folder => folder.Name).Select(
                folder => Path.Combine(eveApplicationDataDir, folder, "cache", "MachoNet", "87.237.38.200"));

            // Get the latest cache folder (differs on every client patch version)
            // We take into consideration the edge case where the user has multiple clients but uses only one
            string latestFolder = cacheFoldersPath.Select(x => new DirectoryInfo(x)).Where(x => x.Exists).SelectMany(
                x => x.GetDirectories()).Select(x => int.Parse(x.Name)).Concat(new[] { 0 }).Max().ToString();

            // Construct the final path to the cache folders
            cacheFoldersPath = cacheFoldersPath.Select(x => x).Select(x => Path.Combine(x, latestFolder, "CachedMethodCalls"));

            // Get the cached files we need to scrap
            IEnumerable<FileInfo> cachedFiles = cacheFoldersPath.Select(
                x => new DirectoryInfo(x)).Where(x => x.Exists).SelectMany(x => x.GetFiles("*.cache"));

            // Finds the cached files that are legit EVE files and satisfy the methods search criteria
            return cachedFiles.Select(
                cachedFile => new CachedFileReader(cachedFile, false)).Where(
                    reader => reader.Buffer.First() == (byte)StreamType.StreamStart).SelectMany(
                        cachedFile =>
                        s_methodFilter.Any()
                            ? s_methodFilter.Where(
                                method => Encoding.ASCII.GetString(cachedFile.Buffer).Contains(method)).Select(
                                    x => new FileInfo(cachedFile.Filename))
                            : new[] { new FileInfo(cachedFile.Filename) });
        }
    }
}
