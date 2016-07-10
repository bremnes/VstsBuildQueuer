# This script runs on Visual Team Services and is adding information regarding branch and commit id to the assemblies 
[regex]$assemblyVersionRegex = '^\[assembly:[ ]?AssemblyVersion\("(.*)"\)'
[regex]$versionRegex = '\"(.*)\"'
$commitId = $env:BUILD_SOURCEVERSION
$branchName = $env:BUILD_SOURCEBRANCHNAME
$revisionInformation = $env:BUILD_BUILDNUMBER.Split('_')[-1]
$versionNumber = "0.0.0.0"

# Extract revision information from build and set variables so it can be used later in build pipeline
# (used for CI, not for releases)
"##vso[task.setvariable variable=RevisionInformation;]$revisionInformation"


$AllVersionFiles = Get-ChildItem "AssemblyInfo.cs" -recurse

foreach ($file in $AllVersionFiles) 
{ 
	Write-Verbose "Checking file $($file.FullName)" -Verbose
    $content = Get-Content $file.FullName | ? { $_ -notmatch "AssemblyInformationalVersion|AssemblyFileVersion" }
    $content -match $assemblyVersionRegex | % {
        if($_ -match $versionRegex ) {
            $version = [version]$matches[1]
            $assemblyFileVersion = $version.ToString() + "-$branchName+$commitId"

			Write-Verbose "Setting AssemblyFileVersion to $assemblyFileVersion" -Verbose
            $content += "[assembly:AssemblyInformationalVersion(`"$assemblyFileVersion`")]"

			if ($content -match "AssemblyTitle\(`"VstsBuildQueuer`"\)"){
				$versionNumber = "$($version.Major).$($version.Minor).$($version.Build)"
			}

            Set-Content -Path $file.FullName -Value $content
        }
    }
}

# used for CI and for releases
"##vso[task.setvariable variable=VersionNumber;]$versionNumber"