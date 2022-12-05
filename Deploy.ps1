
[CmdletBinding()]
Param
(
	[Parameter()]
		[ValidateNotNull()]
			$AssemblyPathsForVersion = {Param ($V) @((Join-Path $PSScriptRoot '../../Assemblies' $V))},

	[Parameter()]
		[ValidateNotNull()]
			$LastestVersion = '1.4',

	[Parameter()]
		[ValidateNotNull()]
		[AllowEmptyCollection()]
			$OldVersions = @(),

	[Parameter()]
		[ValidateNotNull()]
			$BuildOptions = @{
				Configuration = 'Release'
			}
)

$BuildPath = Join-Path $PSScriptRoot 'Build'
$ModPath = $PSScriptRoot
$BuiltModPath = Join-Path $BuildPath 'RimFridge'

$OriginalBranch = git branch --show-current

function Build-Version ($Version)
{
	git checkout $Version

	& $PSScriptRoot/Build.ps1 -AssemblyPaths (& $AssemblyPathsForVersion $Version) @BuildOptions

	$VersionFolder = Join-Path $BuiltModPath $Version

	New-Item -Force -ItemType Directory -Path $VersionFolder
	Copy-Item -Recurse -Force -Path (Join-Path $BuildPath $LastestVersion 'RimFridge/*') -Destination $VersionFolder

	$VersionFolder
}

try
{
	$LatestVersionFolder = (Build-Version $LastestVersion)[-1]
	Remove-Item -Recurse -Force -LiteralPath (Join-Path $BuiltModPath 'About') -ErrorAction Ignore
	Move-Item -Force -LiteralPath (Join-Path $LatestVersionFolder 'About') -Destination $BuiltModPath

	foreach ($Version in $OldVersions)
	{
		$VersionFolder = (Build-Version $Version)[-1]
		Remove-Item -Recurse -Force -LiteralPath (Join-Path $VersionFolder 'About')
	}
}
finally
{
	git checkout $OriginalBranch
}

