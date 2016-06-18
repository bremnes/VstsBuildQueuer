# VstsBuildQueuer
Latest CI-build ![latest ci-build](https://bremnes.visualstudio.com/_apis/public/build/definitions/6558d582-1da3-4ed0-83e0-6375cef1afac/11/badge)

Queue builds for Visual Studio Team Services (or TFS) in defined order

##The problem
Legacy systems often have multiple solutions that are dependent on each other. This could be common libraries such as util classes, types etc. If this system is large and/or complex enough, then it possibly has multiple build definitions where some check in assemblies and/or other binary files. If consuming solutions/projects uses those classes via for instance source mappings in the build definition, it would be advisable to build those as well. (this would be a tail build and is not covered here)

When branching/merging big source trees with multiple of these solutions, one have to trigger the builds in a specific order. It's the latter case VstsBuildQueuer can help with.

*Disclaimer: checking in binary files in the source control is obviously not the preferred way now that Nuget exists, but if you are in legacy position this tool could be of help.*

##Solution
By using VstsBuildQueuer one can specify a build order, and it supports serial and parallell execution. It automatically polls VSTS/TFS and proceeds with the following step if all previous builds was successful.

