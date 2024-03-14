
[CmdletBinding()]
Param
(
	[Parameter()]
		[ValidateNotNull()]
		[AllowEmptyCollection()]
			$AssemblyPaths = @((Join-Path $PSScriptRoot '../../Assemblies/1.5')),

	[Parameter()]
		[ValidateNotNull()]
			$Configuration = 'Debug'
)

$Branch = git branch --show-current
$BuildPath = Join-Path $PSScriptRoot 'Build'
$ModPath = $PSScriptRoot
$BuiltModPath = Join-Path $BuildPath "$Branch/RimFridge"
$BuildBinPath = Join-Path $BuildPath "bin/$Branch"
$BuildObjPath = Join-Path $BuildPath "obj/$Branch"

dotnet `
	build `
	"-c=$Configuration" `
	"-o=$BuildBinPath" `
	"-p:ObjPath=$BuildObjPath" `
	"-p:AssemblyPath=$([String]::Join(';', $AssemblyPaths))" `
	(Join-Path $ModPath 'Source')

if ($LASTEXITCODE -ne 0)
{
	return
}

New-Item -ItemType Directory -Force -Path $BuiltModPath

Copy-Item -Recurse -Force -PassThru -LiteralPath (Join-Path $ModPath 'About') -Destination $BuiltModPath
Copy-Item -Recurse -Force -PassThru -LiteralPath (Join-Path $ModPath 'Defs') -Destination $BuiltModPath
Copy-Item -Recurse -Force -PassThru -LiteralPath (Join-Path $ModPath 'Languages') -Destination $BuiltModPath
Copy-Item -Recurse -Force -PassThru -LiteralPath (Join-Path $ModPath 'Patches') -Destination $BuiltModPath
Copy-Item -Recurse -Force -PassThru -LiteralPath (Join-Path $ModPath 'Source') -Destination $BuiltModPath
Copy-Item -Recurse -Force -PassThru -LiteralPath (Join-Path $ModPath 'Textures') -Destination $BuiltModPath
Copy-Item -Recurse -Force -PassThru -LiteralPath (Join-Path $ModPath 'LICENSE') -Destination $BuiltModPath
Copy-Item -Recurse -Force -PassThru -LiteralPath (Join-Path $ModPath 'README.md') -Destination $BuiltModPath

New-Item -ItemType Directory -Force -Path (Join-Path $BuiltModPath 'Assemblies')

Copy-Item -Force -PassThru -Path (Join-Path $BuildBinPath 'RimFridge.*') -Destination (Join-Path $BuiltModPath 'Assemblies')

Remove-Item -Recurse -Force -LiteralPath (Join-Path $BuiltModPath 'Source/obj')

