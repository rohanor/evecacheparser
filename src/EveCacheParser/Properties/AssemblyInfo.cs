using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("EVE Cache File Parser Library")]
[assembly: AssemblyDescription("An EVE Online Cache File Parser library. Revision Number: 116 Repository URL: https://bitbucket.org/Desmont_McCallock/evecacheparser")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Jimi C")]
[assembly: AssemblyProduct("EVE Cache File Parser Library")]
[assembly: AssemblyCopyright("Copyright ©  2012-2015, Jimi C")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

#if (DEBUG)
// Friend assembly for unit testing
[assembly: InternalsVisibleTo("EveCacheParser.Tests, PublicKey=" +
    "0024000004800000940000000602000000240000525341310004000001000100c101724b5a25b5" +
    "9520fb72f3a341e332797b3be959d90869f8b1c09f39558befdbc16b5ef2ff321b16208a696b36" +
    "0352e3431895adfaa8ab3948918b94bb56f714144efde24e5df79587fbfeceb6f0348407886d8a" +
    "accd93168ba78a48f52646eef2349b9095add7bc5bef2e34894974f1988ef317a7bb4586541609" +
    "92d9b5d1")]
#endif

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("90d4f36f-7079-4077-ad28-f116bb5c8444")]

// Indicates whether a program element is compliant with the Common Language Specification (CLS). 
[assembly: CLSCompliant(true)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.1.0.116")]

// Neutral Language
[assembly: NeutralResourcesLanguage("en")]