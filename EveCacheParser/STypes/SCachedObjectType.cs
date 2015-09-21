# region License
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
# endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using EveCacheParser.Enumerations;

namespace EveCacheParser.STypes
{
    class SCachedObjectType : SType
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SCachedObjectType"/> class.
        /// </summary>
        /// <param name="obj">The object.</param>
        public SCachedObjectType(SType obj)
            : base(StreamType.ClassObject)
        {
            var objects = obj.Members.ToList();

            Version = objects[0].ToObject();
            Object = objects[1].ToObject();
            NodeID = objects[2].ToObject();
            Shared = Convert.ToBoolean(objects[3].ToObject(), CultureInfo.InvariantCulture);
            RawData = objects[4].ToObject();
            IsCompressed = Convert.ToBoolean(objects[5].ToObject(), CultureInfo.InvariantCulture);
            ObjectID = objects[6].ToObject();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is compressed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is compressed; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCompressed { get; set; }

        /// <summary>
        /// Gets or sets the node ID.
        /// </summary>
        /// <value>The node ID.</value>
        object NodeID { get; set; }

        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        /// <value>The object.</value>
        internal object Object { get; set; }

        /// <summary>
        /// Gets or sets the object ID.
        /// </summary>
        /// <value>The object ID.</value>
        object ObjectID { get; set; }

        /// <summary>
        /// Gets or sets the raw data.
        /// </summary>
        /// <value>The raw data.</value>
        internal object RawData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SCachedObjectType"/> is shared.
        /// </summary>
        /// <value><c>true</c> if shared; otherwise, <c>false</c>.</value>
        bool Shared { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        object Version { get; set; }

        #endregion


        #region Methods

        /// <summary>
        /// Decompresses the specified raw data.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <returns></returns>
        internal static byte[] Decompress(ICollection<byte> rawData)
        {
            // The 'rawData' are actually data compressed with zlib ("BEST_SPEED" compression)
            // The following code lines remove the need of 'zlib' usage,
            // because 'zlib' actually uses the same algorith as 'DeflateStream'
            // To make the data compatible for 'DeflateStream', we only have to remove
            // the four last bytes which are the adler32 checksum and
            // the two first bytes which are the zlib header
            var choppedRawData = rawData.Take(rawData.Count - 4).Skip(2).ToArray();

            // Decompress the data
            using (var outStream = GetMemoryStream())
            using (
                var outZStream = new DeflateStream(GetMemoryStream(choppedRawData), CompressionMode.Decompress)
                )
            {
                outZStream.CopyTo(outStream);
                return outStream.ToArray();
            }
        }

        /// <summary>
        /// Gets the memory stream.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns></returns>
        static MemoryStream GetMemoryStream(byte[] buffer = null)
        {
            if (buffer == null || buffer.Length == 0)
                return new MemoryStream();

            return new MemoryStream(buffer);
        }

        /// <summary>
        /// Returns a <see cref="System.Object"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Object"/> that represents this instance.
        /// </returns>
        internal override object ToObject()
        {
            return Clone();
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A memberwise clone of this instance.</returns>
        internal override SType Clone()
        {
            return (SCachedObjectType)MemberwiseClone();
        }

        #endregion
    }
}