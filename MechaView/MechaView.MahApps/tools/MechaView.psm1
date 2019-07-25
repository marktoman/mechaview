
$nuget = "$PSScriptRoot/nuget.exe"
$vsRoot = "${env:ProgramFiles(x86)}/Microsoft Visual Studio/"

$ErrorActionPreference = 'stop'

function Start-App {
    param(
        [Parameter()]
        [Alias('n')]
            [string] $Name,

        [Parameter()]
        [Alias('a','args','al')]
            [string[]] $Arguments
    )

    if (!$Name) {$Name = (Get-Project).name + 'App'}
    
    $outDir = New-App -name $Name -build
    $build = Join-Path $outDir "$Name.exe"
    if (!(Test-Path $build)) {throw 'Cannot start the application. '}

    script:Write-Status 'Starting'
    Start-Process $build -ArgumentList (@('--debug') + $(if ($Arguments) {$Arguments}))
}

function New-App {
    param(
        [Parameter(Mandatory)]
        [Alias('n')]
            [string] $Name,

        [Parameter(ParameterSetName='Build')]
        [Alias('b')]
            [switch] $Build,
        [Parameter(ParameterSetName='Build')]
        [Alias('fw')]
            [string] $DotNetVersion,

        [Parameter(ParameterSetName='Publish')]
        [Alias('p')]
            [switch] $Publish,
        [Parameter(Mandatory, ParameterSetName='Publish')]
        [Alias('url')]
            [string] $InstallUrl)

    $vsFolder = Get-ChildItem $vsRoot |
        Where-Object Name -match '^\d+$' |
        Sort-Object -Descending |
        Get-ChildItem |
        Select-Object -First 1 |
        ForEach-Object FullName
    $msbuild = Join-Path "$vsFolder/" 'MSBuild/Current/Bin/msbuild.exe'
    if (!(Test-Path $msbuild)) {throw 'MSBuild.exe not found, please add it to your %PATH%'}

    if (!$Build -and !$Publish) {$Build = $true}

    if ($InstallUrl) {$InstallUrl = $InstallUrl.TrimEnd('\','/')}

    $proj = Get-Project
    $proj.Save()

    if (!$DotNetVersion) {
        $libTargetFw = $proj.Properties|Where-Object Name -eq 'TargetFrameworkMoniker'|ForEach-Object Value|ForEach-Object Trim
        $ver = $libTargetFw -replace '^\.NETFramework,Version=v([\d\.]+)$', '$1'
        if (!$ver) {throw '.NET Framework version could not be determined. Please use -DotNetVersion parameter.'}
        $DotNetVersion = $ver
    }

    $projName = $proj.Name
    $conf =
        if ($Publish)
            {$proj.ConfigurationManager|Where-Object ConfigurationName -eq Release}
        else
            {$proj.ConfigurationManager.ActiveConfiguration}

    $projAbsDir = [io.path]::GetDirectoryName($proj.FullName)
    $projRoot = Resolve-Path -Relative $projAbsDir
    $projOut  = Join-Path $projRoot $conf.Object.OutputPath
    $confName = $conf.ConfigurationName
    $projRelPath = Resolve-Path -Relative $proj.FullName

    Set-Location $PSScriptRoot\Mecha.Wpf.Ma.App
    Copy-Item (Join-Path $projRoot app.ico) . -Force

    #& $nuget restore -PackagesDirectory ..\..\packages\ 2>&1 | Out-Null
    & $nuget restore -PackagesDirectory .\packages\ 2>&1 | Out-Null
    
    $target = if ($Publish) {'Publish'} else {'Build'}

    script:Write-Status 'Building'
    $err = & $msbuild Mecha.Wpf.Ma.App.csproj `
        /t:$target `
        /p:Configuration=$confName `
        /p:Platform=AnyCPU `
        /v:quiet `
        /nologo `
        /clp:ErrorsOnly `
        /p:DotNetVersion="v$DotNetVersion" `
        /p:AppName="$Name" `
        /p:ProjName="$projName" `
        /p:ProjPath="$projRelPath" `
        /p:ProjGuid="{$($proj.Properties.Item('AssemblyGuid').Value)}" `
        /p:ProjOutput="$projOut"
        #/p:ProjRoot="$($projRoot)" `
        # /p:AppPublishUrl="$InstallUrl/$projName"
    if ($err) {throw $err}

    $projOut
}

function script:Write-Status($s) {Write-Host $s}
