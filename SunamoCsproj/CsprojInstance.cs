namespace SunamoCsproj;

/// <summary>
///     Zde jsou ty co používají XmlDocument
///     Už tu nic nepřidávat, vše už jen do
/// </summary>
public partial class CsprojInstance : CsprojConsts
{
    public string? PathFs;

    public CsprojInstance(XmlDocument xmlDocument)
    {
        this.XmlDocument = xmlDocument;
    }

    public CsprojInstance(string path, string? content = null)
    {
        XmlDocument = new XmlDocument();
        PathFs = path;
        try
        {
            if (content != null)
                XmlDocument.LoadXml(path);
            else if (path != null)
                XmlDocument.Load(path);
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
    ///     potřebuji path abych mohl vytvořit XmlDocument atd.
    ///     Ano, je k nečemu, když vkládám jen XmlDocument - na to vytvořím samostatný ctor.
    /// </summary>
    private CsprojInstance()
    {
    }

    public XmlDocument XmlDocument { get; set; }

    public void CreateOrReplaceMicrosoft_Extensions_Logging_Abstractions()
    {
        //moveAllItemsInItemGroupWhichContainsInInclude(ItemGroupTagName.PackageReference, "Include", "readme.md");
        var newEl = CreateNewItemGroupElement(ItemGroupTagName.PackageReference, "Microsoft.Extensions.Logging.Abstractions", "*", true, null);

        var include = GetItemGroup(ItemGroupTagName.PackageReference, "Include", "Microsoft.Extensions.Logging.Abstractions");

        if (include == null)
        {
            AddXmlElementToItemGroupOrCreate(newEl);
        }
    }

    private XmlNode GetItemGroup(ItemGroupTagName packageReference, string attName, string attValue)
    {
        var node = XmlDocument.SelectSingleNode($"/Project/ItemGroup/{packageReference}[@{attName}='{attValue}']");

        return node;
    }

    public void CreateOrReplaceItemGroupForReadmeMd()
    {
        RemoveAllItemsInItemGroupWhichContainsInInclude(ItemGroupTagName.None, "Include", "readme.md");
        var newEl = CreateNewItemGroupElement(ItemGroupTagName.None, null, null, true, ".");

        AddXmlElementToItemGroupOrCreate(newEl);
    }

    public void RemovePropertyGroupItem(string tag)
    {
        var node = XmlDocument.SelectSingleNode("/Project/PropertyGroup/" + tag);
        node?.ParentNode?.RemoveChild(node);
    }

    public string AddPropertyGroupItemIfNotExists(string key)
    {
        var desc = PropertyGroupItemContent(key);

        if (desc == null)
        {
            Console.WriteLine($"Enter new {key} for " + Path.GetFileNameWithoutExtension(PathFs));
            desc = Console.ReadLine();

            AddOrEditPropertyGroupItem(key, desc, new());
        }

        return desc;
    }



    public string? PropertyGroupItemContent(string tag)
    {
        var text = XmlDocument.SelectSingleNode("/Project/PropertyGroup/" + tag);
        if (text == null) return null;
        return text.InnerText;
    }

    public void RemoveSingleItemGroup(string attrValue, ItemGroupTagName tagName)
    {
        var temp = XmlDocument.SelectSingleNode($"/Project/ItemGroup/{tagName}[@{Include} = '{attrValue}']");
        temp?.ParentNode?.RemoveChild(temp);
    }

    public XmlElement CreateNewPackageReference(string include, string version)
    {
        return CreateNewItemGroupElement(ItemGroupTagName.PackageReference, include, version, null, null);
    }

    /// <summary>
    ///     Vrací ale jinak bude i value xml
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
        var newEl = XmlDocument.CreateElement(tagName.ToString());
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
        var itemGroup = XmlDocument.SelectSingleNode("/Project/ItemGroup");
        if (itemGroup == null)
        {
            var project = XmlDocument.SelectSingleNode("/Project");
            var newEl = XmlDocument.CreateElement("ItemGroup");
            itemGroup = project.AppendChild(newEl);
        }

        itemGroup.AppendChild(xe);
    }

    private XmlAttribute CreateAttr(XmlElement newEl, string attrName, string attrValue)
    {
        var attr = XmlDocument.CreateAttribute(attrName);
        attr.Value = attrValue;

        var newAttr = newEl.Attributes.Append(attr);
        return newAttr;
    }

    public string TurnOnOffAsyncConditionalCompilationSymbol(bool add)
    {
        return AddRemoveDefineConstant(add, ASYNC);
    }

    public void AddOrEditPropertyGroupItem(string tag, string content, ForceValueForKey forceValueForKey)
    {
        var versionEl = XmlDocument.SelectSingleNode("/Project/PropertyGroup/" + tag);

        content = SetValueByDict(content, tag, forceValueForKey);

        if (versionEl != null)
        {
            versionEl.InnerText = content;
        }
        else
        {
            var founded2 = XmlDocument.SelectSingleNode("/Project/PropertyGroup");
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
        return AddRemovePropertyGroupItem(add, "DefineConstants", defineConstant);
    }

    public string AddRemovePropertyGroupItem(bool add, string tag, string partValue)
    {
        var nodes = XmlDocument.SelectNodes("/Project/PropertyGroup");

        var isReleaseGlobal = false;
        var isDebugGlobal = false;

        foreach (XmlElement item2 in nodes)
        {
            var isRelease = false;
            var isDebug = false;

            var condition = XmlHelper.GetAttributeWithNameValue(item2, "Condition");

            if (condition == null)
            {
                continue;
            }

            var data = condition.Replace(" ", "");
            if (data == Release)
                isRelease = true;
            else if (data == Debug) isDebug = true;

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
                    var project = XmlDocument.SelectSingleNode("/Project");

                    AddPropertyGroupItemElement(XmlDocument, isRelease ? Release : Debug, add, partValue, tag, project, item2, []);
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
        if (!isDebugGlobal) AddPropertyGroupItemToProject(XmlDocument, Debug, add, partValue, tag, []);

        if (!isReleaseGlobal) AddPropertyGroupItemToProject(XmlDocument, Release, add, partValue, tag, []);

        return XHelper.FormatXmlInMemory(XmlDocument.OuterXml);
    }

    private string OnOff(string innerXml, bool add, string defineConstant)
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
    ///     Přidává pouze PropertyGroup text attr
    ///     '$(Configuration)|$(Platform)'=='Debug|AnyCPU'
    /// </summary>
    /// <param name="XmlDocument"></param>
    /// <param name="innerAttrValueCondition"></param>
    /// <param name="add"></param>
    /// <param name="defineConstantValue"></param>
    private void AddPropertyGroupItemToProject(XmlDocument XmlDocument, string innerAttrValueCondition, bool add,
        string defineConstantValue, string tag, ForceValueForKey forceValueForKey)
    {
        var project = XmlDocument.SelectSingleNode("/Project");

        var propertyGroup = XmlDocument.CreateNode(XmlNodeType.Element, PropertyGroup, null);
        AddPropertyGroupItemElement(XmlDocument, innerAttrValueCondition, add, defineConstantValue, tag, project, propertyGroup, forceValueForKey);
    }

    private void AddPropertyGroupItemElement(XmlDocument XmlDocument, string innerAttrValueCondition, bool add, string defineConstantValue, string tag, XmlNode? project, XmlNode propertyGroup, ForceValueForKey forceValueForKey)
    {
        var defineConstant = XmlDocument.CreateNode(XmlNodeType.Element, tag, null);
        defineConstantValue = SetValueByDict(defineConstantValue, tag, forceValueForKey);

        defineConstant.InnerXml = (tag == DefineConstants ? DefineConstantsInner + ";" : "") + (add ? defineConstantValue : "");
        propertyGroup.AppendChild(defineConstant);

        var propertyGroupConditionAttr = XmlDocument.CreateAttribute(Condition);
        propertyGroupConditionAttr.Value = innerAttrValueCondition;

        propertyGroup.Attributes.Append(propertyGroupConditionAttr);


        project.AppendChild(propertyGroup);
    }

    private string SetValueByDict(string defineConstantValue, string tag, ForceValueForKey forceValueForKey)
    {
        if (forceValueForKey.TryGetValue(Path.GetFileNameWithoutExtension(PathFs), out var forceValueForKeyDict))
        {
            if (forceValueForKeyDict.TryGetValue(tag, out var forceValue))
            {
                defineConstantValue = forceValue;
            }
        }

        return defineConstantValue;
    }

    public void RemoveItemsFromItemGroupWithAttr(ItemGroupTagName tagName, string value)
    {
        var temp = XmlDocument.SelectNodes($"/Project/ItemGroup/{tagName}[@{value}]");
        // nutno zkontrolovat detailně co se bude mazat. 
        // snadná reverzní cesta neexistuje
        foreach (XmlNode item in temp) item.ParentNode.RemoveChild(item);
    }


    /// <summary>
    ///     Protože mám často null value hodnotách kde mi čisté where selže, je tu tato metdoa
    /// </summary>
    /// <returns></returns>
    public List<ItemGroupElement> GetAllItemsInItemGroupWhichContainsInInclude(ItemGroupTagName tagName, string attr,
        string mustContains)
    {
        var items = ItemsInItemGroup(tagName);
        items = FilterByAttrAndContains(items, attr, mustContains);
        return items;
    }


    public List<ItemGroupElement> FilterByAttrAndContains(List<ItemGroupElement> list, string attr,
        string mustContains)
    {
        return list.Where(data =>
            (attr == "Link" ? data.Link :
                attr == "Include" ? data.Include :
                throw new Exception($"{nameof(attr)} is {attr}, must be Link or Include"))
            .ContainsNullAllow(mustContains)).ToList();
    }

    public void RemoveAllItemsInItemGroupWhichContainsInInclude(ItemGroupTagName tagName, string attr,
        string mustContains)
    {
        var items = ItemsInItemGroup(tagName);
        items = FilterByAttrAndContains(items, attr, mustContains);

        if (items.Count != 0)
            foreach (var item in items)
                item.XmlNode.ParentNode.RemoveChild(item.XmlNode);
    }

    /// <summary>
    ///     Nepotřebuji tu vracet XmlDocument, je value každém vráceném prvku.OwnerDocument
    /// </summary>
    public List<ItemGroupElement> ItemsInItemGroup(ItemGroupTagName tagName)
    {
        var itemsInItemGroup = XmlDocument.SelectNodes("/Project/ItemGroup/" + tagName);

        var result = new List<ItemGroupElement>();

        foreach (XmlNode item in itemsInItemGroup)
        {
            var parameter = ItemGroupElement.Parse(item);

            result.Add(parameter);
        }

        return result;
    }


    public async Task ReplacePackageReferenceForProjectReference(string pathCsproj/*, string pathSlnFolder*/)
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

        var packagesNames = packages.Select(data => data.Include).ToList();
        var projectsNames = projects.Select(data => Path.GetFileNameWithoutExtension(data.Include)).ToList();

        var duplicatedPackages = CAG.GetDuplicities(packagesNames);
        var duplicatedProjects = CAG.GetDuplicities(projectsNames);

        var both = packagesNames.Intersect(projectsNames).ToList();

        var result = new DuplicatesInItemGroup
        {
            DuplicatedPackages = duplicatedPackages,
            DuplicatedProjects = duplicatedProjects,
            ExistsInPackageAndProjectReferences = both
        };
        var dd = result.HasDuplicates();
        return result;
    }


    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public async Task<string> RemoveDuplicatedProjectAndPackageReferences(DuplicatesInItemGroup data)
    {
        if (data == null) data = await DetectDuplicatedProjectAndPackageReferences();

        var nodes = XmlDocument.SelectNodes("/Project/ItemGroup/" + ItemGroupTagName.ProjectReference);

        var csprojNameToRelativePath = new Dictionary<string, string>();

        foreach (XmlNode item in nodes)
        {
            var value = XmlHelper.GetAttrValueOrInnerElement(item, Include);
            var key = Path.GetFileName(value).Replace(".csproj", string.Empty);
#if DEBUG
            if (!csprojNameToRelativePath.ContainsKey(key)) csprojNameToRelativePath.Add(key, value);
#else
csprojNameToRelativePath.Add(key, value);
#endif
        }

        var alreadyProcessedPackages = new List<string>();
        var alreadyProcessedProjects = new List<string>();

        var csi = new CsprojInstance(XmlDocument);


        foreach (var item in data.DuplicatedPackages)
            if (!alreadyProcessedPackages.Contains(item))
                alreadyProcessedPackages.Add(item);
            else
                csi.RemoveSingleItemGroup(item, ItemGroupTagName.PackageReference);

        foreach (var item in data.DuplicatedProjects)
            if (!alreadyProcessedProjects.Contains(item))
                alreadyProcessedProjects.Add(item);
            else
                csi.RemoveSingleItemGroup(csprojNameToRelativePath[item], ItemGroupTagName.ProjectReference);

        return XmlDocument.OuterXml;
    }

    /// <summary>
    ///     Return always content, even if into A1 is passed path
    /// </summary>
    /// <param name="pathOrContentCsproj"></param>
    /// <returns></returns>
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public async Task<string> RemoveDuplicatedProjectAndPackageReferences()
    {
        var data = await DetectDuplicatedProjectAndPackageReferences();

        if (data.HasDuplicates())
        {
            var result = await RemoveDuplicatedProjectAndPackageReferences(data);
            return result;
        }

        return XmlDocument.OuterXml;
    }

    /// <summary>
    /// Univerzální metoda pro úpravu atributu existujícího elementu value ItemGroup
    /// </summary>
    /// <param name="tagName">Typ elementu (např. PackageReference)</param>
    /// <param name="includeValue">Hodnota atributu Include</param>
    /// <param name="attributeName">Název upravovaného atributu</param>
    /// <param name="newValue">Nová hodnota atributu</param>
    public void UpdateItemGroupElementAttribute(ItemGroupTagName tagName, string includeValue, string attributeName, string newValue)
    {
        var node = XmlDocument.SelectSingleNode($"/Project/ItemGroup/{tagName}[@Include='{includeValue}']");
        if (node is XmlElement el)
        {
            var attr = el.GetAttributeNode(attributeName);
            if (attr != null)
            {
                attr.Value = newValue;
            }
            else
            {
                var newAttr = XmlDocument.CreateAttribute(attributeName);
                newAttr.Value = newValue;
                el.Attributes.Append(newAttr);
            }
        }
    }
}