namespace SunamoCsproj.Data;

/// <summary>
/// Represents the property group data type.
/// </summary>
public class PropertyGroupData
{
    /// <summary>
    /// Gets or sets the framework properties.
    /// </summary>
    public FrameworkProperties? FrameworkProperties { get; set; }
    /// <summary>
    /// Gets or sets the assembly attribute properties.
    /// </summary>
    public AssemblyAttributeProperties? AssemblyAttributeProperties { get; set; }
    /// <summary>
    /// Gets or sets the package properties.
    /// </summary>
    public PackageProperties? PackageProperties { get; set; }
    /// <summary>
    /// Gets or sets the publish related properties.
    /// </summary>
    public PublishRelatedProperties? PublishRelatedProperties { get; set; }
    /// <summary>
    /// Gets or sets the build related properties.
    /// </summary>
    public BuildRelatedProperties? BuildRelatedProperties { get; set; }
    /// <summary>
    /// Gets or sets the default item inclusion properties.
    /// </summary>
    public DefaultItemInclusionProperties? DefaultItemInclusionProperties { get; set; }
    /// <summary>
    /// Gets or sets the code analysis properties.
    /// </summary>
    public CodeAnalysisProperties? CodeAnalysisProperties { get; set; }
    /// <summary>
    /// Gets or sets the runtime configuration properties.
    /// </summary>
    public RuntimeConfigurationProperties? RuntimeConfigurationProperties { get; set; }
    /// <summary>
    /// Gets or sets the reference related properties.
    /// </summary>
    public ReferenceRelatedProperties? ReferenceRelatedProperties { get; set; }
    /// <summary>
    /// Gets or sets the restore related properties.
    /// </summary>
    public RestoreRelatedProperties? RestoreRelatedProperties { get; set; }
    /// <summary>
    /// Gets or sets the run related properties.
    /// </summary>
    public RunRelatedProperties? RunRelatedProperties { get; set; }
    /// <summary>
    /// Gets or sets the hosting related properties.
    /// </summary>
    public HostingRelatedProperties? HostingRelatedProperties { get; set; }
    /// <summary>
    /// Gets or sets the item metadata.
    /// </summary>
    public ItemMetadata? ItemMetadata { get; set; }


}


/// <summary>
/// Represents the framework properties type.
/// </summary>
public class FrameworkProperties
{
    /// <summary>
    /// The target framework.
    /// </summary>
    public string? TargetFramework
    {
        get; set;
    }

    /// <summary>
    /// The target frameworks.
    /// </summary>
    public string? TargetFrameworks
    {
        get; set;
    }

    /// <summary>
    /// The net standard implicit package version.
    /// </summary>
    public string? NetStandardImplicitPackageVersion
    {
        get; set;
    }

}

/// <summary>
/// Represents the assembly attribute properties type.
/// </summary>
public class AssemblyAttributeProperties
{
    /// <summary>
    /// The generate assembly info.
    /// </summary>
    public string? GenerateAssemblyInfo
    {
        get; set;
    }

    /// <summary>
    /// The generated assembly info file.
    /// </summary>
    public string? GeneratedAssemblyInfoFile
    {
        get; set;
    }

}

/// <summary>
/// Represents the package properties type.
/// </summary>
public class PackageProperties
{
    /// <summary>
    /// The pack release.
    /// </summary>
    public string? PackRelease
    {
        get; set;
    }
}

/// <summary>
/// Represents the publish related properties type.
/// </summary>
public class PublishRelatedProperties
{
    /// <summary>
    /// The append runtime identifier to output path.
    /// </summary>
    public string? AppendRuntimeIdentifierToOutputPath
    {
        get; set;
    }

    /// <summary>
    /// The append target framework to output path.
    /// </summary>
    public string? AppendTargetFrameworkToOutputPath
    {
        get; set;
    }

    /// <summary>
    /// The copy local lock file assemblies.
    /// </summary>
    public string? CopyLocalLockFileAssemblies
    {
        get; set;
    }

    /// <summary>
    /// The enable package validation.
    /// </summary>
    public string? EnablePackageValidation
    {
        get; set;
    }

    /// <summary>
    /// The error on duplicate publish output files.
    /// </summary>
    public string? ErrorOnDuplicatePublishOutputFiles
    {
        get; set;
    }

    /// <summary>
    /// The generate runtime config dev file.
    /// </summary>
    public string? GenerateRuntimeConfigDevFile
    {
        get; set;
    }

    /// <summary>
    /// The generate runtime configuration files.
    /// </summary>
    public string? GenerateRuntimeConfigurationFiles
    {
        get; set;
    }

    /// <summary>
    /// The generate satellite assemblies for core.
    /// </summary>
    public string? GenerateSatelliteAssembliesForCore
    {
        get; set;
    }

    /// <summary>
    /// The is publishable.
    /// </summary>
    public string? IsPublishable
    {
        get; set;
    }

    /// <summary>
    /// The preserve compilation context.
    /// </summary>
    public string? PreserveCompilationContext
    {
        get; set;
    }

    /// <summary>
    /// The preserve compilation references.
    /// </summary>
    public string? PreserveCompilationReferences
    {
        get; set;
    }

    /// <summary>
    /// The produce reference assembly in out dir.
    /// </summary>
    public string? ProduceReferenceAssemblyInOutDir
    {
        get; set;
    }

    /// <summary>
    /// The publish documentation file.
    /// </summary>
    public string? PublishDocumentationFile
    {
        get; set;
    }

    /// <summary>
    /// The publish documentation files.
    /// </summary>
    public string? PublishDocumentationFiles
    {
        get; set;
    }

    /// <summary>
    /// The publish references documentation files.
    /// </summary>
    public string? PublishReferencesDocumentationFiles
    {
        get; set;
    }

    /// <summary>
    /// The publish release.
    /// </summary>
    public string? PublishRelease
    {
        get; set;
    }

    /// <summary>
    /// The publish self contained.
    /// </summary>
    public string? PublishSelfContained
    {
        get; set;
    }

    /// <summary>
    /// The roll forward.
    /// </summary>
    public string? RollForward
    {
        get; set;
    }

    /// <summary>
    /// The runtime framework version.
    /// </summary>
    public string? RuntimeFrameworkVersion
    {
        get; set;
    }

    /// <summary>
    /// The runtime identifier.
    /// </summary>
    public string? RuntimeIdentifier
    {
        get; set;
    }

    /// <summary>
    /// The runtime identifiers.
    /// </summary>
    public string? RuntimeIdentifiers
    {
        get; set;
    }

    /// <summary>
    /// The satellite resource languages.
    /// </summary>
    public string? SatelliteResourceLanguages
    {
        get; set;
    }

    /// <summary>
    /// The self contained.
    /// </summary>
    public string? SelfContained
    {
        get; set;
    }

    /// <summary>
    /// The use app host.
    /// </summary>
    public string? UseAppHost
    {
        get; set;
    }

}

/// <summary>
/// Represents the build related properties type.
/// </summary>
public class BuildRelatedProperties
{
    /// <summary>
    /// The continuous integration build.
    /// </summary>
    public string? ContinuousIntegrationBuild
    {
        get; set;
    }

    /// <summary>
    /// The copy debug symbol files from packages.
    /// </summary>
    public string? CopyDebugSymbolFilesFromPackages
    {
        get; set;
    }

    /// <summary>
    /// The copy documentation files from packages.
    /// </summary>
    public string? CopyDocumentationFilesFromPackages
    {
        get; set;
    }

    /// <summary>
    /// The disable implicit framework defines.
    /// </summary>
    public string? DisableImplicitFrameworkDefines
    {
        get; set;
    }

    /// <summary>
    /// The documentation file.
    /// </summary>
    public string? DocumentationFile
    {
        get; set;
    }

    /// <summary>
    /// The embedded resource use dependent upon convention.
    /// </summary>
    public string? EmbeddedResourceUseDependentUponConvention
    {
        get; set;
    }

    /// <summary>
    /// The enable preview features.
    /// </summary>
    public string? EnablePreviewFeatures
    {
        get; set;
    }

    /// <summary>
    /// The enable windows targeting.
    /// </summary>
    public string? EnableWindowsTargeting
    {
        get; set;
    }

    /// <summary>
    /// The generate documentation file.
    /// </summary>
    public string? GenerateDocumentationFile
    {
        get; set;
    }

    /// <summary>
    /// The generate requires preview features attribute.
    /// </summary>
    public string? GenerateRequiresPreviewFeaturesAttribute
    {
        get; set;
    }

    /// <summary>
    /// The optimize implicitly triggered build.
    /// </summary>
    public string? OptimizeImplicitlyTriggeredBuild
    {
        get; set;
    }

    /// <summary>
    /// The disable runtime marshalling.
    /// </summary>
    public string? DisableRuntimeMarshalling
    {
        get; set;
    }

}

/// <summary>
/// Represents the default item inclusion properties type.
/// </summary>
public class DefaultItemInclusionProperties
{
    /// <summary>
    /// The default item excludes in project folder.
    /// </summary>
    public string? DefaultItemExcludesInProjectFolder
    {
        get; set;
    }

    /// <summary>
    /// The default item excludes.
    /// </summary>
    public string? DefaultItemExcludes
    {
        get; set;
    }

    /// <summary>
    /// The enable default compile items.
    /// </summary>
    public string? EnableDefaultCompileItems
    {
        get; set;
    }

    /// <summary>
    /// The enable default embedded resource items.
    /// </summary>
    public string? EnableDefaultEmbeddedResourceItems
    {
        get; set;
    }

    /// <summary>
    /// The enable default items.
    /// </summary>
    public string? EnableDefaultItems
    {
        get; set;
    }

    /// <summary>
    /// The enable default none items.
    /// </summary>
    public string? EnableDefaultNoneItems
    {
        get; set;
    }

}

/// <summary>
/// Represents the code analysis properties type.
/// </summary>
public class CodeAnalysisProperties
{
    /// <summary>
    /// The analysis level.
    /// </summary>
    public string? AnalysisLevel
    {
        get; set;
    }

    /// <summary>
    /// The analysis mode.
    /// </summary>
    public string? AnalysisMode
    {
        get; set;
    }

    /// <summary>
    /// The code analysis treat warnings as errors.
    /// </summary>
    public string? CodeAnalysisTreatWarningsAsErrors
    {
        get; set;
    }

    /// <summary>
    /// The enable NET analyzers.
    /// </summary>
    public string? EnableNETAnalyzers
    {
        get; set;
    }

    /// <summary>
    /// The enforce code style in build.
    /// </summary>
    public string? EnforceCodeStyleInBuild
    {
        get; set;
    }

    /// <summary>
    /// The skip upgrade net analyzers nu get warning.
    /// </summary>
    public string? _SkipUpgradeNetAnalyzersNuGetWarning
    {
        get; set;
    }

}

/// <summary>
/// Represents the runtime configuration properties type.
/// </summary>
public class RuntimeConfigurationProperties
{
    /// <summary>
    /// The autorelease pool support.
    /// </summary>
    public string? AutoreleasePoolSupport
    {
        get; set;
    }

    /// <summary>
    /// The concurrent garbage collection.
    /// </summary>
    public string? ConcurrentGarbageCollection
    {
        get; set;
    }

    /// <summary>
    /// The invariant globalization.
    /// </summary>
    public string? InvariantGlobalization
    {
        get; set;
    }

    /// <summary>
    /// The predefined cultures only.
    /// </summary>
    public string? PredefinedCulturesOnly
    {
        get; set;
    }

    /// <summary>
    /// The retain VM garbage collection.
    /// </summary>
    public string? RetainVMGarbageCollection
    {
        get; set;
    }

    /// <summary>
    /// The server garbage collection.
    /// </summary>
    public string? ServerGarbageCollection
    {
        get; set;
    }

    /// <summary>
    /// The thread pool max threads.
    /// </summary>
    public string? ThreadPoolMaxThreads
    {
        get; set;
    }

    /// <summary>
    /// The thread pool min threads.
    /// </summary>
    public string? ThreadPoolMinThreads
    {
        get; set;
    }

    /// <summary>
    /// The tiered compilation.
    /// </summary>
    public string? TieredCompilation
    {
        get; set;
    }

    /// <summary>
    /// The tiered compilation quick jit.
    /// </summary>
    public string? TieredCompilationQuickJit
    {
        get; set;
    }

    /// <summary>
    /// The tiered compilation quick jit for loops.
    /// </summary>
    public string? TieredCompilationQuickJitForLoops
    {
        get; set;
    }

    /// <summary>
    /// The tiered PGO.
    /// </summary>
    public string? TieredPGO
    {
        get; set;
    }

    /// <summary>
    /// The use windows thread pool.
    /// </summary>
    public string? UseWindowsThreadPool
    {
        get; set;
    }

}

/// <summary>
/// Represents the reference related properties type.
/// </summary>
public class ReferenceRelatedProperties
{
    /// <summary>
    /// The asset target fallback.
    /// </summary>
    public string? AssetTargetFallback
    {
        get; set;
    }

    /// <summary>
    /// The disable implicit framework references.
    /// </summary>
    public string? DisableImplicitFrameworkReferences
    {
        get; set;
    }

    /// <summary>
    /// The disable transitive framework reference downloads.
    /// </summary>
    public string? DisableTransitiveFrameworkReferenceDownloads
    {
        get; set;
    }

    /// <summary>
    /// The disable transitive project references.
    /// </summary>
    public string? DisableTransitiveProjectReferences
    {
        get; set;
    }

    /// <summary>
    /// The manage package versions centrally.
    /// </summary>
    public string? ManagePackageVersionsCentrally
    {
        get; set;
    }

}

/// <summary>
/// Represents the restore related properties type.
/// </summary>
public class RestoreRelatedProperties
{
    /// <summary>
    /// The use maui essentials.
    /// </summary>
    public string? UseMauiEssentials
    {
        get; set;
    }

    /// <summary>
    /// The validate executable references match self contained.
    /// </summary>
    public string? ValidateExecutableReferencesMatchSelfContained
    {
        get; set;
    }

}

/// <summary>
/// Represents the run related properties type.
/// </summary>
public class RunRelatedProperties
{
    /// <summary>
    /// The run arguments.
    /// </summary>
    public string? RunArguments
    {
        get; set;
    }

    /// <summary>
    /// The run working directory.
    /// </summary>
    public string? RunWorkingDirectory
    {
        get; set;
    }

}

/// <summary>
/// Represents the hosting related properties type.
/// </summary>
public class HostingRelatedProperties
{
    /// <summary>
    /// The enable com hosting.
    /// </summary>
    public string? EnableComHosting
    {
        get; set;
    }

    /// <summary>
    /// The enable dynamic loading.
    /// </summary>
    public string? EnableDynamicLoading
    {
        get; set;
    }

}

/// <summary>
/// Represents the generated file properties type.
/// </summary>
public class GeneratedFileProperties
{
    /// <summary>
    /// The disable implicit namespace imports.
    /// </summary>
    public string? DisableImplicitNamespaceImports
    {
        get; set;
    }

    /// <summary>
    /// The implicit usings.
    /// </summary>
    public string? ImplicitUsings
    {
        get; set;
    }

    /// <summary>
    /// The items.
    /// </summary>
    public string? Items
    {
        get; set;
    }

    /// <summary>
    /// The assembly metadata.
    /// </summary>
    public string? AssemblyMetadata
    {
        get; set;
    }

    /// <summary>
    /// The internals visible to.
    /// </summary>
    public string? InternalsVisibleTo
    {
        get; set;
    }

    /// <summary>
    /// The package reference.
    /// </summary>
    public string? PackageReference
    {
        get; set;
    }

    /// <summary>
    /// The trimmer root assembly.
    /// </summary>
    public string? TrimmerRootAssembly
    {
        get; set;
    }

    /// <summary>
    /// The using.
    /// </summary>
    public string? Using
    {
        get; set;
    }

}

/// <summary>
/// Represents the item metadata type.
/// </summary>
public class ItemMetadata
{
    /// <summary>
    /// The copy to publish directory.
    /// </summary>
    public string? CopyToPublishDirectory
    {
        get; set;
    }

    /// <summary>
    /// The link base.
    /// </summary>
    public string? LinkBase
    {
        get; set;
    }

}