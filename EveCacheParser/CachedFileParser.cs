using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// Reads the specified file and projects it with an ASCII format.
        /// </summary>
        /// <param name="cachedFile">The cachedFile.</param>
        public static void ShowAsASCII(CachedFileReader cachedFile)
        {
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
        /// Parses the specified cached file.
        /// </summary>
        /// <param name="cachedFile">The cached file.</param>
        /// <returns></returns>
        public static Tuple<Collection<SType>, Collection<SType>> Parse(CachedFileReader cachedFile)
        {
            CachedFileParser parser = new CachedFileParser(cachedFile);
            parser.Parse();

            Collection<SType> key = parser.m_stream.Members[0].Members[0].Members;
            Collection<SType> obj = parser.m_stream.Members[0].Members[1].Members;
            return new Tuple<Collection<SType>, Collection<SType>>(key, obj);
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
            // (usually the size must be at least 64 + 1 bytes)
            List<byte> buffer = new List<byte>(unpackedDataSize);

            if (data.Any())
            {
                int i = 0;
                while (i < data.Count)
                {
                    PackerOpcap opcap = new PackerOpcap(data[i++]);
                    if (opcap.Tzero)
                    {
                        byte count = (byte)(opcap.Tlen + 1);
                        for (; count > 0; count--)
                        {
                            buffer.Add(0);
                        }
                    }
                    else
                    {
                        byte count = (byte)(8 - opcap.Tlen);
                        for (; count > 0; count--)
                        {
                            buffer.Add(data[i++]);
                        }
                    }

                    if (opcap.Bzero)
                    {
                        byte count = (byte)(opcap.Blen + 1);
                        for (; count > 0; count--)
                        {
                            buffer.Add(0);
                        }
                    }
                    else
                    {
                        if (i == data.Count)
                            break;

                        byte count = (byte)(8 - opcap.Blen);
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
                case StreamType.String:
                case StreamType.StringLong:
                case StreamType.StringGlobal:
                case StreamType.StringUnicode:
                    sObject = new SStringType(m_reader.ReadString(m_reader.ReadByte()));
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
                    sObject = new SReferenceType((byte)m_reader.ReadLength());
                    break;
                case StreamType.StringIdent:
                    sObject = new SIdentType(m_reader.ReadString(m_reader.ReadLength()));
                    break;
                case StreamType.Tuple:
                    {
                        int length = m_reader.ReadLength();
                        sObject = new STupleType((uint)length);
                        Parse(sObject, length);
                        break;
                    }
                case StreamType.List:
                    {
                        int length = m_reader.ReadLength();
                        sObject = new SListType((uint)length);
                        Parse(sObject, length);
                        break;
                    }
                case StreamType.Dict:
                    {
                        int length = (m_reader.ReadLength() * 2);
                        sObject = new SDictType((uint)length);
                        Parse(sObject, length);
                        break;
                    }
                case StreamType.ClassObject:
                    sObject = new SObjectType();
                    Parse(sObject, 2);
                    break;
                case StreamType.SharedObj:
                    sObject = m_reader.GetSharedObj(m_reader.ReadLength());
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
                    sObject = ParseObject();
                    break;
                case StreamType.TupleEmpty:
                    sObject = new STupleType(0);
                    break;
                case StreamType.TupleOne:
                    sObject = new STupleType(1);
                    Parse(sObject);
                    break;
                case StreamType.ListEmpty:
                    sObject = new SListType(0);
                    break;
                case StreamType.ListOne:
                    sObject = new SListType(1);
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
                    break;
                case StreamType.TupleTwo:
                    sObject = new STupleType(2);
                    Parse(sObject, 2);
                    break;
                case StreamType.BigInt:
                    sObject = new SLongType(m_reader.ReadBigInt());
                    break;
                case StreamType.Marker:
                    if (m_reader.ReadByte() != (byte)StreamType.Marker)
                        throw new FormatException("Didn't encounter a double marker (0x2d) where it was expected at position " +
                                                  (m_reader.Position - 2));
                    return null;
                default:
                    throw new NotImplementedException(
                        String.Format("Can't identify type {0:x2} at position {1:x2} [{1}] and lenght {2}",
                                      type, m_reader.Position, m_reader.Length));
            }

            if (sObject == null)
                throw new NullReferenceException("An object could not be created");

            if (shared)
                m_reader.AddSharedObj(sObject);


            return sObject;
        }

        /// <summary>
        /// Parses an object.
        /// </summary>
        /// <returns></returns>
        private SType ParseObject()
        {
            SObjectType obj = new SObjectType();
            Parse(obj);

            if (obj.IsValidRowListName)
            {
                SType row;
                do
                {
                    row = GetObject();
                    if (row != null)
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
            CachedFileReader subReader = new CachedFileReader(m_reader, m_reader.ReadLength());
            CachedFileParser subParser = new CachedFileParser(subReader);
            SStreamType subStream = new SStreamType(StreamType.SubStream);
            subParser.Parse();

            subStream.AddMember(subParser.m_stream.Clone());

            m_reader.AdvancePosition(subReader);

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

            if (!header.IsValidDBRowDescriptorName)
                throw new FormatException("Bad DBRow descriptor name");

            STupleType fields = header.Members.First().Members.Last().Members.First() as STupleType;
            if (fields == null)
                return new SNoneType();

            int unpackedDataSize = GetUnpackedDataSize(fields.Members);

            byte[] compressedData = m_reader.ReadBytes(m_reader.ReadLength());
            byte[] uncompressedData = Rle_Unpack(compressedData, unpackedDataSize);

            CachedFileReader reader = new CachedFileReader(uncompressedData);

            // Find the maximum number of elements for each field member
            int maxElements = fields.Members.Select(field => field.Members.Count).Concat(new[] { 0 }).Max();

            // The size of SDict must be the ammount of entries stored,
            // multiplied by the max elements of each field member
            SDictType dict = new SDictType((uint)(fields.Members.Count * maxElements));
            int pass = 1;
            while (pass < 6)
            {
                // The pattern for what data to read on each pass is:
                // 1: 64 bit (Int64, Double)
                // 2: 32 bit (Int32, Single)
                // 3: 16 bit (Int16)
                // 4: 8 bit (Byte)
                // 5: 1 bit (Boolean)

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
                            if (pass == 1)
                                obj = new SStringType("Can't parse strings yet");
                            break;
                        default:
                            throw new NotImplementedException("Unhandled ADO type: " + dbType);
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
                switch (dbType)
                {
                    case DBTypes.Bool:
                        sizes[4] = 0;
                        break;
                    case DBTypes.Byte:
                    case DBTypes.UByte:
                        sizes[3] = 1;
                        break;
                    case DBTypes.Short:
                    case DBTypes.UShort:
                        sizes[2] = 2;
                        break;
                    case DBTypes.Int:
                    case DBTypes.UInt:
                    case DBTypes.Float:
                        sizes[1] = 3;
                        break;
                    case DBTypes.Currency:
                    case DBTypes.Long:
                    case DBTypes.ULong:
                    case DBTypes.Filetime: // Timestamp
                    case DBTypes.DBTimestamp:
                    case DBTypes.Double:
                    case DBTypes.Bytes: // String types
                    case DBTypes.String:
                    case DBTypes.WideString:
                        sizes[0] = 4;
                        break;
                    default:
                        throw new NotImplementedException("Unhandled DB row type " + dbType);
                }
            }

            for (int i = 4; i > 0; i--)
            {
                offset += sizes[i] * (1 << (i - 1));
            }

            offset <<= 3;
            int tempOffset = offset;
            offset += sizes[0] + fields.Count;
            offset = (offset + 7) >> 3;
            offset += tempOffset;

            // If less than 32 then adjust towards 64
            if (offset < 0x20)
            {   offset = (offset + 0x3) & ~0x3;
                offset += fields.Count * 0x3;
            }

            return offset;
        }

        #endregion
    }
}