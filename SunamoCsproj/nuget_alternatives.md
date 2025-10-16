# NuGet Alternatives to SunamoCsproj

This document lists popular NuGet packages that provide similar functionality to SunamoCsproj.

## Overview

C# project file manipulation

## Alternative Packages

### Microsoft.Build
- **NuGet**: Microsoft.Build
- **Purpose**: MSBuild API
- **Key Features**: Project loading, evaluation, building

### Buildalyzer
- **NuGet**: Buildalyzer
- **Purpose**: MSBuild project analyzer
- **Key Features**: Parse projects, get references, analyze builds

### Microsoft.CodeAnalysis.Workspaces
- **NuGet**: Microsoft.CodeAnalysis.Workspaces.MSBuild
- **Purpose**: Roslyn workspace API
- **Key Features**: Solution/project loading, semantic analysis

### NuGet.ProjectModel
- **NuGet**: NuGet.ProjectModel
- **Purpose**: NuGet project operations
- **Key Features**: Package references, restore, lock files

## Comparison Notes

Buildalyzer for simple project analysis. Microsoft.Build for full MSBuild capabilities.

## Choosing an Alternative

Consider these alternatives based on your specific needs:
- **Microsoft.Build**: MSBuild API
- **Buildalyzer**: MSBuild project analyzer
- **Microsoft.CodeAnalysis.Workspaces**: Roslyn workspace API
