Shapeshifter 1.3.1
============
Shapeshifter is a serialization library for long-term data storage. 
It offers the ease of storing data in serialized (JSON based) form while helping you handle object graph changes.    
See [Wiki](https://github.com/csnemes/shapeshifter/wiki) and [Documentation](https://github.com/csnemes/shapeshifter/tree/dev/Documentation/Help) for details.

Should you have any question/problem send an email to csaba.nemes@outlook.com or add an issue/request.

Compatibility:
---
  - .NET Framework 4.0+

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

*1.3.0
  - Modified descendant search to include all visited types. This way ForAllDescendant custom serializers can work without explicitly adding search scope during serializer creation. The most visible effect of this change is that enums now will work properly :-)
  - Bugfix Issue#5: snapshot.exe exits with -1 in case of error
  - Snapshot.exe extended with 'view' verb. It can be used to list names and corresponding versions from the current snapshot
  - Pretty formatting snapshot.exe generic type names  
  - Snapshot.exe entended with 'delete' verb which can be used to delete snapshots from the snapshot history file.
  - Snapshot.exe 'add' verb extended with a replace switch. It can be used to replace an existing snapshot in the snapshot history file.
  - Issue#6: InstanceBuilder now works in a deferred mode. Fields can be overwritten before actual instance creation.
  
*1.3.1
  - snapshot.exe compares existing snapshots in reverse order
  - fixed issue of detecting the same deserializer twice

Notes:
---

