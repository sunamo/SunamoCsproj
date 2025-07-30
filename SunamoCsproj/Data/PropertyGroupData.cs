namespace SunamoCsproj.Data;

public class PropertyGroupData
{
    public FrameworkProperties FrameworkProperties { get; set; }
    public AssemblyAttributeProperties AssemblyAttributeProperties { get; set; }
    public PackageProperties PackageProperties { get; set; }
    public PublishRelatedProperties PublishRelatedProperties { get; set; }
    public BuildRelatedProperties BuildRelatedProperties { get; set; }
    public DefaultItemInclusionProperties DefaultItemInclusionProperties { get; set; }
    public CodeAnalysisProperties CodeAnalysisProperties { get; set; }
    public RuntimeConfigurationProperties RuntimeConfigurationProperties { get; set; }
    public ReferenceRelatedProperties ReferenceRelatedProperties { get; set; }
    public RestoreRelatedProperties RestoreRelatedProperties { get; set; }
    public RunRelatedProperties RunRelatedProperties { get; set; }
    public HostingRelatedProperties HostingRelatedProperties { get; set; }
    public ItemMetadata ItemMetadata { get; set; }


}


public class FrameworkProperties
{
    public string TargetFramework
    {
        get; set;
    }

    public string TargetFrameworks
    {
        get; set;
    }

    public string NetStandardImplicitPackageVersion
    {
        get; set;
    }

}

public class AssemblyAttributeProperties
{
    public string GenerateAssemblyInfo
    {
        get; set;
    }

    public string GeneratedAssemblyInfoFile
    {
        get; set;
    }

}

public class PackageProperties
{
    public string PackRelease
    {
        get; set;
    }
}

public class PublishRelatedProperties
{
    public string AppendRuntimeIdentifierToOutputPath
    {
        get; set;
    }

    public string AppendTargetFrameworkToOutputPath
    {
        get; set;
    }

    public string CopyLocalLockFileAssemblies
    {
        get; set;
    }

    public string EnablePackageValidation
    {
        get; set;
    }

    public string ErrorOnDuplicatePublishOutputFiles
    {
        get; set;
    }

    public string GenerateRuntimeConfigDevFile
    {
        get; set;
    }

    public string GenerateRuntimeConfigurationFiles
    {
        get; set;
    }

    public string GenerateSatelliteAssembliesForCore
    {
        get; set;
    }

    public string IsPublishable
    {
        get; set;
    }

    public string PreserveCompilationContext
    {
        get; set;
    }

    public string PreserveCompilationReferences
    {
        get; set;
    }

    public string ProduceReferenceAssemblyInOutDir
    {
        get; set;
    }

    public string PublishDocumentationFile
    {
        get; set;
    }

    public string PublishDocumentationFiles
    {
        get; set;
    }

    public string PublishReferencesDocumentationFiles
    {
        get; set;
    }

    public string PublishRelease
    {
        get; set;
    }

    public string PublishSelfContained
    {
        get; set;
    }

    public string RollForward
    {
        get; set;
    }

    public string RuntimeFrameworkVersion
    {
        get; set;
    }

    public string RuntimeIdentifier
    {
        get; set;
    }

    public string RuntimeIdentifiers
    {
        get; set;
    }

    public string SatelliteResourceLanguages
    {
        get; set;
    }

    public string SelfContained
    {
        get; set;
    }

    public string UseAppHost
    {
        get; set;
    }

}

public class BuildRelatedProperties
{
    public string ContinuousIntegrationBuild
    {
        get; set;
    }

    public string CopyDebugSymbolFilesFromPackages
    {
        get; set;
    }

    public string CopyDocumentationFilesFromPackages
    {
        get; set;
    }

    public string DisableImplicitFrameworkDefines
    {
        get; set;
    }

    public string DocumentationFile
    {
        get; set;
    }

    public string EmbeddedResourceUseDependentUponConvention
    {
        get; set;
    }

    public string EnablePreviewFeatures
    {
        get; set;
    }

    public string EnableWindowsTargeting
    {
        get; set;
    }

    public string GenerateDocumentationFile
    {
        get; set;
    }

    public string GenerateRequiresPreviewFeaturesAttribute
    {
        get; set;
    }

    public string OptimizeImplicitlyTriggeredBuild
    {
        get; set;
    }

    public string DisableRuntimeMarshalling
    {
        get; set;
    }

}

public class DefaultItemInclusionProperties
{
    public string DefaultItemExcludesInProjectFolder
    {
        get; set;
    }

    public string DefaultItemExcludes
    {
        get; set;
    }

    public string EnableDefaultCompileItems
    {
        get; set;
    }

    public string EnableDefaultEmbeddedResourceItems
    {
        get; set;
    }

    public string EnableDefaultItems
    {
        get; set;
    }

    public string EnableDefaultNoneItems
    {
        get; set;
    }

}

public class CodeAnalysisProperties
{
    public string AnalysisLevel
    {
        get; set;
    }

    public string AnalysisMode
    {
        get; set;
    }

    public string CodeAnalysisTreatWarningsAsErrors
    {
        get; set;
    }

    public string EnableNETAnalyzers
    {
        get; set;
    }

    public string EnforceCodeStyleInBuild
    {
        get; set;
    }

    public string _SkipUpgradeNetAnalyzersNuGetWarning
    {
        get; set;
    }

}

public class RuntimeConfigurationProperties
{
    public string AutoreleasePoolSupport
    {
        get; set;
    }

    public string ConcurrentGarbageCollection
    {
        get; set;
    }

    public string InvariantGlobalization
    {
        get; set;
    }

    public string PredefinedCulturesOnly
    {
        get; set;
    }

    public string RetainVMGarbageCollection
    {
        get; set;
    }

    public string ServerGarbageCollection
    {
        get; set;
    }

    public string ThreadPoolMaxThreads
    {
        get; set;
    }

    public string ThreadPoolMinThreads
    {
        get; set;
    }

    public string TieredCompilation
    {
        get; set;
    }

    public string TieredCompilationQuickJit
    {
        get; set;
    }

    public string TieredCompilationQuickJitForLoops
    {
        get; set;
    }

    public string TieredPGO
    {
        get; set;
    }

    public string UseWindowsThreadPool
    {
        get; set;
    }

}

public class ReferenceRelatedProperties
{
    public string AssetTargetFallback
    {
        get; set;
    }

    public string DisableImplicitFrameworkReferences
    {
        get; set;
    }

    public string DisableTransitiveFrameworkReferenceDownloads
    {
        get; set;
    }

    public string DisableTransitiveProjectReferences
    {
        get; set;
    }

    public string ManagePackageVersionsCentrally
    {
        get; set;
    }

}

public class RestoreRelatedProperties
{
    public string UseMauiEssentials
    {
        get; set;
    }

    public string ValidateExecutableReferencesMatchSelfContained
    {
        get; set;
    }

}

public class RunRelatedProperties
{
    public string RunArguments
    {
        get; set;
    }

    public string RunWorkingDirectory
    {
        get; set;
    }

}

public class HostingRelatedProperties
{
    public string EnableComHosting
    {
        get; set;
    }

    public string EnableDynamicLoading
    {
        get; set;
    }

}

public class GeneratedFileProperties
{
    public string DisableImplicitNamespaceImports
    {
        get; set;
    }

    public string ImplicitUsings
    {
        get; set;
    }

    public string Items
    {
        get; set;
    }

    public string AssemblyMetadata
    {
        get; set;
    }

    public string InternalsVisibleTo
    {
        get; set;
    }

    public string PackageReference
    {
        get; set;
    }

    public string TrimmerRootAssembly
    {
        get; set;
    }

    public string Using
    {
        get; set;
    }

}

public class ItemMetadata
{
    public string CopyToPublishDirectory
    {
        get; set;
    }

    public string LinkBase
    {
        get; set;
    }

}
