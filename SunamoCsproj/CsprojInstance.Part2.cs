namespace SunamoCsproj;

public partial class CsprojInstance : CsprojConsts
{


    /// <summary>
    /// Filter by attr and contains.
    /// </summary>
    /// <param name="list">The list.</param>
    /// <param name="attributeName">The attribute name.</param>
    /// <param name="mustContain">The must contain.</param>
    /// <returns>The filter by attr and contains result.</returns>
    public List<ItemGroupElement> FilterByAttrAndContains(List<ItemGroupElement> list, string attributeName,
        string mustContain)
    {
        return list.Where(element =>
            (attributeName == "Link" ? element.Link :
                attributeName == "Include" ? element.Include :
                throw new Exception($"{nameof(attributeName)} is {attributeName}, must be Link or Include"))
            .ContainsNullAllow(mustContain)).ToList();
    }

    /// <summary>
    /// Remove all items in item group which contains in include.
    /// </summary>
    /// <param name="tagName">The tag name.</param>
    /// <param name="attributeName">The attribute name.</param>
    /// <param name="mustContain">The must contain.</param>
    public void RemoveAllItemsInItemGroupWhichContainsInInclude(ItemGroupTagName tagName, string attributeName,
        string mustContain)
    {
        var items = ItemsInItemGroup(tagName);
        items = FilterByAttrAndContains(items, attributeName, mustContain);

        if (items.Count != 0)
            foreach (var item in items)
                item.XmlNode!.ParentNode!.RemoveChild(item.XmlNode);
    }

    /// <summary>
    /// Don't need to return XmlDocument, it's in each returned element.OwnerDocument.
    /// </summary>
    /// <param name="tagName">Tag name.</param>
    /// <returns>List of item group elements.</returns>
    public List<ItemGroupElement> ItemsInItemGroup(ItemGroupTagName tagName)
    {
        var itemsInItemGroup = XmlDocument.SelectNodes("/Project/ItemGroup/" + tagName);

        var result = new List<ItemGroupElement>();

        foreach (XmlNode item in itemsInItemGroup!)
        {
            var element = ItemGroupElement.Parse(item);

            result.Add(element!);
        }

        return result;
    }


    /// <summary>
    /// Replace package reference for project reference.
    /// </summary>
    /// <param name="pathCsproj">The path csproj.</param>
    public async Task ReplacePackageReferenceForProjectReference(string pathCsproj)
    {
        var csprojInstance = new CsprojInstance(pathCsproj);

        var packageReferences = ItemsInItemGroup(ItemGroupTagName.PackageReference);

        foreach (var item in packageReferences)
        {
            csprojInstance.RemoveSingleItemGroup(item.Include!, ItemGroupTagName.PackageReference);
            csprojInstance.CreateNewItemGroupElement(ItemGroupTagName.ProjectReference,
                "..\\" + item.Include + "\\" + item.Include + ".csproj", null!, null, null!);
        }

        csprojInstance.Save();
    }


    /// <summary>
    /// Detect duplicated project and package references.
    /// </summary>
    /// <returns>The detect duplicated project and package references result.</returns>
    public async Task<DuplicatesInItemGroup> DetectDuplicatedProjectAndPackageReferences()
    {
        var packages = ItemsInItemGroup(ItemGroupTagName.PackageReference);
        var projects = ItemsInItemGroup(ItemGroupTagName.ProjectReference);

        var packagesNames = packages.Select(package => package.Include!).ToList();
        var projectsNames = projects.Select(project => Path.GetFileNameWithoutExtension(project.Include)!).ToList();

        var duplicatedPackages = CAG.GetDuplicities(packagesNames);
        var duplicatedProjects = CAG.GetDuplicities(projectsNames);

        var both = packagesNames.Intersect(projectsNames).ToList();

        var result = new DuplicatesInItemGroup
        {
            DuplicatedPackages = duplicatedPackages,
            DuplicatedProjects = duplicatedProjects,
            ExistsInPackageAndProjectReferences = both
        };
        return result;
    }


    /// <summary>
    /// Remove duplicated project and package references.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>The remove duplicated project and package references result.</returns>
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public async Task<string> RemoveDuplicatedProjectAndPackageReferences(DuplicatesInItemGroup data)
    {
        if (data == null) data = await DetectDuplicatedProjectAndPackageReferences();

        var nodes = XmlDocument.SelectNodes("/Project/ItemGroup/" + ItemGroupTagName.ProjectReference);

        var csprojNameToRelativePath = new Dictionary<string, string>();

        foreach (XmlNode item in nodes!)
        {
            var value = XmlHelper.GetAttrValueOrInnerElement(item, Include);
            var key = Path.GetFileName(value)!.Replace(".csproj", string.Empty);
        }

        var alreadyProcessedPackages = new List<string>();
        var alreadyProcessedProjects = new List<string>();

        var csprojInstance = new CsprojInstance(XmlDocument);


        foreach (var item in data.DuplicatedPackages)
            if (!alreadyProcessedPackages.Contains(item))
                alreadyProcessedPackages.Add(item);
            else
                csprojInstance.RemoveSingleItemGroup(item, ItemGroupTagName.PackageReference);

        foreach (var item in data.DuplicatedProjects)
            if (!alreadyProcessedProjects.Contains(item))
                alreadyProcessedProjects.Add(item);
            else
                csprojInstance.RemoveSingleItemGroup(csprojNameToRelativePath[item], ItemGroupTagName.ProjectReference);

        return XmlDocument.OuterXml;
    }

    /// <summary>
    /// Returns always XML content of the csproj after removing duplicates.
    /// </summary>
    /// <returns>XML content.</returns>
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public async Task<string> RemoveDuplicatedProjectAndPackageReferences()
    {
        var data = await DetectDuplicatedProjectAndPackageReferences();

        if (data.HasDuplicates())
        {
            return await RemoveDuplicatedProjectAndPackageReferences(data);
        }

        return XmlDocument.OuterXml;
    }

    /// <summary>
    /// Universal method for modifying attribute of existing element in ItemGroup.
    /// </summary>
    /// <param name="tagName">Element type (e.g. PackageReference).</param>
    /// <param name="includeValue">Include attribute value.</param>
    /// <param name="attributeName">Name of attribute to modify.</param>
    /// <param name="newValue">New attribute value.</param>
    public void UpdateItemGroupElementAttribute(ItemGroupTagName tagName, string includeValue, string attributeName, string newValue)
    {
        var node = XmlDocument.SelectSingleNode($"/Project/ItemGroup/{tagName}[@Include='{includeValue}']");
        if (node is XmlElement element)
        {
            var attribute = element.GetAttributeNode(attributeName);
            if (attribute != null)
            {
                attribute.Value = newValue;
            }
            else
            {
                var newAttribute = XmlDocument.CreateAttribute(attributeName);
                newAttribute.Value = newValue;
                element.Attributes.Append(newAttribute);
            }
        }
    }
}