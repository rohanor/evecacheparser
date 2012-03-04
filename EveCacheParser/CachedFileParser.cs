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
            Parse();
        }


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
                StreamType check = (StreamType)m_reader.ReadByte();
                SType stream = new SType(StreamType.StreamStart);

                if (check != StreamType.StreamStart)
                    continue;

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
                case StreamType.Marker:
                    sObject = new SMarkerType((byte)ReadLength());
                    break;
                case StreamType.IdentString:
                    sObject = new SIdentType(m_reader.ReadString(ReadLength()));
                    break;
                case StreamType.Tuple:
                    {
                        int length = ReadLength();
                        sObject = new STupleType((uint)length);
                        Parse(sObject, length);
                        break;
                    }
                case StreamType.List:
                    {
                        int length = ReadLength();
                        sObject = new SListType((uint)length);
                        Parse(sObject, length);
                        break;
                    }
                case StreamType.Dict:
                    {
                        int length = (ReadLength() * 2);
                        sObject = new SDictType((uint)length);
                        Parse(sObject, length);
                        break;
                    }
                case StreamType.ClassObject:
                    sObject = new SObjectType();
                    Parse(sObject, 2);
                    break;
                case StreamType.SharedObj:
                    sObject = m_reader.GetSharedObj(ReadLength());
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
                        SObjectType obj = new SObjectType();
                        sObject = obj;
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
                        break;
                    }
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
                    sObject = GetDBRow();
                    break;

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


        private int ReadLength()
        {
            CheckSize(1);
            int lenght = m_reader.ReadByte();

            if (lenght != 255)
                return lenght;

            CheckSize(4);
            return m_reader.ReadInt();
        }

        private void CheckSize(int i)
        {
            if (m_reader.Position + i > m_reader.Length)
                throw new EndOfStreamException();
        }

        private SType GetDBRow()
        {
            SType nhead = GetObject();

            SObjectType head = nhead as SObjectType;
            if (head == null)
                throw new Exception("The DBRow header isn't present...");

            if (head.Name != "blue.DBRowDescriptor")
                throw new Exception("Bad descriptor name");

            STupleType fields = head.Members[0].Members[1].Members[0] as STupleType;
            if (fields == null)
                return new SNoneType();

            int len = ReadLength();
            byte[] olddata = m_reader.ReadBytes(len);
            byte[] newdata = Unpack(olddata);
            SType body = new SDBRowType(newdata);

            CachedFileReader blob = new CachedFileReader(newdata);

            SDictType dict = new SDictType(999999); // TODO: need dynamic sized dict
            int step = 1;
            while (step < 6)
            {
                foreach (SType field in fields.Members)
                {
                    SType fieldName = field.Members[0];
                    SIntType fieldType = (SIntType)field.Members[1];
                    int fieldTypeInt = fieldType.Value;

                    byte boolcount = 0;
                    bool boolbuf = false;
                    SType obj = null;

                    switch (fieldTypeInt)
                    {
                        case 2: // 16bit int
                            if (step == 3)
                                obj = new SShortType(blob.ReadShort());
                            break;
                        case 3: // 32bit int
                            if (step == 2)
                                obj = new SIntType(blob.ReadInt());
                            break;
                        case 4:
                            obj = new SFloatType(blob.ReadFloat());
                            break;
                        case 5: // double
                            if (step == 1)
                                obj = new SDoubleType(blob.ReadDouble());
                            break;
                        case 6: // currency
                            if (step == 1)
                                obj = new SLongType(blob.ReadLong());
                            break;
                        case 11: // boolean
                            if (step == 5)
                            {
                                if (boolcount == 0)
                                {
                                    boolbuf = Convert.ToBoolean(blob.ReadByte());
                                    boolcount++;
                                }
                                if (boolbuf && boolcount != 0)
                                    obj = new SBooleanType(1);
                                else
                                    obj = new SBooleanType(0);
                            }
                            break;
                        case 16:
                            obj = new SIntType(blob.ReadByte());
                            break;
                        case 17:
                            goto case 16;
                        case 18: // 16bit int
                            goto case 2;
                        case 19: // 32bit int
                            goto case 3;
                        case 20: // 64bit int
                            goto case 6;
                        case 21: // 64bit int
                            goto case 6;
                        case 64: // timestamp
                            goto case 6;
                        case 128: // string types
                        case 129:
                        case 130:
                            obj = new SStringType("I can't parse strings yet - be patient");
                            break;
                        default:
                            throw new Exception("Unhandled ADO type " + fieldTypeInt);
                    }

                    if (obj == null)
                        continue;

                    dict.AddMember(obj);
                    dict.AddMember(fieldName.Clone());
                }

                step++;
            }

            SType fakerow = new STupleType(3);
            fakerow.AddMember(head);
            fakerow.AddMember(body);
            fakerow.AddMember(dict);
            return fakerow;
        }

        private static byte[] Unpack(IList<byte> inputBytes)
        {
            List<byte> buffer = new List<byte>();
            if (inputBytes.Count == 0)
                return new byte[] { };

            int i = 0;
            while (i < inputBytes.Count)
            {
                PackerOpcap opcap = new PackerOpcap(inputBytes[i++]);
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
                        buffer.Add(inputBytes[i++]);
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
                        buffer.Add(inputBytes[i++]);
                }
            }
            return buffer.ToArray();
        }
    }
    public struct PackerOpcap
    {
        public readonly byte Tlen;
        public readonly bool Tzero;
        public readonly byte Blen;
        public readonly bool Bzero;

        public PackerOpcap(byte b)
        {
            Tlen = (byte)((byte)(b << 5) >> 5);
            Tzero = (byte)((byte)(b << 4) >> 7) == 1;
            Blen = (byte)((byte)(b << 1) >> 5);
            Bzero = (byte)(b >> 7) == 1;
        }
    }

}