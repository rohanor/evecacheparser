using System;
using System.IO;
using System.Linq;
using EveCacheParser.STypes;

namespace EveCacheParser
{
    public class CachedFileParser
    {
        private readonly StreamType[] m_lenghtTypes = {
                                                          StreamType.Tuple, StreamType.Dict, StreamType.List,
                                                          StreamType.StringLong, StreamType.StringRef, StreamType.StringUnicode,
                                                          StreamType.StringGlobal, StreamType.SubStream, StreamType.Utf8,
                                                          StreamType.BigInt, StreamType.SharedObj, StreamType.Blue,
                                                          StreamType.Buffer
                                                      };


        private readonly CachedFileReader m_reader;

        private int m_lenght;
        private static SType s_obj;

        private CachedFileParser(CachedFileReader reader)
        {
            m_reader = reader;
            Parse();
        }


        #region Satic Methods

        public static SType Parse(CachedFileReader cachedFile)
        {
            new CachedFileParser(cachedFile);
            return s_obj;
        }

        #endregion


        private void Parse()
        {
            while (m_reader.AtEnd)
            {
                byte type = m_reader.ReadByte();
                StreamType checkType = (StreamType)type;
                bool shared = Convert.ToBoolean(type & (byte)StreamType.SharedFlag);

                if (shared)
                {
                }

                if (m_lenghtTypes.Contains(checkType))
                    ReadLenght();
                else
                    m_lenght = 0;

                s_obj = GetObjectOf(checkType);
            }
        }

        private SType GetObjectOf(StreamType type)
        {
            SType streamObject = null;
            switch (type)
            {
                case StreamType.StreamStart:
                    break;
                case StreamType.None:
                    streamObject = new SNoneType();
                    break;
                default:
                    throw new Exception("Can't identify type " + String.Format("{0:x2}", (int)type) +
                                        " at position " + String.Format("{0:x2}", m_reader.Position) + " and lenght " +
                                        m_reader.Length);
            }
            return streamObject;
        }

        private void ReadLenght()
        {
            CheckSize(1);
            m_lenght = m_reader.ReadByte();

            if (m_lenght != 255)
                return;

            CheckSize(4);
            m_lenght = m_reader.ReadInt();
        }

        private void CheckSize(int i)
        {
            if (m_reader.Position + i > m_reader.Length)
                throw new EndOfStreamException();
        }
    }
}