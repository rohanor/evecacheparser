namespace EveCacheParser
{
    internal struct PackerOpcap
    {
        public readonly byte Tlen;
        public readonly bool Tzero;
        public readonly byte Blen;
        public readonly bool Bzero;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackerOpcap"/> struct.
        /// </summary>
        /// <param name="b">The b.</param>
        internal PackerOpcap(byte b)
        {
            Tlen = (byte)((byte)(b << 5) >> 5);
            Tzero = (byte)((byte)(b << 4) >> 7) == 1;
            Blen = (byte)((byte)(b << 1) >> 5);
            Bzero = (byte)(b >> 7) == 1;
        }
    }
}