using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EveCacheParser.STypes;

namespace EveCacheParser
{
    public class CachedFileParser
    {
        #region Fields

        private readonly CachedFileReader m_reader;
        private readonly SStreamType m_stream;
        private static bool s_dumpStructure;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedFileParser"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private CachedFileParser(CachedFileReader reader)
        {                    
            m_reader = reader;
            m_stream = new SStreamType(StreamType.StreamStart);
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Dumps the structure of the file to a file.
        /// </summary>
        /// <param name="file">The file.</param>
        public static void DumpStructure(FileInfo file)
        {
            Console.WriteLine("Dumping Structure...");

            s_dumpStructure = true;
            SType.Reset();
            Parse(file);
            SType.DumpTypes(file.Name);
        }


        /// <summary>
        /// Reads the specified file and shows it in an ASCII format.
        /// </summary>
        /// <param name="file">The file.</param>
        public static void ShowAsASCII(FileInfo file)
        {
            Console.WriteLine("Converting to ASCII...");

            CachedFileReader cachedFile = new CachedFileReader(file);

            // Dump 16 bytes per line
            int len = cachedFile.Length;
            for (int i = 0; i < len; i += 16)
            {
                int cnt = Math.Min(16, len - i);
                byte[] line = new byte[cnt];
                Array.Copy(cachedFile.Buffer, i, line, 0, cnt);

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

        /// <summary>
        /// Parses the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static void Parse(FileInfo file)
        {
            Console.WriteLine("Parsing...");

            CachedFileReader cachedFile = new CachedFileReader(file);
            CachedFileParser parser = new CachedFileParser(cachedFile);
            parser.Parse();
        }

        /// <summary>
        /// Uncompresses the provided data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="unpackedDataSize">Size of the unpacked data.</param>
        /// <returns>A new array with the uncompressed data.</returns>
        /// <remarks>See http://yannramin.com/2009/12/28/about-rle_unpack-in-libevecache/ </remarks>
        private static byte[] Rle_Unpack(IList<byte> data, int unpackedDataSize)
        {
            // Initialize the list with the calculated unpacked size
            List<byte> buffer = new List<byte>(unpackedDataSize);

            if (data.Any())
            {
                int i = 0;
                while (i < data.Count)
                {
                    byte b = data[i++];
                    byte tlen = (byte)((byte)(b << 5) >> 5);
                    bool tzero = (byte)((byte)(b << 4) >> 7) == 1;
                    byte blen = (byte)((byte)(b << 1) >> 5);
                    bool bzero = (byte)(b >> 7) == 1;

                    if (tzero)
                    {
                        byte count = (byte)(tlen + 1);
                        for (; count > 0; count--)
                        {
                            buffer.Add(0);
                        }
                    }
                    else
                    {
                        byte count = (byte)(8 - tlen);
                        for (; count > 0; count--)
                        {
                            buffer.Add(data[i++]);
                        }
                    }

                    if (bzero)
                    {
                        byte count = (byte)(blen + 1);
                        for (; count > 0; count--)
                        {
                            buffer.Add(0);
                        }
                    }
                    else
                    {
                        if (i == data.Count)
                            break;

                        byte count = (byte)(8 - blen);
                        for (; count > 0; count--)
                        {
                            buffer.Add(data[i++]);
                        }
                    }
                }

                // Ensure that the buffer has enough data
                while (buffer.Count < buffer.Capacity)
                {
                    buffer.Add(0);
                }
            }

            return buffer.ToArray();
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Parses the data of a stream.
        /// </summary>
        private void Parse()
        {
            while (!m_reader.AtEnd)
            {
                Parse(m_stream);
            }
        }

        /// <summary>
        /// Parses the specified stream for the specified limit.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="limit">The limit.</param>
        private void Parse(SType stream, int limit = 1)
        {
            while (!m_reader.AtEnd && limit-- > 0)
            {
                stream.AddMember(GetObject());
            }
        }

        /// <summary>
        /// Gets an object from the data.
        /// </summary>
        /// <returns></returns>
        private SType GetObject()
        {
            SType sObject = null;
            byte type = m_reader.ReadByte();
            StreamType checkType = (StreamType)(type & ~(byte)StreamType.SharedFlag);
            bool shared = Convert.ToBoolean(type & (byte)StreamType.SharedFlag);

            switch (checkType)
            {
                case 0:
                case StreamType.StreamStart:
                    break;
                case StreamType.None:
                    sObject = new SNoneType();
                    break;
                case StreamType.Utf8:
                    sObject = new SStringType(m_reader.ReadUtf8String(m_reader.ReadLength()));
                    break;
                case StreamType.String:
                case StreamType.StringLong:
                    sObject = new SStringType(m_reader.ReadString(m_reader.ReadLength()));
                    break;
                case StreamType.StringGlobal:
                    sObject = new SStringType(m_reader.ReadString(m_reader.ReadLength()));
                    CheckShared(shared, sObject);
                    break;
                case StreamType.StringUnicode:
                    sObject = new SStringType(m_reader.ReadUnicodeString(m_reader.ReadLength() * 2));
                    break;
                case StreamType.Long:
                    sObject = new SLongType(m_reader.ReadLong());
                    break;
                case StreamType.Int:
                    sObject = new SIntType(m_reader.ReadInt());
                    break;
                case StreamType.Short:
                    sObject = new SShortType(m_reader.ReadShort());
                    break;
                case StreamType.Byte:
                    sObject = new SByteType(m_reader.ReadByte());
                    break;
                case StreamType.IntNegOne:
                    sObject = new SIntType(-1);
                    break;
                case StreamType.IntZero:
                    sObject = new SIntType(0);
                    break;
                case StreamType.IntOne:
                    sObject = new SIntType(1);
                    break;
                case StreamType.Double:
                    sObject = new SDoubleType(m_reader.ReadDouble());
                    break;
                case StreamType.DoubleZero:
                    sObject = new SDoubleType(0);
                    break;
                case StreamType.StringEmpty:
                    sObject = new SStringType(null);
                    break;
                case StreamType.StringOne:
                    sObject = new SStringType(m_reader.ReadString(1));
                    break;
                case StreamType.StringRef:
                    sObject = new SReferenceType(m_reader.ReadByte());
                    break;
                case StreamType.StringIdent:
                    sObject = new SIdentType(m_reader.ReadString(m_reader.ReadLength()));
                    CheckShared(shared, sObject);
                    break;
                case StreamType.Tuple:
                    {
                        int length = m_reader.ReadLength();
                        sObject = new STupleType((uint)length);
                        CheckShared(shared, sObject);
                        Parse(sObject, length);
                        break;
                    }
                case StreamType.List:
                    {
                        int length = m_reader.ReadLength();
                        sObject = new SListType((uint)length);
                        CheckShared(shared, sObject);
                        Parse(sObject, length);
                        break;
                    }
                case StreamType.Dict:
                    {
                        int length = (m_reader.ReadLength() * 2);
                        sObject = new SDictType((uint)length);
                        CheckShared(shared, sObject);
                        Parse(sObject, length);
                        break;
                    }
                case StreamType.ClassObject:
                    sObject = new SObjectType();
                    CheckShared(shared, sObject);
                    Parse(sObject, 2);
                    break;
                case StreamType.SharedObj:
                    sObject = m_reader.GetSharedObj(m_reader.ReadLength() - 1);
                    break;
                case StreamType.Checksum:
                    sObject = new SStringType("checksum");
                    m_reader.ReadInt();
                    break;
                case StreamType.BoolTrue:
                    sObject = new SBooleanType(1);
                    break;
                case StreamType.BoolFalse:
                    sObject = new SBooleanType(0);
                    break;
                case StreamType.NewObj:
                case StreamType.Object:
                    {
                        int storedId = m_reader.ReserveSlot(shared);
                        sObject = ParseObject();
                        m_reader.UpdateSlot(storedId, sObject);
                    }
                    break;
                case StreamType.TupleEmpty:
                    sObject = new STupleType(0);
                    break;
                case StreamType.TupleOne:
                    sObject = new STupleType(1);
                    CheckShared(shared, sObject);
                    Parse(sObject);
                    break;
                case StreamType.ListEmpty:
                    sObject = new SListType(0);
                    CheckShared(shared, sObject);
                    break;
                case StreamType.ListOne:
                    sObject = new SListType(1);
                    CheckShared(shared, sObject);
                    Parse(sObject);
                    break;
                case StreamType.StringUnicodeEmpty:
                    sObject = new SStringType(String.Empty);
                    break;
                case StreamType.StringUnicodeOne:
                    sObject = new SStringType(m_reader.ReadString(2));
                    break;
                case StreamType.CompressedDBRow:
                    sObject = ParseDBRow();
                    break;
                case StreamType.SubStream:
                    sObject = ParseSubStream();
                    CheckShared(shared, sObject);
                    break;
                case StreamType.TupleTwo:
                    sObject = new STupleType(2);
                    CheckShared(shared, sObject);
                    Parse(sObject, 2);
                    break;
                case StreamType.BigInt:
                    sObject = new SLongType(m_reader.ReadBigInt());
                    CheckShared(shared, sObject);
                    break;
                case StreamType.Marker:
                    break;
                default:
                    throw new NotImplementedException(
                        String.Format("Can't identify type {0:x2} at position {1:x2} [{1}] and length {2}",
                                      type, m_reader.Position, m_reader.Length));
            }

            if (sObject == null && type != (byte)StreamType.Marker)
                throw new NullReferenceException("An object could not be created");

            return sObject;
        }

        /// <summary>
        /// Checks if an object is shared.
        /// </summary>
        /// <param name="shared">if set to <c>true</c> it's a shared object.</param>
        /// <param name="sObject">The object.</param>
        private void CheckShared(bool shared, SType sObject)
        {
            if (shared)
                m_reader.AddSharedObj(sObject);
        }

        /// <summary>
        /// Parses an object.
        /// </summary>
        /// <returns></returns>
        private SType ParseObject()
        {
            SObjectType obj = new SObjectType();
            Parse(obj);

            if (obj.IsDBRow)
            {
                SType row;
                do
                {
                    row = GetObject();
                    obj.AddMember(row);
                } while (row != null);
            }

            return obj;
        }

        /// <summary>
        /// Parses a sub stream.
        /// </summary>
        /// <returns></returns>
        private SStreamType ParseSubStream()
        {
            SStreamType subStream = new SStreamType(StreamType.SubStream);
            int length = m_reader.ReadLength();

            if (s_dumpStructure)
            {
                CachedFileReader subReader = new CachedFileReader(m_reader, length);
                CachedFileParser subParser = new CachedFileParser(subReader);
                subParser.Parse();
                subStream.AddMember(subParser.m_stream.Clone());
            }

            m_reader.Seek(length);

            return subStream;
        }

        /// <summary>
        /// Parses a database row.
        /// </summary>
        /// <returns></returns>
        private SType ParseDBRow()
        {
            SObjectType header = GetObject() as SObjectType;

            if (header == null)
                throw new NullReferenceException("DBRow header not found");

            if (!header.IsDBRowDescriptor)
                throw new FormatException("Bad DBRow descriptor name");

            STupleType fields = header.Members.First().Members.Last().Members.First() as STupleType;
            if (fields == null)
                return new SNoneType();

            // Check for double marker in stream (usually found in a file with one DBRow)
            int length = m_reader.ReadLength();
            if (m_reader.IsDoubleMarker(length))
                length = m_reader.ReadLength();

            int unpackedDataSize = GetUnpackedDataSize(fields.Members);
            byte[] compressedData = m_reader.ReadBytes(length);
            byte[] uncompressedData = Rle_Unpack(compressedData, unpackedDataSize);

            CachedFileReader reader = new CachedFileReader(uncompressedData);

            // Find the maximum number of elements for each field member
            int maxElements = fields.Members.Select(field => field.Members.Count).Concat(new[] { 0 }).Max();

            // The size of SDict must be the ammount of entries stored,
            // multiplied by the max elements of each field member
            SDictType dict = new SDictType((uint)(fields.Members.Count * maxElements));
            int pass = 1;
            while (pass < 7)
            {
                // The pattern for what data to read on each pass is:
                // 1: 64 bit (Int64, Double)
                // 2: 32 bit (Int32, Single)
                // 3: 16 bit (Int16)
                // 4: 8 bit (Byte)
                // 5: 1 bit (Boolean)
                // 6: strings

                foreach (SType field in fields.Members)
                {
                    SType fieldName = field.Members.First();
                    SLongType fieldType = (SLongType)field.Members.Last();
                    DBTypes dbType = (DBTypes)fieldType.LongValue;

                    byte boolCount = 0;
                    bool boolBuffer = false;
                    SType obj = null;

                    switch (dbType)
                    {
                        case DBTypes.Short:
                        case DBTypes.UShort:
                            if (pass == 3)
                                obj = new SShortType(reader.ReadShort());
                            break;
                        case DBTypes.Int:
                        case DBTypes.UInt:
                            if (pass == 2)
                                obj = new SIntType(reader.ReadInt());
                            break;
                        case DBTypes.Float:
                            if (pass == 2)
                                obj = new SDoubleType(reader.ReadFloat());
                            break;
                        case DBTypes.Double:
                            if (pass == 1)
                                obj = new SDoubleType(reader.ReadDouble());
                            break;
                        case DBTypes.Currency:
                        case DBTypes.Long:
                        case DBTypes.ULong:
                        case DBTypes.Filetime: // Timestamp
                        case DBTypes.DBTimestamp:
                            if (pass == 1)
                                obj = new SLongType(reader.ReadLong());
                            break;
                        case DBTypes.Bool:
                            if (pass == 5)
                            {
                                if (boolCount == 0)
                                {
                                    boolBuffer = Convert.ToBoolean(reader.ReadByte());
                                    boolCount++;
                                }

                                obj = boolBuffer && boolCount != 0
                                          ? new SBooleanType(1)
                                          : new SBooleanType(0);
                            }
                            break;
                        case DBTypes.Byte:
                        case DBTypes.UByte:
                            if (pass == 4)
                                obj = new SByteType(reader.ReadByte());
                            break;
                        case DBTypes.Bytes: // String types
                        case DBTypes.String:
                        case DBTypes.WideString:
                            if (pass == 6)
                                obj = GetObject();
                            break;
                        default:
                            throw new NotImplementedException("Unhandled db column type: " + dbType);
                    }

                    if (obj == null)
                        continue;

                    dict.AddMember(obj);
                    dict.AddMember(fieldName.Clone());
                }

                pass++;
            }

            STupleType parsedDBRow = new STupleType(2);
            parsedDBRow.AddMember(header);
            parsedDBRow.AddMember(dict);
            return parsedDBRow;
        }

        /// <summary>
        /// Gets the size of the unpacked data.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        private static int GetUnpackedDataSize(ICollection<SType> fields)
        {
            int[] sizes = new int[5];
            int offset = 0;

            foreach (DBTypes dbType in fields.Select(field => (DBTypes)field.Members.Last().LongValue))
            {
                int index;
                switch (dbType)
                {
                    case DBTypes.Bool:
                        index = 0;
                        break;
                    case DBTypes.Byte:
                    case DBTypes.UByte:
                        index = 1;
                        break;
                    case DBTypes.Short:
                    case DBTypes.UShort:
                        index = 2;
                        break;
                    case DBTypes.Int:
                    case DBTypes.UInt:
                    case DBTypes.Float:
                        index = 3;
                        break;
                    case DBTypes.Currency:
                    case DBTypes.Long:
                    case DBTypes.ULong:
                    case DBTypes.Filetime: // Timestamp
                    case DBTypes.DBTimestamp:
                    case DBTypes.Double:
                        index = 4;
                        break;
                    case DBTypes.Empty:
                        continue;
                    case DBTypes.Bytes: // String types
                    case DBTypes.String:
                    case DBTypes.WideString:
                        continue;
                    default:
                        throw new NotImplementedException("Unhandled DB row type " + dbType);
                }

                if (dbType != DBTypes.Empty)
                    sizes[index]++;
            }

            for (int i = 4; i > 0; i--)
            {
                offset += sizes[i] * (1 << (i - 1));
            }

            offset += sizes[0];

            return offset;
        }

        #endregion
    }
}