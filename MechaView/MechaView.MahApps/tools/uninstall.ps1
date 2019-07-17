param($installPath, $toolsPath, $package, $project)

remove-module $package.id -ErrorAction SilentlyContinue