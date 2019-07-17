param($installPath, $toolsPath, $package)

#import-module (join-path $toolsPath "$($package.id).psm1")
import-module (join-path $toolsPath "MechaView.psm1")

