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

namespace EveCacheParser.Enumerations
{
    enum DbTypes
    {
        Empty = 0,
        Null = 1,
        Short = 2,
        Int = 3,
        Float = 4,
        Double = 5,
        Currency = 6,
        Date = 7,
        BinaryString = 8,
        IDispatch = 9,
        Error = 10,
        Bool = 11,
        Variant = 12,
        BigInt = 13,
        Decimal = 14,
        Byte = 16,
        UByte = 17,
        UShort = 18,
        UInt = 19,
        Long = 20,
        ULong = 21,
        Filetime = 64,
        Guid = 72,
        Bytes = 128,
        String = 129,
        WideString = 130,
        Numeric = 131,
        UserDefinedType = 132,
        DBDate = 133,
        DBTime = 134,
        DBTimestamp = 135,
        HChapter = 136,
        DBLifetime = 137,
        PropVariant = 138,
        VarNumeric = 139,

        Vector = 0x1000,
        Array = 0x2000,
        ByRef = 0x4000,
        Reserved = 0x8000,
    }
}