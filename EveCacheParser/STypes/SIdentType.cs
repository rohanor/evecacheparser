﻿# region License
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

using System.Globalization;

namespace EveCacheParser.STypes
{
    internal sealed class SIdentType : SStringType
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SIdentType"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        internal SIdentType(string text)
            : base(StreamType.StringIdent)
        {
            Text = text;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A memberwise clone of this instance.</returns>
        internal override SType Clone()
        {
            return (SIdentType)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "<SIdentType '{0}'>", Text);
        }

        #endregion
    }
}
