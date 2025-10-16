# SunamoCsproj

Working with csprojs at one package

## Overview

SunamoCsproj is part of the Sunamo package ecosystem, providing modular, platform-independent utilities for .NET development.

## Main Components

### Key Classes

- **CsprojConsts**
- **CsprojHelper**
- **CsprojNsHelper**
- **CsprojData**
- **DuplicatesInItemGroup**
- **ForceValueForKey**
- **ItemGroupElement**
- **ItemGroupElements**

### Key Methods

- `DetectDuplicatedProjectAndPackageReferences()`
- `CsprojPathFromName()`
- `AddLinkToCsproj()`
- `GetCsprojFromCsPath()`
- `RemoveDuplicatedProjectAndPackageReferences()`
- `CreateOrReplaceMicrosoft_Extensions_Logging_Abstractions()`
- `CreateOrReplaceItemGroupForReadmeMd()`
- `RemovePropertyGroupItem()`
- `AddPropertyGroupItemIfNotExists()`
- `RemoveSingleItemGroup()`

## Installation

```bash
dotnet add package SunamoCsproj
```

## Dependencies

- **Microsoft.Extensions.Logging.Abstractions** (v9.0.3)

## Package Information

- **Package Name**: SunamoCsproj
- **Version**: 25.6.7.1
- **Target Framework**: net9.0
- **Category**: Platform-Independent NuGet Package
- **Source Files**: 27

## Related Packages

This package is part of the Sunamo package ecosystem. For more information about related packages, visit the main repository.

## License

See the repository root for license information.
