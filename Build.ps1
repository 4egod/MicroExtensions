Set-Location $PSScriptRoot

$configuration = "Release"
$target = "MicroExtensions"

$path = "$target/bin/$configuration"

[array]$files = Get-ChildItem "$path/*.nupkg"
$file = $files[$files.Length - 1]

New-Item -ItemType Directory -Force -Path "$path/lib/net452"
Copy-Item -Path "TinyCLR/$target/bin/$configuration/*" -Destination "$path/lib/net452" -Include "$target.dll" -Force

Copy-Item -Path "MF/$target/bin/$configuration" -Destination "$path/lib/netmf43" -Recurse -Force
Remove-Item "$path/lib/netmf43" -Exclude @("$target.dll", "$target.pe", "le", "be") -Recurse -Force

Compress-Archive -Path "$path/lib" -DestinationPath $file -Update

Copy-Item -Path $file -Destination "../Bin" -Recurse -Force
Copy-Item -Path $file -Destination "D:\Nuget" -Force

Remove-Item "$path/lib" -Recurse -Force