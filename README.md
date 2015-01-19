Shapeshifter 1.2.0
============
Shapeshifter is a serialization library for long-term data storage. 
It offers the ease of storing data in serialized (JSON based) form while helping you handle object graph changes.    
See [Wiki](https://github.com/csnemes/shapeshifter/wiki) and [Documentation](https://github.com/csnemes/shapeshifter/tree/dev/Documentation/Help) for details.

Should you have any question/problem send an email to csaba.nemes@outlook.com or add an issue/request.

Compatibility:
---
  - .NET Framework 4.0+

Current Release:
---
  - This is the initial production release 

To install:
---
  - using NuGet: Install-Package Shapeshifter
  - build and use the binaries

To build:
---
Use Visual Studio 2013

Version History:
---
*1.0.0 
    Initial release
 
*1.1.0 
  - Added support for non-static custom deserializers (see wiki for details)
  - Added ShapeshifterSerializerFactory class to simplify configuration 
  
*1.2.0 
  - Instance builder can be configured to enable the modification of an instance after it is read from the builder  
  - Non static custom deserializers can participate in a fix-up phase right before returning the deserialized object hierarchy
  - Bugfix: Snapshot did not take into account classes which only contained custom deserializers
  - Bugfix: Snapshot verbose flag did not list the assemblies processed
  - Small refactorings 

Notes:
---

