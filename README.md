# PsJsonToPowershellClass

[![PowerShell Gallery Version](https://img.shields.io/powershellgallery/v/ PsJsonToPowershellClass?label= PsJsonToPowershellClass&logo=powershell&style=plastic)](https://www.powershellgallery.com/packages/PsJsonToPowershellClass)
[![PowerShell Gallery](https://img.shields.io/powershellgallery/dt/PsJsonToPowershellClass?style=plastic)](https://www.powershellgallery.com/packages/PsJsonToPowershellClass)

A Powershell module that converts JSON to Powershell classes. JSON can be supplied as a string, a file that will be read or a URL that will be downloaded.

Available in the [Powershell Gallery](https://www.powershellgallery.com/packages/PsJsonToPowershellClass)

## Description
Convert JSON to Powershell classes. JSON can be supplied as a string, a file that will be read or a URL that will be downloaded.

Optionally create functions showing example usage.

Optionally write an output file.

Optionally copy the output to clipboard.

## Installation (from the Powershell Gallery)

```powershell
Install-Module  PsJsonToPowershellClass
Import-Module  PsJsonToPowershellClass
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

## Building the module and importing locally

### Build the .NET core solution

```powershell
dotnet build [Github clone/download directory]\ps-json-to-powershell-class\src\PsJsonToPowershellClass\PsJsonToPowershellClass.csproj
```

### Copy the built files to your Powershell modules directory

Remove any existing installation in this directory, create a new module directory and copy all the built files.

```powershell
Remove-Item "C:\Users\[User]\Documents\PowerShell\Modules\PsJsonToPowershellClass" -Recurse -Force -ErrorAction SilentlyContinue
New-Item -Path 'C:\Users\[User]\Documents\PowerShell\Modules\PsJsonToPowershellClass' -ItemType Directory
Get-ChildItem -Path "[Github clone/download directory]\ps-json-to-powershell-class\src\PsJsonToPowershellClass\bin\Debug\net6.0\" | Copy-Item -Destination "C:\Users\[User]\Documents\PowerShell\Modules\PsJsonToPowershellClass" -Recurse
```

## Contribute

Please raise an issue if you find a bug or want to request a new feature, or create a pull request to contribute.

<a href='https://ko-fi.com/K3K22CEIT' target='_blank'><img height='36' style='border:0px;height:36px;' src='https://cdn.ko-fi.com/cdn/kofi4.png?v=2' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a>
