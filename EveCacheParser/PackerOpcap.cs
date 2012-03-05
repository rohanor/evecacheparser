namespace EveCacheParser
{
    internal struct PackerOpcap
    {
        public readonly byte Tlen;
        public readonly bool Tzero;
        public readonly byte Blen;
        public readonly bool Bzero;

        internal PackerOpcap(byte b)
        {
            Tlen = (byte)((byte)(b << 5) >> 5);
            Tzero = (byte)((byte)(b << 4) >> 7) == 1;
            Blen = (byte)((byte)(b << 1) >> 5);
            Bzero = (byte)(b >> 7) == 1;
        }
    }
}