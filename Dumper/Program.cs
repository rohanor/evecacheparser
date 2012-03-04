using System;
using System.IO;
using EveCacheParser;
using EveCacheParser.STypes;

namespace ConsoleApplication
{
    internal class Program
    {
        private static void Main()
        {
            CachedFilesFinder.SetMethodFilter("GetOrders", "GetOldPriceHistory");

            foreach (FileInfo cachedFile in CachedFilesFinder.GetMachoCachedFiles())
            {
                CachedFileReader file = CachedFileReader.Read(cachedFile);
                SType parser = CachedFileParser.Parse(file);

                //    int len = s_reader.Length;
                //    // Dump 16 bytes per line
                //    for (int i = 0; i < len; i += Bytesize)
                //    {
                //        int cnt = Math.Min(16, len - i);
                //        byte[] line = new byte[cnt];
                //        Array.Copy(s_reader.Buffer, i, line, 0, cnt);
                //        // Write address + hex + ascii
                //        Console.Write("{0:X6} ", i);
                //        Console.Write(BitConverter.ToString(line));
                //        Console.Write(" ");
                //        // Convert non-ascii characters to "."
                //        for (int j = 0; j < cnt; ++j)
                //        {
                //            if (line[j] < 0x20 || line[j] > 0x7f)
                //                //line[j] = (byte)'.';
                //                if (j < cnt - 1)
                //                    Console.Write(line[j]);
                //                else
                //                    Console.WriteLine(line[j]);
                //            else if (j < cnt - 1)
                //                Console.Write(Encoding.Default.GetString(new[] { line[j] }));
                //            else
                //                Console.WriteLine(Encoding.Default.GetString(new[] { line[j] }));

                //            if (line[j] == 0x40)
                //            {
                //                byte left;
                //                byte right;
                //                if (j > 0)
                //                    left = line[j - 1];
                //                if (j < cnt - 1)
                //                    right = line[j + 1];
                //            }
                //        }
                //        //Console.WriteLine(Encoding.ASCII.GetString(line));
                //    }
            }
            Console.ReadLine();
        }
    }
}
