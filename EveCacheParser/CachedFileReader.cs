using System;
using System.IO;

namespace EveCacheParser
{
    public class CachedFileReader
    {
        private int m_shareSkip;
        private int[] m_sharedObj;

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
            get { return Position < EndOfObjectsData; }
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

        internal int ReadInt()
        {
            byte[] temp = ReadBytes(4);
            return BitConverter.ToInt32(temp, 0);
        }

        internal byte ReadByte()
        {
            byte temp = GetByte();
            Seek(1);
            return temp;
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
            int[] sharedMap = new int[sharedMapsize];
            for (int i = 0; i < sharedMapsize; i++)
            {
                sharedMap[i] = ReadInt();
            }

            // Security Check #2
            for (int i = 0; i < sharedMapsize; i++)
            {
                if ((sharedMap[i] > sharedMapsize) || (sharedMap[i] < 1))
                    throw new IndexOutOfRangeException();
            }

            m_sharedObj = new int[sharedMapsize];

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
