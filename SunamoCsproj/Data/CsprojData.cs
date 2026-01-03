// variables names: ok
namespace SunamoCsproj.Data;

/// <summary>
/// EN: Represents csproj file data structure.
/// CZ: Reprezentuje datovou strukturu csproj souboru.
/// </summary>
public class CsprojData
{
    /// <summary>
    /// EN: PropertyGroup section data.
    /// CZ: Data sekce PropertyGroup.
    /// </summary>
    public PropertyGroupData? PropertyGroup { get; set; }
}
