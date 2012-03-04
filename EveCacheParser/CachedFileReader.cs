using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EveCacheParser.STypes;

namespace EveCacheParser
{
    public class CachedFileReader
    {
        private SType[] m_sharedObj;
        private int[] m_sharedMap;
        private int m_sharePosition;
        private int m_shareSkip;

        internal CachedFileReader(FileInfo filename, bool deploy = true)
        {
            Filename = filename.Name;
            Fullname = filename.FullName;
            using (FileStream stream = filename.OpenRead())
            {
                BinaryReader binaryReader = new BinaryReader(stream);
                Buffer = binaryReader.ReadBytes((int)stream.Length);
            }

            if (deploy)
                SecurityCheck();
        }

        private CachedFileReader(byte[] buffer)
        {
            Buffer = new byte[buffer.Length];
            Array.Copy(buffer, Buffer, buffer.Length);
        }

        internal CachedFileReader(CachedFileReader source, int length)
        {
            Buffer = source.Buffer;
            Filename = source.Filename;
            Fullname = source.Fullname;
            Position = source.Position;

            SecurityCheck();
            EndOfObjectsData = length + source.Position;
        }


        #region Properties

        private string Filename { get; set; }

        internal string Fullname { get; private set; }

        internal byte[] Buffer { get; private set; }

        internal int Position { get; private set; }

        private int EndOfObjectsData { get; set; }

        internal int Length
        {
            get { return Buffer.Length; }
        }

        internal bool AtEnd
        {
            get { return Position >= EndOfObjectsData; }
        }

        #endregion


        #region Static Methods

        public static CachedFileReader Read(FileInfo file)
        {
            return new CachedFileReader(file);
        }

        #endregion


        #region Internal Methods

        internal SType ReadBigInt()
        {
            SType sObject;

            switch (ReadByte())
            {
                case 8:
                    sObject = new SLongType(ReadLong());
                    break;
                case 4:
                    sObject = new SIntType(ReadInt());
                    break;
                case 3:
                    sObject = new SIntType(ReadByte() + (ReadByte() << 16));
                    break;
                case 2:
                    sObject = new SShortType(ReadShort());
                    break;
                default:
                    sObject = new SByteType(ReadByte());
                    break;
            }

            return sObject;
        }

        internal double ReadDouble()
        {
            return BitConverter.ToDouble(ReadBytes(8), 0);
        }

        internal float ReadFloat()
        {
            return BitConverter.ToSingle(ReadBytes(8), 0);
        }

        internal short ReadShort()
        {
            return BitConverter.ToInt16(ReadBytes(2), 0);
        }

        internal long ReadLong()
        {
            return BitConverter.ToInt64(ReadBytes(8), 0);
        }

        internal string ReadString(int length)
        {
            return Encoding.ASCII.GetString(ReadBytes(length));
        }

        internal int ReadInt()
        {
            return BitConverter.ToInt32(ReadBytes(4), 0);
        }

        internal byte ReadByte()
        {
            byte temp = GetByte();
            Seek(1);
            return temp;
        }

        internal int ReadLength()
        {
            CheckSize(1);
            int lenght = ReadByte();

            if (lenght != 255)
                return lenght;

            CheckSize(4);
            return ReadInt();
        }

        internal void AddSharedObj(SType obj)
        {
            if (m_sharedMap == null)
                throw new Exception("Uninitialized shared map");

            if (m_sharedObj == null)
                throw new Exception("Uninitialized shared obj");

            if (m_sharePosition >= m_sharedMap.Length)
                throw new Exception("position out of range");

            int shareid = m_sharedMap[m_sharePosition];

            if (shareid > m_sharedMap.Length)
                throw new Exception("shareid out of range");

            m_sharedObj[m_sharePosition] = obj.Clone();

            m_sharePosition++;
        }

        internal SType GetSharedObj(int id)
        {
            if (m_sharedObj[id] == null)
                throw new Exception("ShareTab: No entry at position " + id);

            return m_sharedObj[id].Clone();
        }

        internal SType GetDBRow(SObjectType header)
        {
            if (header == null)
                throw new Exception("The DBRow header isn't present...");

            if (header.Name != "blue.DBRowDescriptor")
                throw new Exception("Bad descriptor name");

            STupleType fields = header.Members[0].Members[1].Members[0] as STupleType;
            if (fields == null)
                return new SNoneType();

            int len = ReadLength();
            byte[] olddata = ReadBytes(len);
            byte[] newdata = Unpack(olddata);
            SType body = new SDBRowType(newdata);

            CachedFileReader blob = new CachedFileReader(newdata);

            SDictType dict = new SDictType((uint)fields.Members.Count * 2); // size of dict is the ammount of entries
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
                        case 18:
                        case 2: // 16bit int
                            if (step == 3)
                                obj = new SShortType(blob.ReadShort());
                            break;
                        case 3: // 32bit int
                        case 19:
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
                        case 20:// 64bit int
                        case 21:
                        case 64: // timestamp
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
                        case 17:
                            obj = new SByteType(blob.ReadByte());
                            break;
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
            fakerow.AddMember(header);
            fakerow.AddMember(body);
            fakerow.AddMember(dict);
            return fakerow;
        }

        internal void Seek(int offset, SeekOrigin origin = SeekOrigin.Current)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length - offset;
                    break;
                default:
                    throw new IOException("Invalid origin");
            }
        }

        #endregion


        #region Private Methods

        private void SecurityCheck()
        {
            // Move one position
            Seek(1);

            // Get the # of the shared mapped data in stream
            int sharedMapsize = ReadInt();

            // Calculate the size of the shared mapped data in stream
            m_shareSkip = sharedMapsize * sizeof (int);

            // Security check #1
            if (Position + m_shareSkip > Length)
                throw new EndOfStreamException();

            // Store the position to return to
            int positionTemp = Position;

            // Jump to the shared mapped data posistion in stream
            Seek(m_shareSkip, SeekOrigin.End);

            // Store the shared mapped data  
            m_sharedMap = new int[sharedMapsize];
            for (int i = 0; i < sharedMapsize; i++)
            {
                m_sharedMap[i] = ReadInt();
            }

            // Security Check #2
            for (int i = 0; i < sharedMapsize; i++)
            {
                if ((m_sharedMap[i] > sharedMapsize) || (m_sharedMap[i] < 1))
                    throw new IndexOutOfRangeException();
            }

            m_sharedObj = new SType[sharedMapsize];

            // Mark the end of the data
            EndOfObjectsData = Length - m_shareSkip;

            // Return to the stored position
            Seek(positionTemp, SeekOrigin.Begin);
        }

        private byte[] ReadBytes(int length)
        {
            byte[] temp = GetBytes(new byte[length], length);
            Seek(length);
            return temp;
        }

        private byte GetByte()
        {
            return Buffer[Position];
        }

        private byte[] GetBytes(byte[] destination, int count)
        {
            int copyLength = Position + count < Length ? count : Length - Position;
            Array.Copy(Buffer, Position, destination, 0, copyLength);
            return destination;
        }

        private void CheckSize(int length)
        {
            if (Position + length > Length)
                throw new EndOfStreamException();
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

        #endregion
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
