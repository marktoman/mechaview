$ErrorActionPreference = 'stop'
#$msbuild = "${env:ProgramFiles(x86)}\MSBuild\14.0\Bin\MSbuild.exe"
$msbuild = 'msbuild'
$fw = 'net45'
$gloablNugetDir = 'C:\lib\nuget'
$coreProj = "$PSScriptRoot\Mecha.Core\Mecha.Core.csproj"
$coreWpfProj = "$PSScriptRoot\Mecha.Core.Wpf\Mecha.Core.Wpf.csproj"
$pkgDir = "$PSScriptRoot\MechaView.MahApps"
$wpfMaAppDir = "$pkgDir\tools\Mecha.Wpf.Ma.App"
$lib = "$pkgDir\lib\$fw\"
$wpfNuspec = "$pkgDir\MechaView.MahApps.nuspec"
$settingsProj = "$PSScriptRoot\Mecha.Wpf.Settings\Mecha.Wpf.Settings.csproj"
$wpfMaProj = "$PSScriptRoot\Mecha.Wpf.Ma\Mecha.Wpf.Ma.csproj"

function Set-Version
{
    param(
        [Parameter(Mandatory, ValueFromPipeLine, ValueFromPipelineByPropertyName)]
            $Version,
        [Parameter()]
            [string] $Path = '.')

    begin {$a = @()}
    process {$a += $Version}
    end {
        $ErrorActionPreference = 'Stop'
        [int[]] $ver = $a.foreach({$_ -split '\.'})    
        $Path = join-path $Path version
        set-content $Path ($ver -join '.')
    }
}
function Get-Version
{
    param(    
        [Parameter()]
            [nullable[int]] $Increment,
        [Parameter()]
            [string] $Path = '.',
        [Parameter()]
            [switch] $AsNum)
    
    $Path = join-path $Path version

    [int[]] $ver =
        if (test-path $Path)
            {(get-content $Path) -split '\.'}
        else
            {0,0,0}

    if ($Increment -ne $null) {
        $ver[$Increment]++
        
        if ($Increment -lt 0)
            {$Increment = $ver.Length + $Increment}
                
        for ($i = $Increment + 1; $i -lt $ver.Length; $i++)
            {$ver[$i] = 0}
    }

    if ($AsNum) {$ver} else {$ver -join '.'}
}

function build($proj) {
    write-host "Building $(split-path $proj -Leaf)"
    $err = & $msbuild $proj `
        /t:Build /p:Configuration=Release /p:Platform=AnyCPU /v:quiet /nologo `
        /clp:ErrorsOnly `
        /p:OutputPath="$lib"
    if ($err) {throw $err}
}

$ver = get-version -inc -1 -path $PSScriptRoot

Remove-Item "$wpfMaAppDir\.vs","$wpfMaAppDir\obj" -Recurse -ErrorAction SilentlyContinue
mkdir $lib -ea 0|out-null

build $coreProj
build $coreWpfProj
build $settingsProj
build $wpfMaProj

nuget pack $wpfNuspec -outputdirectory $gloablNugetDir -version $ver 2>&1

set-version $ver -path $PSScriptRoot

