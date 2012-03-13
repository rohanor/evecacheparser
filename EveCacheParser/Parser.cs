using System;
using System.Collections.Generic;
using System.IO;

namespace EveCacheParser
{
    public static class Parser
    {
        #region CacheFilesFinder Methods

        /// <summary>
        /// Sets the folders to look for cached files.
        /// </summary>
        /// <param name="args">The folders.</param>
        public static void SetCachedFilesFolders(params string[] args)
        {
            CachedFilesFinder.SetCachedFilesFolders(args);
        }

        /// <summary>
        /// Sets the folders to look for cached files.
        /// </summary>
        /// <param name="folders">The folders.</param>
        public static void SetCachedFilesFolders(IEnumerable<string> folders)
        {
            CachedFilesFinder.SetCachedFilesFolders(folders);
        }

        /// <summary>
        /// Sets the methods to includ in filter.
        /// </summary>
        /// <param name="args">The methods.</param>
        public static void SetIncludeMethodsFilter(params string[] args)
        {
            CachedFilesFinder.SetIncludeMethodsFilter(args);
        }

        /// <summary>
        /// Sets the methods to includ in filter.
        /// </summary>
        /// <param name="methods">The methods.</param>
        public static void SetIncludeMethodsFilter(IEnumerable<string> methods)
        {
            CachedFilesFinder.SetIncludeMethodsFilter(methods);
        }

        /// <summary>
        /// Sets the methods to exclude in filter.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void SetExcludeMethodsFilter(params string[] args)
        {
            CachedFilesFinder.SetExcludeMethodsFilter(args);
        }

        /// <summary>
        /// Sets the methods to exclude in filter.
        /// </summary>
        /// <param name="methods">The methods.</param>
        public static void SetExcludeMethodsFilter(IEnumerable<string> methods)
        {
            CachedFilesFinder.SetExcludeMethodsFilter(methods);
        }

        /// <summary>
        /// Gets the bulk data cached files.
        /// </summary>
        /// <param name="folderPath">The folder location.</param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetBulkDataCachedFiles(string folderPath)
        {
            return CachedFilesFinder.GetBulkDataCachedFiles(folderPath);
        }

        /// <summary>
        /// Gets the macho net cached files.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetMachoNetCachedFiles()
        {
            return CachedFilesFinder.GetMachoNetCachedFiles();
        }

        #endregion


        #region CacheFileParser Methods

        /// <summary>
        /// Dumps the structure of the file to a file.
        /// </summary>
        /// <param name="file">The file.</param>
        public static void DumpStructure(FileInfo file)
        {
            CachedFileParser.DumpStructure(file);
        }


        /// <summary>
        /// Reads the specified file and shows it in an ASCII format.
        /// </summary>
        /// <param name="file">The file.</param>
        public static void ShowAsASCII(FileInfo file)
        {
            CachedFileParser.ShowAsASCII(file);
        }

        /// <summary>
        /// Parses the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static KeyValuePair<object, object> Parse(FileInfo file)
        {
            return CachedFileParser.Parse(file);
        }

        #endregion
    }
}
