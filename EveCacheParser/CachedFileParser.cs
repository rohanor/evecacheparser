using System;
using System.Collections.Generic;
using System.IO;
using EveCacheParser.STypes;

namespace EveCacheParser
{
    public class CachedFileParser
    {
        private readonly StreamType[] m_lenghtTypes = {
                                                          StreamType.StringLong, StreamType.StringRef, StreamType.SubStream, StreamType.Utf8,
                                                          StreamType.BigInt, StreamType.SharedObj, StreamType.Blue,
                                                          StreamType.IdentString
                                                      };


        private readonly CachedFileReader m_reader;
        private static KeyValuePair<Key, CachedObjects> s_result;

        private CachedFileParser(CachedFileReader reader)
        {
            m_reader = reader;
            Streams = new List<SType>();
            Parse();
        }

        private List<SType> Streams{get; set; }


        #region Static Methods

        public static KeyValuePair<Key, CachedObjects> Parse(CachedFileReader cachedFile)
        {
            new CachedFileParser(cachedFile);

            return s_result;
        }

        #endregion


        private void Parse()
        {
            var i = m_reader.ReadByte() + (m_reader.ReadByte() << 16);
            
            //var o = (m_reader.ReadByte() << 16) + m_reader.ReadByte();
            //var p = m_reader.ReadByte() + m_reader.ReadLong();


            while (!m_reader.AtEnd)
            {
                StreamType check = (StreamType)m_reader.ReadByte();
                SType stream = new SType(StreamType.StreamStart);

                if (check != StreamType.StreamStart)
                    continue;

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

            byte type = m_reader.ReadByte();
            StreamType checkType = (StreamType)type;
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
                case StreamType.StringUnicode:
                case StreamType.StringGlobal:
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
                case StreamType.IdentString:
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
                    sObject = m_reader.GetDBRow(GetObject());
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
                ////case StreamType.Marker:
                ////    if (m_reader.ReadByte() != 0x2d)
                ////        throw new Exception("Didn't encounter a double 0x2d where one was expected at " +
                ////                                 (m_reader.Position - 2));
                ////    else if (lastDbRow != null)
                ////        lastDbRow.IsLast = true;
                ////    return null;

                default:
                    break;
                    //throw new Exception("Can't identify type " + String.Format("{0:x2}", (int)type) +
                    //                    " at position " + String.Format("{0:x2}", m_reader.Position) + " and lenght " +
                    //                    m_reader.Length);
            }

            //if (sObject == null)
            //    throw new Exception("sObject can't be null");

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