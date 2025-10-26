namespace SunamoCsproj.Data;

public class DuplicatesInItemGroup
{
    public List<string> DuplicatedPackages { get; set; }
    public List<string> DuplicatedProjects { get; set; }
    public List<string> ExistsInPackageAndProjectReferences { get; set; }

    public bool HasDuplicates()
    {
        return !(new int?[] { DuplicatedPackages?.Count, DuplicatedProjects?.Count, ExistsInPackageAndProjectReferences?.Count }.All(d => d == 0));
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

    private void AddProperty(StringBuilder stringBuilder, string v, List<string> duplicatedPackages)
    {
        if (duplicatedPackages.Count > 0)
        {
            stringBuilder.AppendLine(v + ":");
            foreach (var item in duplicatedPackages)
            {
                stringBuilder.AppendLine(item);
            }
        }
    }
}