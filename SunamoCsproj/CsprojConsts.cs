namespace SunamoCsproj;

/// <summary>
/// Represents the csproj consts type.
/// </summary>
public class CsprojConsts
{
    // Is replaced by ItemGroupTagNme
    /// <summary>
    /// The include constant.
    /// </summary>
    public const string Include = "Include";
    /// <summary>
    /// The version constant.
    /// </summary>
    public const string Version = "Version";
    /// <summary>
    /// The link constant.
    /// </summary>
    public const string Link = "Link";

    /// <summary>
    /// The release constant.
    /// </summary>
    public const string Release = "'$(Configuration)|$(Platform)'=='Release|AnyCPU'";
    /// <summary>
    /// The debug constant.
    /// </summary>
    public const string Debug = "'$(Configuration)|$(Platform)'=='Debug|AnyCPU'";
    /// <summary>
    /// The property group constant.
    /// </summary>
    public const string PropertyGroup = "PropertyGroup";
    /// <summary>
    /// The define constants inner constant.
    /// </summary>
    public const string DefineConstantsInner = "$(DefineConstants)";
    /// <summary>
    /// The define constants constant.
    /// </summary>
    public const string DefineConstants = "DefineConstants";
    /// <summary>
    /// The condition constant.
    /// </summary>
    public const string Condition = "Condition";
    /// <summary>
    /// The ASYNC constant.
    /// </summary>
    public const string ASYNC = "ASYNC";
}