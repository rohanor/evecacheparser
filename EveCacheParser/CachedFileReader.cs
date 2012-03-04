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
            Filename = filename.FullName;
            using (FileStream stream = filename.OpenRead())
            {
                BinaryReader binaryReader = new BinaryReader(stream);
                Buffer = binaryReader.ReadBytes((int)stream.Length);
            }

            if (deploy)
                SecurityCheck();
        }
        
        internal CachedFileReader(byte[] buffer)
        {
            Buffer = new byte[buffer.Length];
            Array.Copy(buffer, Buffer, buffer.Length);
        }


        #region Properties

        internal string Filename { get; set; }

        internal byte[] Buffer { get; private set; }

        internal int Position { get; private set; }

        internal int Length
        {
            get { return Buffer.Length; }
        }

        internal bool AtEnd
        {
            get { return Position >= EndOfObjectsData; }
        }

        internal int EndOfObjectsData
        {
            get { return Length - m_shareSkip; }
        }

        #endregion


        #region Static Methods

        public static CachedFileReader Read(FileInfo file)
        {
            return new CachedFileReader(file);
        }

        #endregion


        #region Internal Methods

        public double ReadDouble()
        {
            return BitConverter.ToDouble(ReadBytes(8), 0);
        }

        public float ReadFloat()
        {
            return BitConverter.ToSingle(ReadBytes(8), 0);
        }

        public short ReadShort()
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

        internal byte[] ReadBytes(int length)
        {
            byte[] temp = GetBytes(new byte[length], length);
            Seek(length);
            return temp;
        }

        #endregion

        internal void AddSharedObj(SType obj)
        {
            if (m_sharedMap == null)
                throw new Exception("Uninitialized shared map");

            if (m_sharedObj == null)
                throw new Exception("Uninitialized shared obj");

            if (m_sharePosition >= m_sharedMap.Length)
                throw new Exception("cursor out of range");

            int shareid = m_sharedMap[m_sharePosition];

            if (shareid > m_sharedMap.Length)
                throw new Exception("shareid out of range");

            m_sharedObj[shareid] = obj.Clone();

            m_sharePosition++;
        }


        internal SType GetSharedObj(int id)
        {
            if (m_sharedObj[id] == null)
                throw new Exception("ShareTab: No entry at position " + id);

            return m_sharedObj[id].Clone();
        }

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

            // Return to the stored position
            Seek(positionTemp, SeekOrigin.Begin);
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

        private void Seek(int offset, SeekOrigin origin = SeekOrigin.Current)
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
    }
}
