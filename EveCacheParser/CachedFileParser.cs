using System;
using System.Collections.Generic;
using EveCacheParser.STypes;

namespace EveCacheParser
{
    public class CachedFileParser
    {
        private readonly CachedFileReader m_reader;
        private static KeyValuePair<Key, CachedObjects> s_result;

        private CachedFileParser(CachedFileReader reader)
        {
            m_reader = reader;
            Streams = new List<SType>();
            Parse();
        }

        private List<SType> Streams { get; set; }


        #region Static Methods

        public static KeyValuePair<Key, CachedObjects> Parse(CachedFileReader cachedFile)
        {
            new CachedFileParser(cachedFile);
            return s_result;
        }

        #endregion


        private void Parse()
        {
            while (!m_reader.AtEnd)
            {
                //StreamType check = (StreamType)m_reader.ReadByte();
                SType stream = new SType(StreamType.StreamStart);

                //if (check != StreamType.StreamStart)
                //    continue;

                Streams.Add(stream);
                Parse(stream, 1);
            }
        }

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

        private SType GetObject()
        {
            SType sObject = null;
            SDBRowType lastDbRow = null;

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
                case StreamType.Float:
                    sObject = new SFloatType(m_reader.ReadFloat());
                    break;
                case StreamType.Double:
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
                    sObject = new SStringType(string.Empty);
                    break;
                case StreamType.StringUnicodeOne:
                    sObject = new SStringType(m_reader.ReadString(2));
                    break;
                case StreamType.CompressedDBRow:
                    sObject = m_reader.GetDBRow(GetObject() as SObjectType);
                    lastDbRow = sObject as SDBRowType;
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
                        throw new Exception("Didn't encounter a double marker (0x2d) where it was expected at " +
                                            (m_reader.Position - 2));
                    else if (lastDbRow != null)
                        lastDbRow.IsLast = true;
                    return null;
                default:
                    throw new Exception(string.Format("Can't identify type {0:x2} at position {1:x2} [{1}] and lenght {2}",
                                                          type, m_reader.Position, m_reader.Length));
            }

            if (sObject == null)
                throw new Exception("sObject shouldn't be null");

            if (shared)
                m_reader.AddSharedObj(sObject);


            return sObject;
        }

        private SType ParseObject()
        {
            SObjectType obj = new SObjectType();
            SType sObject = obj;
            Parse(sObject, 1);

            if (obj.Name == "dbutil.RowList")
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

        private SType ParseSubStream()
        {
            int len = m_reader.ReadLength();
            CachedFileReader readerSub = new CachedFileReader(m_reader, len);
            SSubStreamType subStream = new SSubStreamType(len);
            CachedFileParser subParser = new CachedFileParser(readerSub);
            subParser.Parse();
            foreach (SType stype in subParser.Streams)
            {
                subStream.AddMember(stype.Clone());
            }

            //m_reader.Seek(readerSub.Position, SeekOrigin.Begin);
            return subStream;
        }
    }
}