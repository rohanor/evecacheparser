using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace EveCacheParser.STypes
{
    internal class SCachedObjectType : SType
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SCachedObjectType"/> class.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public SCachedObjectType(SType obj)
            : base(StreamType.ClassObject)
        {
            var objects = obj.Members.ToList();

            Version = objects[0].ToObject();
            Object = objects[1].ToObject();
            NodeID = objects[2].ToObject();
            IsCompressed = Convert.ToBoolean(objects[3].ToObject());
            RawData = Encoding.ASCII.GetBytes(objects[4].Text);
            Shared = Convert.ToBoolean(objects[5].ToObject());
            ObjectID = objects[6].ToObject();

            //GetCachedObject();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is compressed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is compressed; otherwise, <c>false</c>.
        /// </value>
        private bool IsCompressed { get; set; }

        /// <summary>
        /// Gets or sets the node ID.
        /// </summary>
        /// <value>The node ID.</value>
        private object NodeID { get; set; }

        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        /// <value>The object.</value>
        private object Object { get; set; }

        /// <summary>
        /// Gets or sets the object ID.
        /// </summary>
        /// <value>The object ID.</value>
        private object ObjectID { get; set; }

        /// <summary>
        /// Gets or sets the raw data.
        /// </summary>
        /// <value>The raw data.</value>
        private object RawData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SCachedObjectType"/> is shared.
        /// </summary>
        /// <value><c>true</c> if shared; otherwise, <c>false</c>.</value>
        private bool Shared { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        private object Version { get; set; }

        #endregion


        #region Methods

        private void GetCachedObject()
        {
            if (!(Object is SNoneType))
                return;

            if (RawData is SNoneType)
                throw new InvalidDataException("No object?!");

            byte[] data = IsCompressed ? Decompress() : (byte[])RawData;

            CachedFileReader reader = new CachedFileReader(data, data.Length);
            CachedFileParser parser = new CachedFileParser(reader);
            parser.Parse();
            Object = parser.Stream.Members.Select(member => member.ToObject()).ToList();

            RawData = new SNoneType().ToObject();
        }

        private byte[] Decompress()
        {
            byte[] compressedData = (byte[])RawData;
            byte[] decompressedData = new byte[4096];
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (DeflateStream outZStream = new DeflateStream(outMemoryStream, CompressionMode.Decompress))
            using (Stream inMemoryStream = new MemoryStream(compressedData))
            {
                byte[] buffer = new byte[4096];
                int len;
                while ((len = inMemoryStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outZStream.Read(buffer, 0, len);
                }
                outZStream.Flush();
                decompressedData = outMemoryStream.ToArray();
            }
            return decompressedData;
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