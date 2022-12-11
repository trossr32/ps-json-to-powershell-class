# JsonToPowershellClass

[![PowerShell Gallery Version](https://img.shields.io/powershellgallery/v/JsonToPowershellClass?label=JsonToPowershellClass&logo=powershell&style=plastic)](https://www.powershellgallery.com/packages/JsonToPowershellClass)
[![PowerShell Gallery](https://img.shields.io/powershellgallery/dt/JsonToPowershellClass?style=plastic)](https://www.powershellgallery.com/packages/JsonToPowershellClass)

A Powershell module that converts JSON to Powershell classes. JSON can be supplied as a string, a file that will be read or a URL that will be downloaded.

Available in the [Powershell Gallery](https://www.powershellgallery.com/packages/JsonToPowershellClass)

## Description
Convert JSON to Powershell classes. JSON can be supplied as a string, a file that will be read or a URL that will be downloaded.

Optionally create functions showing example usage.

Optionally write an output file.

Optionally copy the output to clipboard.

## Installation (from the Powershell Gallery)

```powershell
Install-Module  JsonToPowershellClass
Import-Module  JsonToPowershellClass
```

## Included cmdlets

```powershell
Convert-JsonToPowershellClass
```

## Examples

### Convert a JSON file to Powershell classes including usage examples and copy the output to clipboard.

```powershell
Convert-JsonToPowershellClass -JsonFile 'C:\Temp\a-json-file.json' -CopyToClipboard -IncludeExamples
```

### Download JSON from a URL, convert to Powershell classes using 'TestyMcTestFace' as the root class, include usage examples, copy to clipboard and write a ps1 file to the file system

```powershell
Convert-JsonToPowershellClass -Url 'https://dummyjson.com/products' -IncludeExamples -CopyToClipboard -OutputFile 'C:\Users\rob\Downloads\sample_new3.ps1' -RootObjectClassName TestyMcTestyFace
```

## Building the module and importing locally

### Build the .NET core solution

```powershell
dotnet build [Github clone/download directory]\ps-json-to-powershell-class\src\PsJsonToPowershellClass\PsJsonToPowershellClass.csproj
```

### Copy the built files to your Powershell modules directory

Remove any existing installation in this directory, create a new module directory and copy all the built files.

```powershell
Remove-Item "C:\Users\[User]\Documents\PowerShell\Modules\JsonToPowershellClass" -Recurse -Force -ErrorAction SilentlyContinue
New-Item -Path 'C:\Users\[User]\Documents\PowerShell\Modules\JsonToPowershellClass' -ItemType Directory
Get-ChildItem -Path "[Github clone/download directory]\ps-json-to-powershell-class\src\PsJsonToPowershellClass\bin\Debug\net6.0\" | Copy-Item -Destination "C:\Users\[User]\Documents\PowerShell\Modules\JsonToPowershellClass" -Recurse
```

## Contribute

Please raise an issue if you find a bug or want to request a new feature, or create a pull request to contribute.

<a href='https://ko-fi.com/K3K22CEIT' target='_blank'><img height='36' style='border:0px;height:36px;' src='https://cdn.ko-fi.com/cdn/kofi4.png?v=2' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a>
