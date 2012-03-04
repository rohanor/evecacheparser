using System;
using System.Text;

namespace EveCacheParser
{
    public static class DebugASCII
    {
        public static void Read(CachedFileReader file)
        {
            int len = file.Length;
            // Dump 16 bytes per line
            for (int i = 0; i < len; i += 16)
            {
                int cnt = Math.Min(16, len - i);
                byte[] line = new byte[cnt];
                Array.Copy(file.Buffer, i, line, 0, cnt);
                // Write address + hex + ascii
                Console.Write("{0:X6} ", i);
                Console.Write(BitConverter.ToString(line));
                Console.Write(" ");
                // Convert non-ascii characters to "."
                for (int j = 0; j < cnt; ++j)
                {
                    if (line[j] < 0x20 || line[j] > 0x7f)
                        line[j] = (byte)'.';
                    //    if (j < cnt - 1)
                    //        Console.Write(line[j]);
                    //    else
                    //        Console.WriteLine(line[j]);
                    //else if (j < cnt - 1)
                    //    Console.Write(Encoding.ASCII.GetString(new[] { line[j] }));
                    //else
                    //    Console.WriteLine(Encoding.ASCII.GetString(new[] { line[j] }));

                    //if (line[j] == 0x40)
                    //{
                    //    byte left;
                    //    byte right;
                    //    if (j > 0)
                    //        left = line[j - 1];
                    //    if (j < cnt - 1)
                    //        right = line[j + 1];
                    //}
                }
                Console.WriteLine(Encoding.ASCII.GetString(line));
            }

        }
    }
}
