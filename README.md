# VstsBuildQueuer  ![latest ci-build](https://bremnes.visualstudio.com/_apis/public/build/definitions/6558d582-1da3-4ed0-83e0-6375cef1afac/11/badge)
Queue builds for Visual Studio Team Services (or TFS) in defined order

##The legacy
Legacy systems often have multiple solutions that are dependent on each other. This could be common libraries such as util classes, enums, types etc. If this system is large and/or complex enough, then it might have build definitions checking in assemblies and/or other binary files to the source control system. If the consuming solutions/projects uses those classes via for instance source mappings in the build definition, it would be advisable to build those as well. (this would be a tail build and is not covered here)

When branching/merging big source trees with multiple of these solutions, one have to trigger the builds in a specific order. VstsBuildQueuer can help with this.

*Disclaimer: checking in binary files in the source control is obviously not the preferred way since package managers such as Nuget exists. But VstsBuildQueuer could be of help if you happen to be in this position.*

##Automate the build sequence
Instead of queuing builds manually and waiting for results, this tool automates that process. You can specify a build order, and it supports serial and parallell execution. It automatically polls VSTS/TFS and proceeds with the following step when all builds are completed. By default it continues with the next step even though one of the build fails. But you can override this by specyfing your own action. So if you want it to throw an exception or do some manual interception, you can do just that. 

##Example
```cs
IBuildQueuer queue = new BuildQueuer();
await queue
	.InitConfig(
		"MyProject",
		"https://myaccount.visualstudio.com",
		new VssBasicCredential(string.Empty, "3ucqyyxfsadasv4ffjsj6mp3c6abg7kez3xwxdikijgnh21d4kta"), // this is the personal access token key
		Console.WriteLine, // default handler for log messages
		failedBuildDefinitionName => // handling failed builds
		{
			Console.WriteLine($"FAILED BUILD: {failedBuildDefinitionName}, see build log for more information. Fix build manually and press [Enter] if you want to continue with the rest of the builds");
			Console.ReadLine();
			Console.WriteLine($"Continuing past failed build {failedBuildDefinitionName}..");
		})
	.QueueBuilds("CommonUtils") // queues and wait for result before continuing
	.QueueBuilds("SharedClasses", "BusinessLogic") // queue both in parallel, continue when both are completed
	.QueueRemainingBuildDefinitions("", "Integration"); // queue remaining build definitions that haven't been built up to this point and doesn't include "Integration" in their name

Console.WriteLine("BuildQueuer completed");
```
(See examples for more usages)