Set-Location $PSScriptRoot
$path = 'MicroExtensions\Bin\Release\'
Get-ChildItem ($path + '*.nupkg') | Rename-Item -NewName { $_.Name -replace '.nupkg','.zip' }
$file = Get-ChildItem ($path + '*.zip')
Copy-Item -Path '.\TinyCLR\MicroExtensions\bin\Release' -Destination ($path + 'lib\net452') -Exclude 'mscorlib.dll' -Recurse -Force
Copy-Item -Path '.\MF\MicroExtensions\bin\Release' -Destination ($path + 'lib\netmf43') -Recurse -Force
Remove-Item ($path + 'lib\netmf43\le') -Exclude @('*.pe', '*.pdbx') -Recurse -Force
Remove-Item ($path + 'lib\netmf43\be') -Exclude @('*.pe', '*.pdbx') -Recurse -Force
Compress-Archive -Path ($path + "lib") -DestinationPath ($path + $file.Name) -Update
Get-ChildItem ($path + '*.zip') | Rename-Item -NewName { $_.Name -replace '.zip','.nupkg' }
Copy-Item -Path ($path + '*.nupkg') -Destination '.\Bin' -Force
Copy-Item -Path ($path + '*.nupkg') -Destination 'D:\Nuget' -Force
Remove-Item ($path + 'lib') -Recurse -Force