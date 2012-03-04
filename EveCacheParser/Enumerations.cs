
namespace EveCacheParser
{
    public enum StreamType
    {
        StreamStart = 0x07e,
        None = 0x01, // Python None type
        StringGlobal = 0x02, // A string identifying usually a type, function or class object
        Long = 0x3, // 64 bit signed value
        Int = 0x04, // 32 bit signed value
        Short = 0x05, // 16 bit signed value
        Byte = 0x6, // 8 bit signed value
        IntNegOne = 0x07, // The value of -1
        IntZero = 0x08, // The value of 0
        IntOne = 0x09, // The value of 1
        Float = 0x0a, // 64 bit signed float
        Double = 0x0b, // 64 bit signed double
        StringLong = 0x0d, // String, longer than 255 characters using normal count
        StringEmpty = 0xe, // String, empty
        StringOne = 0xf, // String, 1 character
        String = 0x10, // String, next byte is 0x00 - 0xff being the count
        StringRef = 0x11, // String, reference to line in columnlookup
        StringUnicode = 0x12, // String unicode, next byte is count
        IdentString = 0x13, // Buffer object, identifier string
        Tuple = 0x014, // Tuple, next byte is count
        List = 0x15, // List, next byte is count
        Dict = 0x16, // Dictionary, next byte is count
        ClassObject = 0x017, // Class object, name of the class follows as string
        Blue = 0x18, // Blue object
        Callback = 0x19, // Callback
        SharedObj = 0x1b, // Shared object reference
        Checksum = 0x1c, // Checksum of rest of stream
        BoolTrue = 0x1f, // Boolean True
        BoolFalse = 0x20, // Boolean False
        Pickler = 0x21, // Standard pickle of undetermined size
        Object = 0x22, // Object
        NewObj = 0x23, // New object
        TupleEmpty = 0x24, // Tuple, empty
        TupleOne = 0x25, // Tuple, single element
        ListEmpty = 0x26, // List, empty
        ListOne = 0x27, // List, single element
        StringUnicodeEmpty = 0x28, // String unicode, empty
        StringUnicodeOne = 0x29, // String unicode, 1 character
        CompressedDBRow = 0x2a, // Database row, a RLEish compressed row
        SubStream = 0x2b, // Embedded stream, substream - length bytes followed by 0x7e
        TupleTwo = 0x2c, // Tuple, two elements
        Marker = 0x02d, // Marker (for the NewObj/Object iterators that follow them)
        Utf8 = 0x2e, // UTF8 string unicode, next byte is buffer size count
        BigInt = 0x2f, // Big int, next byte is count

        SharedFlag = 0x40
    }
}