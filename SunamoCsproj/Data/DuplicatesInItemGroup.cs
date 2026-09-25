namespace SunamoCsproj.Data;

/// <summary>
/// Represents duplicate references found in csproj ItemGroup elements.
/// </summary>
public class DuplicatesInItemGroup
{
    /// <summary>
    /// Gets or sets the list of duplicated package references.
    /// </summary>
    public List<string> DuplicatedPackages { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of duplicated project references.
    /// </summary>
    public List<string> DuplicatedProjects { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of references that exist in both package and project references.
    /// </summary>
    public List<string> ExistsInPackageAndProjectReferences { get; set; } = new();

    /// <summary>
    /// Checks if any duplicates exist.
    /// </summary>
    /// <returns>True if any duplicates exist, false otherwise.</returns>
    public bool HasDuplicates()
    {
        return DuplicatedPackages.Count > 0 || DuplicatedProjects.Count > 0 || ExistsInPackageAndProjectReferences.Count > 0;
    }

    /// <summary>
    /// Appends duplicate information to StringBuilder.
    /// </summary>
    /// <param name="stringBuilder">The StringBuilder to append to.</param>
    /// <param name="path">The file path to include in output.</param>
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

    /// <summary>
    /// Adds property and its values to StringBuilder if list is not empty.
    /// </summary>
    /// <param name="stringBuilder">The StringBuilder to append to.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="values">The list of values to append.</param>
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