param($installPath, $toolsPath, $package, $project)

$project.object.references|
    Where-Object Name -in `
        'Mecha.Core.Wpf',
        'Mecha.Wpf.Ma',
        'MahApps.Metro',
        'System.Windows.Interactivity',
        'ControlzEx',
        'Castle.Core' |
    ForEach-Object Remove

$project.save()
