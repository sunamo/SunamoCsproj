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

    public void AppendToSb(StringBuilder sb, string path)
    {
        if (!HasDuplicates())
        {
            return;
        }

        sb.AppendLine(path + ":");

        AddProperty(sb, nameof(DuplicatedPackages), DuplicatedPackages);
        AddProperty(sb, nameof(DuplicatedProjects), DuplicatedProjects);
        AddProperty(sb, nameof(ExistsInPackageAndProjectReferences), ExistsInPackageAndProjectReferences);

        sb.AppendLine();
        sb.AppendLine();
    }

    private void AddProperty(StringBuilder sb, string v, List<string> duplicatedPackages)
    {
        if (duplicatedPackages.Count > 0)
        {
            sb.AppendLine(v + ":");
            foreach (var item in duplicatedPackages)
            {
                sb.AppendLine(item);
            }
        }
    }
}
