namespace SunamoCsproj.Data;

public class DuplicatesInItemGroup
{
    public List<string> DuplicatedPackages { get; set; } = new();

    public List<string> DuplicatedProjects { get; set; } = new();

    public List<string> ExistsInPackageAndProjectReferences { get; set; } = new();

    public bool HasDuplicates()
    {
        return DuplicatedPackages.Count > 0 || DuplicatedProjects.Count > 0 || ExistsInPackageAndProjectReferences.Count > 0;
    }

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
