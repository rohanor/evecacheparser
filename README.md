EVECacheParser - .NET 4/C# EVE Cache File Parser Library
--
Copyright © 2012 Jimi 'Desmont McCallock' C <jimikar@gmail.com>

EVECacheParser is an EVE Online cache/bulkdata file parser library.

LICENSE
--
EVECacheParser is distributed under GPL v2 
(see license.txt that is included with the distribution).

REQUIREMENTS
--
- Windows (XP or later).
- x86/x64 compatible processor.
- .NET Framework 4 or higher (possible support from Mono, although not tested)
- An EVE Online installation.

Notes:

- A full EVE installation is not required in every case. It is perfectly
  acceptable to have only the bulkdata and cache folders in the EVE root.

- On Windows, the location of the cache folder is automatically detected
  (in Local AppData, or EVE's root when EVE is normally run with /LUA:OFF).

SECURITY WARNING
--
!!! DO NOT DECODE DATA FROM UNTRUSTED SOURCES WITH THIS LIBRARY !!!

Decoding maliciously constructed or erroneous data may compromise 
your system's security and/or stability.

DISCLAIMER
--
This product does not modify in any way any file associated with the 
EVE client or writes files that change the EVE client behavior, 
therefore does not violate CCP's EVE Online EULA.

EVE Online is a registered trademark of CCP hf.

HOW TO USE
--
Visit the [wiki](https://bitbucket.org/Desmont_McCallock/evecacheparser/wiki/Home) for more details.