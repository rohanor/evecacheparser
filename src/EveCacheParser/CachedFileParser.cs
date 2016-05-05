/* EVECacheParser - .NET 4/C# EVE Cache File Parser Library
 * Copyright © 2012 Jimi 'Desmont McCallock' C <jimikar@gmail.com>
 *
 * Based on:
 * - reverence - Python library for processing EVE Online cache and bulkdata
 *    Copyright © 2003-2011 Jamie 'Entity' van den Berge <jamie@hlekkir.com>
 *    https://github.com/ntt/reverence
 *
 * - libevecache - C++ EVE online reverse engineered cache reading library
 *    Copyright © 2009-2010  StackFoundry LLC and Yann 'Kaladr' Ramin <yann@stackfoundry.com>
 *    http://dev.eve-central.com/libevecache/
 *    http://gitorious.org/libevecache
 *    https://github.com/theatrus/libevecache
 *
 * - EveCache.Net - A port of libevecache to C#
 *    Copyright © 2011 Jason 'Jay Wareth' Watkins <jason@blacksunsystems.net>
 *    https://github.com/jwatkins42/EveCache.Net
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using EveCacheParser.Collections;
using EveCacheParser.Enumerations;
using EveCacheParser.Exceptions;
using EveCacheParser.STypes;

namespace EveCacheParser
{
    class CachedFileParser
    {
        #region Fields

        readonly CachedFileReader m_reader;
        readonly SStreamType m_stream;
        static bool s_dumpStructure;
        static bool s_cachedObject;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedFileParser"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        CachedFileParser(CachedFileReader reader)
        {
            m_reader = reader;
            m_stream = new SStreamType(StreamType.StreamStart);
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Dumps the structure of the file to a file.
        /// </summary>
        /// <param name="file">The file.</param>
        internal static void DumpStructure(FileInfo file)
        {
            Console.WriteLine("Dumping Structure...");

            s_dumpStructure = true;
            Parse(file);
            SType.DumpTypes(file.Name);
            s_dumpStructure = false;
        }


        /// <summary>
        /// Reads the specified file and shows it in an ASCII format.
        /// </summary>
        /// <param name="file">The file.</param>
        internal static void ShowAsAscii(FileInfo file)
        {
            Console.WriteLine("Converting to ASCII...");

            var cachedFile = new CachedFileReader(file);

            // Dump 16 bytes per line
            var len = cachedFile.Length;
            for (var i = 0; i < len; i += 16)
            {
                var cnt = Math.Min(16, len - i);
                var line = new byte[cnt];
                Array.Copy(cachedFile.Buffer, i, line, 0, cnt);

                // Write address + hex + ascii
                Console.Write("{0:X6} ", i);
                Console.Write(BitConverter.ToString(line));
                Console.Write(" ");

                // Convert non-ascii characters to "."
                for (var j = 0; j < cnt; ++j)
                {
                    if (line[j] < 0x20 || line[j] > 0x7f)
                        line[j] = (byte)'.';
                }
                Console.WriteLine(Encoding.ASCII.GetString(line));
            }
        }

        /// <summary>
        /// Parses the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        internal static object Parse(byte[] data)
        {
            var reader = new CachedFileReader(data);
            var parser = new CachedFileParser(reader);
            parser.Parse();

            return parser.m_stream.Members.Select(member => member.ToObject()).ToList();
        }

        /// <summary>
        /// Parses the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        internal static KeyValuePair<object, object> Parse(FileInfo file)
        {
            Console.WriteLine("Parsing...");

            SType.Reset();
            var cachedFile = new CachedFileReader(file);
            var parser = new CachedFileParser(cachedFile);
            parser.Parse();

            return !s_dumpStructure ? parser.ToObjects() : new KeyValuePair<object, object>();
        }

        /// <summary>
        /// Converts the stream into objects.
        /// </summary>
        /// <returns></returns>
        KeyValuePair<object, object> ToObjects()
        {
            IList<SType> tupleTwoMembers = m_stream.Members.First().Members;
            var key = tupleTwoMembers.First().ToObject();
            var value = tupleTwoMembers.Count > 2
                               ? tupleTwoMembers.Skip(1).Select(member => member.ToObject()).ToList()
                               : tupleTwoMembers.Last().ToObject();

            return new KeyValuePair<object, object>(key, value);
        }

        /// <summary>
        /// Uncompresses the provided data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="unpackedDataSize">Size of the unpacked data.</param>
        /// <returns>A new array with the uncompressed data.</returns>
        /// <remarks>See http://yannramin.com/2009/12/28/about-rle_unpack-in-libevecache/ </remarks>
        static byte[] Rle_Unpack(IList<byte> data, int unpackedDataSize)
        {
            // Initialize the list with the calculated unpacked size
            var buffer = new List<byte>(unpackedDataSize);

            if (data.Any())
            {
                var i = 0;
                while (i < data.Count)
                {
                    var b = data[i++];
                    var tlen = (byte)((byte)(b << 5) >> 5);
                    var tzero = (byte)((byte)(b << 4) >> 7) == 1;
                    var blen = (byte)((byte)(b << 1) >> 5);
                    var bzero = (byte)(b >> 7) == 1;

                    if (tzero)
                    {
                        var count = (byte)(tlen + 1);
                        for (; count > 0; count--)
                        {
                            buffer.Add(0);
                        }
                    }
                    else
                    {
                        var count = (byte)(8 - tlen);
                        for (; count > 0; count--)
                        {
                            buffer.Add(data[i++]);
                        }
                    }

                    if (bzero)
                    {
                        if (i == data.Count)
                            break;

                        var count = (byte)(blen + 1);
                        for (; count > 0; count--)
                        {
                            buffer.Add(0);
                        }
                    }
                    else
                    {
                        if (i == data.Count)
                            break;

                        var count = (byte)(8 - blen);
                        for (; count > 0; count--)
                        {
                            buffer.Add(data[i++]);
                        }
                    }
                }

                // Ensure that the buffer has enough data
                while (buffer.Count < unpackedDataSize)
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
        void Parse()
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
        void Parse(SType stream, int limit = 1)
        {
            while (!m_reader.AtEnd && limit-- > 0)
            {
                var obj = GetObject();

                // Null object is marker and is handled in base method
                stream.AddMember(obj);

                // Compensate for markers
                if (obj == null)
                    limit++;
            }
        }

        /// <summary>
        /// Gets an object from the data.
        /// </summary>
        /// <returns></returns>
        SType GetObject()
        {
            SType sObject = null;
            var type = m_reader.ReadByte();
            var checkType = (StreamType)(type & ~(byte)StreamType.SharedFlag);
            var shared = Convert.ToBoolean(type & (byte)StreamType.SharedFlag);

            switch (checkType)
            {
                case 0:
                case StreamType.StreamStart:
                    break;
                case StreamType.Marker:
                    break;
                case StreamType.None:
                    sObject = new SNoneType();
                    break;
                case StreamType.Utf8:
                    sObject = new SStringType(m_reader.ReadUtf8String(m_reader.ReadLength()));
                    break;
                case StreamType.String:
                case StreamType.StringLong:
                    sObject = new SStringType(m_reader.ReadString(m_reader.ReadLength()));
                    break;
                case StreamType.StringGlobal:
                    sObject = new SStringType(m_reader.ReadString(m_reader.ReadLength()));
                    CheckShared(shared, sObject);
                    break;
                case StreamType.StringUnicode:
                    sObject = new SStringType(m_reader.ReadUnicodeString(m_reader.ReadLength() * 2));
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
                    var id = m_reader.ReadByte();
                    sObject = new SReferenceType(id, StringsTable.GetStringByID(id));
                    break;
                case StreamType.StringIdent:
                    sObject = new SIdentType(m_reader.ReadString(m_reader.ReadLength()));
                    CheckShared(shared, sObject);
                    break;
                case StreamType.Tuple:
                    {
                        var length = m_reader.ReadLength();
                        sObject = new STupleType((uint)length);
                        CheckShared(shared, sObject);
                        Parse(sObject, length);
                        break;
                    }
                case StreamType.List:
                    {
                        var length = m_reader.ReadLength();
                        sObject = new SListType((uint)length);
                        CheckShared(shared, sObject);
                        Parse(sObject, length);
                        break;
                    }
                case StreamType.Dict:
                    {
                        var length = (m_reader.ReadLength() * 2);
                        sObject = new SDictType((uint)length);
                        CheckShared(shared, sObject);
                        Parse(sObject, length);
                        break;
                    }
                case StreamType.SharedObj:
                    sObject = m_reader.GetSharedObj(m_reader.ReadLength() - 1);
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
                case StreamType.ClassObject:
                case StreamType.NewObj:
                case StreamType.Object:
                    {
                        var storedId = m_reader.ReserveSlot(shared);
                        sObject = ParseObject();
                        m_reader.UpdateSlot(storedId, sObject);
                    }
                    break;
                case StreamType.TupleEmpty:
                    sObject = new STupleType(0);
                    break;
                case StreamType.TupleOne:
                    sObject = new STupleType(1);
                    CheckShared(shared, sObject);
                    Parse(sObject);
                    break;
                case StreamType.ListEmpty:
                    sObject = new SListType(0);
                    CheckShared(shared, sObject);
                    break;
                case StreamType.ListOne:
                    sObject = new SListType(1);
                    CheckShared(shared, sObject);
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
                    CheckShared(shared, sObject);
                    break;
                case StreamType.TupleTwo:
                    sObject = new STupleType(2);
                    CheckShared(shared, sObject);
                    Parse(sObject, 2);
                    break;
                case StreamType.BigInt:
                    sObject = new SLongType(m_reader.ReadBigInt());
                    CheckShared(shared, sObject);
                    break;
                default:
                    throw new ParserException(
                        String.Format(CultureInfo.InvariantCulture,
                                      "Can't identify type {0:x2} at position {1:x2} [{1}] and length {2}",
                                      type, m_reader.Position, m_reader.Length));
            }

            if (sObject == null && type != (byte)StreamType.Marker)
                throw new ParserException("An object could not be created");

            return sObject;
        }

        /// <summary>
        /// Checks if an object is shared.
        /// </summary>
        /// <param name="shared">if set to <c>true</c> it's a shared object.</param>
        /// <param name="sObject">The object.</param>
        void CheckShared(bool shared, SType sObject)
        {
            if (shared)
                m_reader.AddSharedObj(sObject);
        }

        /// <summary>
        /// Parses an object.
        /// </summary>
        /// <returns></returns>
        SType ParseObject()
        {
            s_cachedObject = false;

            SType row;
            var obj = new SObjectType();
            Parse(obj);

            if (obj.IsCachedMethodCallResult || obj.IsCachedObject || obj.IsKeyVal)
                obj.AddMember(GetObject());

            if (obj.IsObjectCachingCachedObject)
            {
                s_cachedObject = true;
                obj.AddMember(new SCachedObjectType(GetObject()));
            }

            if (obj.IsRowList || obj.IsCRowset)
            {
                do
                {
                    row = GetObject();
                    obj.AddMember(row);
                } while (row != null); // Null object is marker

                // Check for sequencial marker
                if (GetObject() != null)
                    throw new ParserException("Marker was expected but not found");
            }

            if (!obj.IsCFilterRowset && !obj.IsRowDict && !obj.IsCIndexedRowset)
                return obj;

            // Check for single marker
            if (GetObject() != null)
                return obj;
            do
            {
                row = GetObject();
                obj.AddMember(row);
            } while (row != null); // Null object is marker

            return obj;
        }

        /// <summary>
        /// Parses a sub stream.
        /// </summary>
        /// <returns></returns>
        SType ParseSubStream()
        {
            SType subStream = new SStreamType(StreamType.SubStream);
            var length = m_reader.ReadLength();

            if (s_dumpStructure)
            {
                var subReader = new CachedFileReader(m_reader, length);
                var subParser = new CachedFileParser(subReader);
                subParser.Parse();
                subStream.AddMember(subParser.m_stream.Clone());
            }

            if (s_cachedObject)
                return new SStringType(m_reader.ReadString(length));

            m_reader.Seek(length);

            return subStream;
        }

        /// <summary>
        /// Parses a database row.
        /// </summary>
        /// <returns></returns>
        SType ParseDBRow()
        {
            var header = GetObject() as SObjectType;

            if (header == null)
                throw new ParserException("DBRow header not found");

            if (!header.IsDBRowDescriptor)
                throw new ParserException("Bad DBRow descriptor name");

            var fields = header.Members.First().Members.First(
                member => member is STupleType).Members.First().Members.Cast<STupleType>().ToList();

            if (!fields.Any())
                return new SNoneType();

            // Check for double marker in stream (usually found in a file with one DBRow)
            var length = m_reader.ReadLength();
            if (m_reader.IsDoubleMarker(length))
                length = m_reader.ReadLength();

            var unpackedDataSize = GetUnpackedDataSize(fields);
            var compressedData = m_reader.ReadBytes(length);
            var uncompressedData = Rle_Unpack(compressedData, unpackedDataSize);

            var reader = new CachedFileReader(uncompressedData, false);

            // Find the maximum number of elements for each field member
            var maxElements = fields.Select(field => field.Members.Count).Concat(new[] { 0 }).Max();

            // The size of SDict must be the ammount of entries stored,
            // multiplied by the max elements of each field member
            var dict = new SDictType((uint)(fields.Count() * maxElements));
            var pass = 1;
            while (pass < 7)
            {
                // The pattern for what data to read on each pass is:
                // 1: 64 bit (Int64, Double)
                // 2: 32 bit (Int32, Single)
                // 3: 16 bit (Int16)
                // 4: 8 bit (Byte)
                // 5: 1 bit (Boolean)
                // 6: strings

                foreach (var field in fields)
                {
                    var fieldName = field.Members.First();
                    var fieldType = (SLongType)field.Members.Last();
                    var dbType = (DbTypes)fieldType.Value;

                    byte boolCount = 0;
                    var boolBuffer = false;
                    SType obj = null;

                    switch (dbType)
                    {
                        case DbTypes.Short:
                        case DbTypes.UShort:
                            if (pass == 3)
                                obj = new SShortType(reader.ReadShort());
                            break;
                        case DbTypes.Int:
                        case DbTypes.UInt:
                            if (pass == 2)
                                obj = new SIntType(reader.ReadInt());
                            break;
                        case DbTypes.Float:
                            if (pass == 2)
                                obj = new SDoubleType(reader.ReadFloat());
                            break;
                        case DbTypes.Double:
                            if (pass == 1)
                                obj = new SDoubleType(reader.ReadDouble());
                            break;
                        case DbTypes.Currency:
                            if (pass == 1)
                                obj = new SDoubleType(reader.ReadLong() / 10000.0);
                            break;
                        case DbTypes.Long:
                        case DbTypes.ULong:
                        case DbTypes.Filetime: // Timestamp
                        case DbTypes.DBTimestamp:
                            if (pass == 1)
                                obj = new SLongType(reader.ReadLong());
                            break;
                        case DbTypes.Bool:
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
                        case DbTypes.Byte:
                        case DbTypes.UByte:
                            if (pass == 4)
                                obj = new SByteType(reader.ReadByte());
                            break;
                        case DbTypes.Bytes: // String types
                        case DbTypes.String:
                        case DbTypes.WideString:
                            if (pass == 6)
                                obj = GetObject();
                            break;
                        default:
                            throw new ParserException("Unhandled db column type: " + dbType);
                    }

                    if (obj == null)
                        continue;

                    dict.AddMember(obj);
                    dict.AddMember(fieldName.Clone());
                }

                pass++;
            }

            var parsedDBRow = new STupleType(2);
            parsedDBRow.AddMember(header);
            parsedDBRow.AddMember(dict);
            return parsedDBRow;
        }

        /// <summary>
        /// Gets the size of the unpacked data.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        static int GetUnpackedDataSize(IEnumerable<STupleType> fields)
        {
            var sizes = new int[5];
            var offset = 0;

            foreach (var dbType in fields.Select(field => (DbTypes)field.Members.Last().Value))
            {
                int index;
                switch (dbType)
                {
                    case DbTypes.Bool:
                        index = 0;
                        break;
                    case DbTypes.Byte:
                    case DbTypes.UByte:
                        index = 1;
                        break;
                    case DbTypes.Short:
                    case DbTypes.UShort:
                        index = 2;
                        break;
                    case DbTypes.Int:
                    case DbTypes.UInt:
                    case DbTypes.Float:
                        index = 3;
                        break;
                    case DbTypes.Currency:
                    case DbTypes.Long:
                    case DbTypes.ULong:
                    case DbTypes.Filetime: // Timestamp
                    case DbTypes.DBTimestamp:
                    case DbTypes.Double:
                        index = 4;
                        break;
                    case DbTypes.Empty:
                        continue;
                    case DbTypes.Bytes: // String types
                    case DbTypes.String:
                    case DbTypes.WideString:
                        continue;
                    default:
                        throw new ParserException("Unhandled DB row type " + dbType);
                }

                if (dbType != DbTypes.Empty)
                    sizes[index]++;
            }

            for (var i = 4; i > 0; i--)
            {
                offset += sizes[i] * (1 << (i - 1));
            }

            offset += sizes[0];

            return offset;
        }

        #endregion

    }
}