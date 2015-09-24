using System;
using System.IO;
using Moq;
using Xunit;

namespace EveCacheParser.Tests
{
    public class ParserTest
    {
        static readonly string s_testFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        [Fact]
        public void GetMachoNetCachedFilesReturnsCollectionOfCachedFiles()
        {
            FileInfo[] files = CachedFilesFinder.GetMachoNetCachedFiles(s_testFolder);
            Assert.NotEmpty(files);
        }
    }
}
