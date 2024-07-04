# AdmxPolicyManager

[![NuGet Version](https://img.shields.io/nuget/v/AdmxPolicyManager)](https://www.nuget.org/packages/AdmxPolicyManager/) ![Build Status](https://github.com/rkttu/AdmxPolicyManager/actions/workflows/dotnet.yml/badge.svg) [![GitHub Sponsors](https://img.shields.io/github/sponsors/rkttu)](https://github.com/sponsors/rkttu/)

Windows Policy Setting and Lookup Framework for use with the AdmxParser package

## Breaking Changes

### v0.5.x -> v0.6

**The code has been completely redeveloped to use AdmxParser's interpreted model, and it has been completely redeveloped to ensure stable operation, and is equipped with code that is completely different from the existing 0.5.x version.**

## Minimum Requirements

- Requires a platform with .NET Standard 2.0 or later, and Windows Vista+, Windows Server 2008+
  - Supported .NET Version: .NET Core 2.0+, .NET 5+, .NET Framework 4.6.1+, Mono 5.4+, UWP 10.0.16299+, Unity 2018.1+

## How to use

The code below is a simple depiction of how to use it. We'll add more full code examples as they become available.

```csharp
var admxDirectory = AdmxDirectory.GetSystemPolicyDefinitions();
await admxDirectory.LoadAsync();

var inetres = admxDirectory.LoadedAdmxContents.FirstOrDefault(x => x.TargetNamespace.prefix == "inetres")!;
var fontSizePolicy = inetres.GetUserPolicy("FontSize")!;
fontSizePolicy.SetUserPolicy(true);
Console.Out.WriteLine(fontSizePolicy.GetUserPolicy());

var elemId = fontSizePolicy.GetElementIds().First();
Console.Out.WriteLine(fontSizePolicy.GetUserElement(elemId));
fontSizePolicy.SetUserElement(elemId, 2);
Console.Out.WriteLine(fontSizePolicy.GetUserElement(elemId));
```

## License

This library follows Apache-2.0 license. See [LICENSE](./LICENSE) file for more information.
