namespace SunamoCsproj.Data;

/// <summary>
/// Represents the duplicates in item group type.
/// </summary>
public class DuplicatesInItemGroup
{
    /// <summary>
    /// Gets or sets the duplicated packages.
    /// </summary>
    public List<string> DuplicatedPackages { get; set; } = new();

    /// <summary>
    /// Gets or sets the duplicated projects.
    /// </summary>
    public List<string> DuplicatedProjects { get; set; } = new();

    /// <summary>
    /// Gets or sets the exists in package and project references.
    /// </summary>
    public List<string> ExistsInPackageAndProjectReferences { get; set; } = new();

    /// <summary>
    /// Has duplicates.
    /// </summary>
    /// <returns>The has duplicates result.</returns>
    public bool HasDuplicates()
    {
        return DuplicatedPackages.Count > 0 || DuplicatedProjects.Count > 0 || ExistsInPackageAndProjectReferences.Count > 0;
    }

    /// <summary>
    /// Append to sb.
    /// </summary>
    /// <returns>The append to sb result.</returns>
    public void AppendToSb(StringBuilder stringBuilder, string path)
    {
        if (!HasDuplicates())
        {
            return;
        }

        stringBuilder.AppendLine(path + ":");

        AddProperty(stringBuilder, nameof(DuplicatedPackages), DuplicatedPackages);
        AddProperty(stringBuilder, nameof(DuplicatedProjects), DuplicatedProjects);
        AddProperty(stringBuilder, nameof(ExistsInPackageAndProjectReferences), ExistsInPackageAndProjectReferences);

        stringBuilder.AppendLine();
        stringBuilder.AppendLine();
    }

    private void AddProperty(StringBuilder stringBuilder, string propertyName, List<string> values)
    {
        if (values.Count > 0)
        {
            stringBuilder.AppendLine(propertyName + ":");
            foreach (var item in values)
            {
                stringBuilder.AppendLine(item);
            }
        }
    }
}
