using System;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
using Xunit;

namespace EveCacheParser.Tests
{
    public class ParserTest
    {
        static readonly string s_testFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        static readonly string s_getOrdersText = "GetOrders";
        static readonly string s_getOldPriceHistoryText = "GetOldPriceHistory";
        static readonly string s_getNewPriceHistoryText = "GetNewPriceHistory";

        [Fact]
        public void GetMachoNetCachedFiles_Returns_Collection_Of_Cached_Files()
        {
            FileInfo[] files = CachedFilesFinder.GetMachoNetCachedFiles(s_testFolder);
            Assert.NotEmpty(files);
        }

        [Fact]
        public void SetIncludeMethodsFilter_Of_GetOrder_Returns_Only_GetOrders_Files()
        {
            CachedFilesFinder.SetIncludeMethodsFilter();
            CachedFilesFinder.SetExcludeMethodsFilter();

            CachedFilesFinder.SetIncludeMethodsFilter(s_getOrdersText);
            FileInfo[] files = CachedFilesFinder.GetMachoNetCachedFiles(s_testFolder);

            foreach (CachedFileReader cfr in files.Select(file => new CachedFileReader(file, doSecurityCheck: false)))
            {
                Assert.True(Encoding.ASCII.GetString(cfr.Buffer).Contains(s_getOrdersText));
            }
        }

        [Fact]
        public void SetIncludeMethodsFilter_Of_GetOldPriceHistory_Returns_Only_GetOldPriceHistory_Files()
        {
            CachedFilesFinder.SetIncludeMethodsFilter();
            CachedFilesFinder.SetExcludeMethodsFilter();

            CachedFilesFinder.SetIncludeMethodsFilter(s_getOldPriceHistoryText);
            FileInfo[] files = CachedFilesFinder.GetMachoNetCachedFiles(s_testFolder);

            foreach (CachedFileReader cfr in files.Select(file => new CachedFileReader(file, doSecurityCheck: false)))
            {
                Assert.True(Encoding.ASCII.GetString(cfr.Buffer).Contains(s_getOldPriceHistoryText));
            }
        }

        [Fact]
        public void SetIncludeMethodsFilter_Of_GetNewPriceHistory_Returns_Only_GetNewPriceHistory_Files()
        {
            CachedFilesFinder.SetIncludeMethodsFilter();
            CachedFilesFinder.SetExcludeMethodsFilter();

            CachedFilesFinder.SetIncludeMethodsFilter(s_getNewPriceHistoryText);
            FileInfo[] files = CachedFilesFinder.GetMachoNetCachedFiles(s_testFolder);

            foreach (CachedFileReader cfr in files.Select(file => new CachedFileReader(file, doSecurityCheck: false)))
            {
                Assert.True(Encoding.ASCII.GetString(cfr.Buffer).Contains(s_getNewPriceHistoryText));
            }
        }

        [Fact]
        public void SetExcludeMethodsFilter_Of_GetOrder_Does_Not_Return_GetOrders_Files()
        {
            CachedFilesFinder.SetIncludeMethodsFilter();
            CachedFilesFinder.SetExcludeMethodsFilter();

            CachedFilesFinder.SetExcludeMethodsFilter(s_getOrdersText);
            FileInfo[] files = CachedFilesFinder.GetMachoNetCachedFiles(s_testFolder);

            foreach (CachedFileReader cfr in files.Select(file => new CachedFileReader(file, doSecurityCheck: false)))
            {
                Assert.False(Encoding.ASCII.GetString(cfr.Buffer).Contains(s_getOrdersText));
            }
        }

        [Fact]
        public void SetExcludeMethodsFilter_Of_GetOldPriceHistory_Does_Not_Return_GetOldPriceHistory_Files()
        {
            CachedFilesFinder.SetIncludeMethodsFilter();
            CachedFilesFinder.SetExcludeMethodsFilter();

            CachedFilesFinder.SetExcludeMethodsFilter(s_getOldPriceHistoryText);
            FileInfo[] files = CachedFilesFinder.GetMachoNetCachedFiles(s_testFolder);

            foreach (CachedFileReader cfr in files.Select(file => new CachedFileReader(file, doSecurityCheck: false)))
            {
                Assert.False(Encoding.ASCII.GetString(cfr.Buffer).Contains(s_getOldPriceHistoryText));
            }
        }

        [Fact]
        public void SetExcludeMethodsFilter_Of_GetNewPriceHistory_Does_Not_Return_GetNewPriceHistory_Files()
        {
            CachedFilesFinder.SetIncludeMethodsFilter();
            CachedFilesFinder.SetExcludeMethodsFilter();

            CachedFilesFinder.SetExcludeMethodsFilter(s_getNewPriceHistoryText);
            FileInfo[] files = CachedFilesFinder.GetMachoNetCachedFiles(s_testFolder);

            foreach (CachedFileReader cfr in files.Select(file => new CachedFileReader(file, doSecurityCheck: false)))
            {
                Assert.False(Encoding.ASCII.GetString(cfr.Buffer).Contains(s_getNewPriceHistoryText));
            }
        }
    }
}
