namespace SunamoCsproj;

/// <summary>
///     Zde jsou ty co používají xd
///     Už tu nic nepřidávat, vše už jen do
/// </summary>
public partial class CsprojInstance : CsprojConsts
{
    public string PathFs;

    public CsprojInstance(XmlDocument xd)
    {
        this.xd = xd;
    }

    public CsprojInstance(string path, string content = null)
    {
        xd = new XmlDocument();
        PathFs = path;
        try
        {
            if (content != null)
                xd.LoadXml(path);
            else if (path != null)
                xd.Load(path);
            else
                ThrowEx.Custom("Was not entered path neither content");
        }
        catch (Exception ex)
        {
            ThrowEx.Custom(ex.Message + " Path: " + path);
        }
    }

    /// <summary>
    ///     Je tento ctor k nečemu?
    ///     potřebuji path abych mohl vytvořit xd atd.
    ///     Ano, je k nečemu, když vkládám jen xd - na to vytvořím samostatný ctor.
    /// </summary>
    private CsprojInstance()
    {
    }

    public XmlDocument xd { get; set; }

    public void CreateOrReplaceItemGroupForReadmeMd()
    {
        RemoveAllItemsInItemGroupWhichContainsInInclude(ItemGroupTagName.None, "Include", "readme.md");
        var newEl = CreateNewItemGroupElement(ItemGroupTagName.None, null, null, true, ".");

        AddXmlElementToItemGroupOrCreate(newEl);
    }

    public void RemovePropertyGroupItem(string tag)
    {
        var node = xd.SelectSingleNode("/Project/PropertyGroup/" + tag);
        node?.ParentNode?.RemoveChild(node);
    }



    public string AddPropertyGroupItemIfNotExists(string key, string csprojPath)
    {
        var desc = PropertyGroupItemContent(key);

        if (desc == null)
        {
            Console.WriteLine($"Enter new {key} for " + Path.GetFileNameWithoutExtension(csprojPath));
            desc = Console.ReadLine();

            AddOrEditPropertyGroupItem(key, desc);
        }

        return desc;
    }



    public string PropertyGroupItemContent(string tag)
    {
        var s = xd.SelectSingleNode("/Project/PropertyGroup/" + tag);
        if (s == null) return null;
        return s.InnerText;
    }

    public void RemoveSingleItemGroup(string attrValue, ItemGroupTagName tagName)
    {
        var t = xd.SelectSingleNode($"/Project/ItemGroup/{tagName}[@{Include} = '{attrValue}']");
        if (t != null) t.ParentNode?.RemoveChild(t);
    }

    public void CreateElementInPropertyGroupWhichDoesNotExists()
    {
        // Používat na to MSBuild jako mám v MsBuildTarget
    }

    public XmlElement CreateNewPackageReference(string include, string version)
    {
        return CreateNewItemGroupElement(ItemGroupTagName.PackageReference, include, version, null, null);
    }

    /// <summary>
    ///     Vrací ale jinak bude i v xml
    ///     Pokud některý parametr není potřeba, vloží se null
    ///     Pouze vytvoří nový element a vrátí jej, to jestli ho potom vložím přes ReplaceChild či AppendChild už je na mě
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="include"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public XmlElement CreateNewItemGroupElement(ItemGroupTagName tagName, string include, string version, bool? pack,
        string packagePath)
    {
        var newEl = xd.CreateElement(tagName.ToString());
        if (include != null)
        {
            var attr = CreateAttr(newEl, Include, include);
            newEl.Attributes.Append(attr);
        }

        if (version != null)
        {
            var attrVersion = CreateAttr(newEl, Version, "*");
            newEl.Attributes.Append(attrVersion);
        }

        if (pack.HasValue)
        {
            var packAttr = CreateAttr(newEl, "Pack", pack.Value.ToString());
            newEl.Attributes.Append(packAttr);
        }

        if (packagePath != null)
        {
            var packagePathAttr = CreateAttr(newEl, "PackagePath", packagePath);
            newEl.Attributes.Append(packagePathAttr);
        }

        return newEl;
    }

    public void AddXmlElementToItemGroupOrCreate(XmlElement xe)
    {
        var itemGroup = xd.SelectSingleNode("/Project/ItemGroup");
        if (itemGroup == null)
        {
            var project = xd.SelectSingleNode("/Project");
            var newEl = xd.CreateElement("ItemGroup");
            itemGroup = project.AppendChild(newEl);
        }

        itemGroup.AppendChild(xe);
    }

    private XmlAttribute CreateAttr(XmlElement newEl, string attrName, string attrValue)
    {
        var attr = xd.CreateAttribute(attrName);
        attr.Value = attrValue;

        var newAttr = newEl.Attributes.Append(attr);
        return newAttr;
    }

    public string TurnOnOffAsyncConditionalCompilationSymbol(bool add)
    {
        return AddRemoveDefineConstant(add, ASYNC);
    }

    public void AddOrEditPropertyGroupItem(string tag, string content)
    {
        var versionEl = xd.SelectSingleNode("/Project/PropertyGroup/" + tag);

        if (versionEl != null)
        {
            versionEl.InnerText = content;
        }
        else
        {
            var founded2 = xd.SelectSingleNode("/Project/PropertyGroup");
            var newEl = founded2.AddElement(tag);
            newEl.InnerText = content;
        }
    }

    public string AddRemoveNoWarn(bool add, string warn)
    {
        return AddRemovePropertyGroupItem(add, "NoWarn", warn);
    }

    public string AddRemoveDefineConstant(bool add, string defineConstant)
    {
        return AddRemovePropertyGroupItem(add, "DefineConstant", defineConstant);
    }

    public string AddRemovePropertyGroupItem(bool add, string tag, string partValue)
    {
        var nodes = xd.SelectNodes("/Project/PropertyGroup");

        var isReleaseGlobal = false;
        var isDebugGlobal = false;

        foreach (XmlElement item2 in nodes)
        {


            var isRelease = false;
            var isDebug = false;

            var d = XmlHelper.GetAttributeWithNameValue(item2, "Condition");
            if (d == Release)
                isRelease = true;
            else if (d == Debug) isDebug = true;

            if (isDebug && isDebugGlobal)
            {
                continue;
            }

            if (isRelease && isReleaseGlobal)
            {
                continue;
            }

            if (isDebug || isRelease)
            {
                var ch = item2.ChildNodes;

                var singleNode = item2.SelectSingleNode(tag);

                if (singleNode != null)
                {
                    singleNode.InnerXml = OnOff(singleNode.InnerXml, add, partValue);
                }
                else
                {
                    var project = xd.SelectSingleNode("/Project");

                    AddPropertyGroupItemElement(xd, isRelease ? Release : Debug, add, partValue, tag, project, item2);
                }

                if (isDebug)
                {
                    isDebugGlobal = true;
                }
                else if (isRelease)
                {
                    isReleaseGlobal = true;
                }
            }
        }


        // First must be debug due to unit tests
        if (!isDebugGlobal) AddPropertyGroupItemToProject(xd, Debug, add, partValue, tag);

        if (!isReleaseGlobal) AddPropertyGroupItemToProject(xd, Release, add, partValue, tag);

        return XHelper.FormatXmlInMemory(xd.OuterXml);
    }

    private static string OnOff(string innerXml, bool add, string defineConstant)
    {
        var parts = innerXml.Split(';').ToList();

        if (add)
        {
            if (!parts.Contains(defineConstant)) parts.Add(defineConstant);
        }
        else
        {
            parts.Remove(defineConstant);
        }

        if (parts.Count == 1)
        {
            return parts[0];
        }

        return string.Join(';', parts);
    }

    /// <summary>
    ///     Přidává pouze PropertyGroup s attr
    ///     '$(Configuration)|$(Platform)'=='Debug|AnyCPU'
    /// </summary>
    /// <param name="xd"></param>
    /// <param name="innerAttrValueCondition"></param>
    /// <param name="add"></param>
    /// <param name="defineConstantValue"></param>
    private static void AddPropertyGroupItemToProject(XmlDocument xd, string innerAttrValueCondition, bool add,
        string defineConstantValue, string tag)
    {
        var project = xd.SelectSingleNode("/Project");

        var propertyGroup = xd.CreateNode(XmlNodeType.Element, PropertyGroup, null);
        AddPropertyGroupItemElement(xd, innerAttrValueCondition, add, defineConstantValue, tag, project, propertyGroup);
    }

    private static void AddPropertyGroupItemElement(XmlDocument xd, string innerAttrValueCondition, bool add, string defineConstantValue, string tag, XmlNode? project, XmlNode propertyGroup)
    {
        var defineConstant = xd.CreateNode(XmlNodeType.Element, tag, null);

        defineConstant.InnerXml = (tag == DefineConstants ? DefineConstantsInner + ";" : "") + (add ? defineConstantValue : "");
        propertyGroup.AppendChild(defineConstant);

        var propertyGroupConditionAttr = xd.CreateAttribute(Condition);
        propertyGroupConditionAttr.Value = innerAttrValueCondition;

        propertyGroup.Attributes.Append(propertyGroupConditionAttr);


        project.AppendChild(propertyGroup);
    }

    public void RemoveItemsFromItemGroupWithAttr(ItemGroupTagName tagName, string v)
    {
        var t = xd.SelectNodes($"/Project/ItemGroup/{tagName}[@{v}]");
        // nutno zkontrolovat detailně co se bude mazat. 
        // snadná reverzní cesta neexistuje
        foreach (XmlNode item in t) item.ParentNode.RemoveChild(item);
    }


    /// <summary>
    ///     Protože mám často null v hodnotách kde mi čisté where selže, je tu tato metdoa
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="attr"></param>
    /// <param name="mustContains"></param>
    /// <param name="pathCsproj"></param>
    /// <returns></returns>
    public List<ItemGroupElement> GetAllItemsInItemGroupWhichContainsInInclude(ItemGroupTagName tagName, string attr,
        string mustContains)
    {
        var items = ItemsInItemGroup(tagName);
        items = FilterByAttrAndContains(items, attr, mustContains);
        return items;
    }


    public static List<ItemGroupElement> FilterByAttrAndContains(List<ItemGroupElement> l, string attr,
        string mustContains)
    {
        return l.Where(d =>
            (attr == "Link" ? d.Link :
                attr == "Include" ? d.Include :
                throw new Exception($"{nameof(attr)} is {attr}, must be Link or Include"))
            .ContainsNullAllow(mustContains)).ToList();
    }

    public void RemoveAllItemsInItemGroupWhichContainsInInclude(ItemGroupTagName tagName, string attr,
        string mustContains)
    {
        var items = ItemsInItemGroup(tagName);
        items = FilterByAttrAndContains(items, attr, mustContains);

        if (items.Any())
            foreach (var item in items)
                item.XmlNode.ParentNode.RemoveChild(item.XmlNode);
    }

    /// <summary>
    ///     Nepotřebuji tu vracet XmlDocument, je v každém vráceném prvku.OwnerDocument
    /// </summary>
    public List<ItemGroupElement> ItemsInItemGroup(ItemGroupTagName tagName)
    {
        var itemsInItemGroup = xd.SelectNodes("/Project/ItemGroup/" + tagName);

        var result = new List<ItemGroupElement>();

        foreach (XmlNode item in itemsInItemGroup)
        {
            var p = ItemGroupElement.Parse(item);

            result.Add(p);
        }

        return result;
    }


    public async Task ReplacePackageReferenceForProjectReference(string pathCsproj, string pathSlnFolder)
    {
        //pathSlnFolder = pathSlnFolder.TrimEnd('\\') + "\\";

        var csp = new CsprojInstance(pathCsproj);

        var packagesRef = ItemsInItemGroup(ItemGroupTagName.PackageReference);

        foreach (var item in packagesRef)
        {
            //var fnwoe = Path.GetFileNameWithoutExtension(item.Include);
            csp.RemoveSingleItemGroup(item.Include, ItemGroupTagName.PackageReference);
            csp.CreateNewItemGroupElement(ItemGroupTagName.ProjectReference,
                "..\\" + item.Include + "\\" + item.Include + ".csproj", null, null, null);
        }

        csp.Save();
    }


    public async Task<DuplicatesInItemGroup> DetectDuplicatedProjectAndPackageReferences()
    {
        var packages = ItemsInItemGroup(ItemGroupTagName.PackageReference);
        var projects = ItemsInItemGroup(ItemGroupTagName.ProjectReference);

        var packagesNames = packages.Select(d => d.Include).ToList();
        var projectsNames = projects.Select(d => Path.GetFileNameWithoutExtension(d.Include)).ToList();

        var duplicatedPackages = CAG.GetDuplicities(packagesNames);
        var duplicatedProjects = CAG.GetDuplicities(projectsNames);

        var both = packagesNames.Intersect(projectsNames).ToList();

        var r = new DuplicatesInItemGroup
        {
            DuplicatedPackages = duplicatedPackages,
            DuplicatedProjects = duplicatedProjects,
            ExistsInPackageAndProjectReferences = both
        };
        var dd = r.HasDuplicates();
        return r;
    }


    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public async Task<string> RemoveDuplicatedProjectAndPackageReferences(DuplicatesInItemGroup d)
    {
        if (d == null) d = await DetectDuplicatedProjectAndPackageReferences();

        var nodes = xd.SelectNodes("/Project/ItemGroup/" + ItemGroupTagName.ProjectReference);

        var csprojNameToRelativePath = new Dictionary<string, string>();

        foreach (XmlNode item in nodes)
        {
            var v = XmlHelper.GetAttrValueOrInnerElement(item, Include);
            var key = Path.GetFileName(v).Replace(".csproj", string.Empty);
#if DEBUG
            if (!csprojNameToRelativePath.ContainsKey(key)) csprojNameToRelativePath.Add(key, v);
#else
csprojNameToRelativePath.Add(key, v);
#endif
        }

        var alreadyProcessedPackages = new List<string>();
        var alreadyProcessedProjects = new List<string>();

        var csi = new CsprojInstance(xd);


        foreach (var item in d.DuplicatedPackages)
            if (!alreadyProcessedPackages.Contains(item))
                alreadyProcessedPackages.Add(item);
            else
                csi.RemoveSingleItemGroup(item, ItemGroupTagName.PackageReference);

        foreach (var item in d.DuplicatedProjects)
            if (!alreadyProcessedProjects.Contains(item))
                alreadyProcessedProjects.Add(item);
            else
                csi.RemoveSingleItemGroup(csprojNameToRelativePath[item], ItemGroupTagName.ProjectReference);

        return xd.OuterXml;
    }

    /// <summary>
    ///     Return always content, even if into A1 is passed path
    /// </summary>
    /// <param name="pathOrContentCsproj"></param>
    /// <returns></returns>
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public async Task<string> RemoveDuplicatedProjectAndPackageReferences()
    {
        var d = await DetectDuplicatedProjectAndPackageReferences();

        if (d.HasDuplicates())
        {
            var result = await RemoveDuplicatedProjectAndPackageReferences(d);
            return result;
        }

        return xd.OuterXml;
    }
}