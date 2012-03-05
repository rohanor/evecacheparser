using System;
using System.Collections.Generic;
using System.Linq;
using EveCacheParser.STypes;

namespace EveCacheParser
{
    public class CachedFileParser
    {
        #region Fields

        private static KeyValuePair<Key, CachedObjects> s_result;
        private readonly CachedFileReader m_reader;
        private readonly List<SType> m_streams;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedFileParser"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private CachedFileParser(CachedFileReader reader)
        {
            m_reader = reader;
            m_streams = new List<SType>();
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Parses the specified cached file.
        /// </summary>
        /// <param name="cachedFile">The cached file.</param>
        /// <returns></returns>
        public static KeyValuePair<Key, CachedObjects> Parse(CachedFileReader cachedFile)
        {
            CachedFileParser parser = new CachedFileParser(cachedFile);
            parser.Parse();
            s_result = new KeyValuePair<Key, CachedObjects>();

            return s_result;
        }

        /// <summary>
        /// Uncompresses the provided data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private static byte[] UncompressData(IList<byte> data)
        {
            List<byte> buffer = new List<byte>();
            if (data.Count == 0)
                return new byte[] { };

            int i = 0;
            while (i < data.Count)
            {
                PackerOpcap opcap = new PackerOpcap(data[i++]);
                if (opcap.Tzero)
                {
                    byte count = (byte)(opcap.Tlen + 1);
                    for (; count > 0; count--)
                        buffer.Add(0);
                }
                else
                {
                    byte count = (byte)(8 - opcap.Tlen);
                    for (; count > 0; count--)
                        buffer.Add(data[i++]);
                }

                if (opcap.Bzero)
                {
                    byte count = (byte)(opcap.Blen + 1);
                    for (; count > 0; count--)
                        buffer.Add(0);
                }
                else
                {
                    byte count = (byte)(8 - opcap.Blen);
                    for (; count > 0; count--)
                        buffer.Add(data[i++]);
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
                SStreamType stream = new SStreamType(StreamType.StreamStart);
                m_streams.Add(stream);
                Parse(stream, 1);
            }
        }

        /// <summary>
        /// Parses the specified stream for the specified limit.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="limit">The limit.</param>
        private void Parse(SType stream, int limit)
        {
            while (!m_reader.AtEnd && limit > 0)
            {
                SType sObject = GetObject();
                if (sObject != null)
                    stream.AddMember(sObject);

                limit--;
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
                    Parse(sObject, 1);
                    break;
                case StreamType.ListEmpty:
                    sObject = new SListType(0);
                    break;
                case StreamType.ListOne:
                    sObject = new SListType(1);
                    Parse(sObject, 1);
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
                    sObject = m_reader.ReadBigInt();
                    break;
                case StreamType.Marker:
                    if (m_reader.ReadByte() != (byte)StreamType.Marker)
                        throw new FormatException("Didn't encounter a double marker (0x2d) where it was expected at " +
                                                  (m_reader.Position - 2));
                    return null;
                default:
                    throw new NotImplementedException(
                        String.Format("Can't identify type {0:x2} at position {1:x2} [{1}] and lenght {2}",
                                      type, m_reader.Position, m_reader.Length));
            }

            if (sObject == null)
                throw new NullReferenceException("sObject caould not be created");

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
            SType sObject = obj;
            Parse(sObject, 1);

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
            return sObject;
        }

        /// <summary>
        /// Parses a sub stream.
        /// </summary>
        /// <returns></returns>
        private SType ParseSubStream()
        {
            CachedFileReader subReader = new CachedFileReader(m_reader, m_reader.ReadLength());
            CachedFileParser subParser = new CachedFileParser(subReader);
            SStreamType subStream = new SStreamType(StreamType.SubStream);
            subParser.Parse();

            foreach (SType type in subParser.m_streams)
            {
                subStream.AddMember(type.Clone());
            }

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

            byte[] compressedData = m_reader.ReadBytes(m_reader.ReadLength());
            byte[] uncompressedData = UncompressData(compressedData);

            CachedFileReader reader = new CachedFileReader(uncompressedData);

            // Find the maximum number of elements for each field member
            int maxElements = fields.Members.Select(field => field.Members.Count).Concat(new[] { 0 }).Max();

            // The size of SDict must be the ammount of entries stored,
            // multiplied by the max elements of each field member
            SDictType dict = new SDictType((uint)(fields.Members.Count * maxElements));
            int step = 1;
            while (step < 6)
            {
                foreach (SType field in fields.Members)
                {
                    SType fieldName = field.Members.First();
                    SLongType fieldType = (SLongType)field.Members.Last();
                    long fieldTypeValue = fieldType.Value;

                    byte boolCount = 0;
                    bool boolBuffer = false;
                    SType obj = null;

                    switch (fieldTypeValue)
                    {
                        case 2: // 8 bit int
                            if (step == 3)
                                obj = new SByteType(reader.ReadByte());
                            break;
                        case 3: // 32 bit int
                        case 19:
                            if (step == 2)
                                obj = new SIntType(reader.ReadInt());
                            break;
                        case 4: // float
                            obj = new SDoubleType(reader.ReadFloat());
                            break;
                        case 5: // double
                            if (step == 1)
                                obj = new SDoubleType(reader.ReadDouble());
                            break;
                        case 6: // currency
                        case 20: // 64 bit int
                        case 21:
                        case 64: // timestamp
                            if (step == 1)
                                obj = new SLongType(reader.ReadLong());
                            break;
                        case 11: // boolean
                            if (step == 5)
                            {
                                if (boolCount == 0)
                                {
                                    boolBuffer = Convert.ToBoolean(reader.ReadByte());
                                    boolCount++;
                                }
                                if (boolBuffer && boolCount != 0)
                                    obj = new SBooleanType(1);
                                else
                                    obj = new SBooleanType(0);
                            }
                            break;
                        case 16:
                        case 17:
                            obj = new SByteType(reader.ReadByte());
                            break;
                        case 18: // 16 bit int
                            obj = new SShortType(reader.ReadShort());
                            break;
                        case 128: // String types
                        case 129:
                        case 130:
                            obj = new SStringType("Can't parse strings yet");
                            break;
                        default:
                            throw new NotImplementedException("Unhandled ADO type " + fieldTypeValue);
                    }

                    if (obj == null)
                        continue;

                    dict.AddMember(obj);
                    dict.AddMember(fieldName.Clone());
                }

                step++;
            }

            STupleType parsedDBRow = new STupleType(2);
            parsedDBRow.AddMember(header);
            parsedDBRow.AddMember(dict);
            return parsedDBRow;
        }

        #endregion
    }
}