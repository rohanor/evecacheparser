using System;
using System.Text;

namespace EveCacheParser
{
    public static class DebugASCII
    {
        /// <summary>
        /// Reads the specified file and projects it with an ASCII format.
        /// </summary>
        /// <param name="file">The file.</param>
        public static void Read(CachedFileReader file)
        {
            // Dump 16 bytes per line
            int len = file.Length;
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
                }
                Console.WriteLine(Encoding.ASCII.GetString(line));
            }

        }
    }
}
