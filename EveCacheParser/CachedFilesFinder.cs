using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EveCacheParser
{
    public static class CachedFilesFinder
    {
        private static List<string> s_methodIncludeFilter = new List<string>();
        private static List<string> s_methodExcludeFilter = new List<string>();

        /// <summary>
        /// Sets the included method filter.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void SetIncludeMethodsFilter(params string[] args)
        {
            SetIncludeMethodsFilter(new List<string>(args));
        }

        /// <summary>
        /// Sets the included method filter.
        /// </summary>
        /// <param name="methods">The methods.</param>
        public static void SetIncludeMethodsFilter(IEnumerable<string> methods)
        {
            if (methods == null)
            {
                s_methodIncludeFilter = new List<string>();
                return;
            }

            s_methodIncludeFilter = methods.Where(x => !String.IsNullOrWhiteSpace(x)).ToList();
        }

        /// <summary>
        /// Sets the excluded method filter.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void SetExcludeMethodsFilter(params string[] args)
        {
            SetExcludeMethodsFilter(new List<string>(args));
        }

        /// <summary>
        /// Sets the excluded method filter.
        /// </summary>
        /// <param name="methods">The methods.</param>
        public static void SetExcludeMethodsFilter(IEnumerable<string> methods)
        {
            if (methods == null)
            {
                s_methodExcludeFilter = new List<string>();
                return;
            }

            s_methodExcludeFilter = methods.Where(x => !String.IsNullOrWhiteSpace(x)).ToList();
        }

        /// <summary>
        /// Gets the bulk data cached files.
        /// </summary>
        /// <param name="folderPath">The folder location.</param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetBulkDataCachedFiles(string folderPath)
        {
            if (String.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return null;

            return new DirectoryInfo(folderPath).GetFiles("*.cache2");
        }

        /// <summary>
        /// Gets the macho net cached files.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetMachoNetCachedFiles()
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
            DirectoryInfo[] foldersIn = directory.GetDirectories("*_tranquility").OrderBy(dir => dir.CreationTimeUtc).ToArray();

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
            return cachedFiles.Select(cachedFile => new CachedFileReader(cachedFile, false)).Where(
                reader => reader.Buffer.First() == (byte)StreamType.StreamStart).Where(
                    cachedFile =>
                    s_methodIncludeFilter.Any()
                        ? s_methodIncludeFilter.Any(method => Encoding.ASCII.GetString(cachedFile.Buffer).Contains(method))
                        : !s_methodExcludeFilter.Any() ||
                          s_methodExcludeFilter.All(method => !Encoding.ASCII.GetString(cachedFile.Buffer).Contains(method))
                ).Select(cachedFile => new FileInfo(cachedFile.Filename)).ToArray();
        }
    }
}
