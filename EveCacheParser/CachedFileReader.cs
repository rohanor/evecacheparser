using System;
using System.IO;
using System.Text;
using EveCacheParser.STypes;

namespace EveCacheParser
{
    internal class CachedFileReader
    {
        #region Fields

        private SType[] m_sharedObj;
        private int[] m_sharedMap;
        private int m_sharePosition;
        private int m_shareSkip;
        private readonly byte[] m_subBuffer;

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedFileReader"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="doSecurityCheck">if set to <c>true</c> does a security check.</param>
        internal CachedFileReader(FileInfo file, bool doSecurityCheck = true)
        {
            Filename = file.Name;
            Fullname = file.FullName;
            using (FileStream stream = file.OpenRead())
            {
                BinaryReader binaryReader = new BinaryReader(stream);
                Buffer = binaryReader.ReadBytes((int)stream.Length);
            }

            if (doSecurityCheck)
                SecurityCheck();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedFileReader"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        internal CachedFileReader(byte[] buffer)
        {
            Buffer = new byte[buffer.Length];
            Array.Copy(buffer, Buffer, buffer.Length);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedFileReader"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="length">The length.</param>
        internal CachedFileReader(CachedFileReader source, int length)
        {
            m_subBuffer = new byte[length];
            Array.Copy(source.Buffer, source.Position, m_subBuffer, 0, length);

            Buffer = m_subBuffer;
            Filename = source.Filename;
            Fullname = source.Fullname;
            Position = 0;

            SecurityCheck();

            EndOfObjectsData = length - m_shareSkip;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>The filename.</value>
        private string Filename { get; set; }

        /// <summary>
        /// Gets or sets the fullname.
        /// </summary>
        /// <value>The fullname.</value>
        internal string Fullname { get; private set; }

        /// <summary>
        /// Gets or sets the buffer.
        /// </summary>
        /// <value>The buffer.</value>
        internal byte[] Buffer { get; private set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        internal int Position { get; private set; }

        /// <summary>
        /// Gets or sets the end of the objects data.
        /// </summary>
        /// <value>The end of objects data.</value>
        private int EndOfObjectsData { get; set; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        internal int Length
        {
            get { return Buffer.Length; }
        }

        /// <summary>
        /// Gets a value indicating whether we have reach the end of the data.
        /// </summary>
        /// <value><c>true</c> if we have reach the end of the data; otherwise, <c>false</c>.</value>
        internal bool AtEnd
        {
            get { return Position >= EndOfObjectsData; }
        }

        #endregion


        #region Internal Methods

        /// <summary>
        /// Reads a big int.
        /// </summary>
        /// <returns></returns>
        internal long ReadBigInt()
        {
            byte[] bytes = new byte[8];
            int length = ReadLength();
            for (int i = 0; i < length; i++)
            {
                bytes[i] = ReadByte();
            }

            return BitConverter.ToInt64(bytes, 0);
        }

        /// <summary>
        /// Reads a double.
        /// </summary>
        /// <returns></returns>
        internal double ReadDouble()
        {
            return BitConverter.ToDouble(ReadBytes(8), 0);
        }

        /// <summary>
        /// Reads a float.
        /// </summary>
        /// <returns></returns>
        internal float ReadFloat()
        {
            return BitConverter.ToSingle(ReadBytes(4), 0);
        }

        /// <summary>
        /// Reads a short.
        /// </summary>
        /// <returns></returns>
        internal short ReadShort()
        {
            return BitConverter.ToInt16(ReadBytes(2), 0);
        }

        /// <summary>
        /// Reads a long.
        /// </summary>
        /// <returns></returns>
        internal long ReadLong()
        {
            return BitConverter.ToInt64(ReadBytes(8), 0);
        }

        /// <summary>
        /// Reads a long.
        /// </summary>
        /// <returns></returns>
        internal long ReadLong(int length)
        {
            return BitConverter.ToInt64(ReadBytes(length), 0);
        }

        /// <summary>
        /// Reads a string.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        internal string ReadString(int length)
        {
            return Encoding.ASCII.GetString(ReadBytes(length));
        }

        /// <summary>
        /// Reads an int.
        /// </summary>
        /// <returns></returns>
        internal int ReadInt()
        {
            return BitConverter.ToInt32(ReadBytes(4), 0);
        }

        /// <summary>
        /// Reads a byte.
        /// </summary>
        /// <returns></returns>
        internal byte ReadByte()
        {
            byte temp = GetByte();
            Seek(1);
            return temp;
        }

        /// <summary>
        /// Reads the bytes.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        internal byte[] ReadBytes(int length)
        {
            byte[] temp = GetBytes(new byte[length], length);
            Seek(length);
            return temp;
        }

        /// <summary>
        /// Reads the length.
        /// </summary>
        /// <returns></returns>
        internal int ReadLength()
        {
            CheckSize(1);
            int lenght = ReadByte();

            if (lenght != 255)
                return lenght;

            CheckSize(4);
            return ReadInt();
        }

        /// <summary>
        /// Adds a shared object.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void AddSharedObj(SType obj)
        {
            if (m_sharedMap == null)
                throw new NullReferenceException("sharedMap not initialized");

            if (m_sharedObj == null)
                throw new NullReferenceException("sharedObj not initialized");

            if (m_sharedMap.Length == 0)
                return;

            if (m_sharePosition >= m_sharedMap.Length)
                throw new IndexOutOfRangeException("sharePosition out of range");

            int shareid = m_sharedMap[m_sharePosition++] - 1;

            if (shareid >= m_sharedMap.Length)
                throw new IndexOutOfRangeException("shareid out of range");

            m_sharedObj[shareid] = obj.Clone();
        }

        /// <summary>
        /// Gets a shared object.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        internal SType GetSharedObj(int id)
        {
            if (m_sharedObj[id - 1] == null)
                throw new NullReferenceException("No shared object at position " + (id - 1));

            return m_sharedObj[id - 1].Clone();
        }

        /// <summary>
        /// Advances the readers position.
        /// </summary>
        /// <param name="subReader">The sub stream reader.</param>
        internal void AdvancePosition(CachedFileReader subReader)
        {
            Seek(Position + subReader.EndOfObjectsData + subReader.m_shareSkip, SeekOrigin.Begin);
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Does a security check on the data.
        /// </summary>
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

            // Create and store the shared mapped data
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

            // Create the shared objects table
            m_sharedObj = new SType[sharedMapsize];

            // Store the end of the data
            EndOfObjectsData = Length - m_shareSkip;

            // Return to the stored position
            Seek(positionTemp, SeekOrigin.Begin);
        }

        /// <summary>
        /// Seeks the data to the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="origin">The origin.</param>
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

        /// <summary>
        /// Gets a byte.
        /// </summary>
        /// <returns></returns>
        private byte GetByte()
        {
            CheckSize(1);
            return Buffer[Position];
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        private byte[] GetBytes(byte[] destination, int count)
        {
            CheckSize(count);
            int copyLength = Position + count < Length ? count : Length - Position;
            Array.Copy(Buffer, Position, destination, 0, copyLength);
            return destination;
        }

        /// <summary>
        /// Checks the size.
        /// </summary>
        /// <param name="length">The length.</param>
        private void CheckSize(int length)
        {
            if (Position + length > Length)
                throw new EndOfStreamException("Not enough data");
        }


        #endregion

    }
}
