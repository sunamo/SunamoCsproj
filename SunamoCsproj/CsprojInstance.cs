namespace SunamoCsproj;

/// <summary>
/// EN: Class for working with csproj files using XmlDocument. Don't add anything here, add to CsprojHelper instead.
/// CZ: Třída pro práci s csproj soubory pomocí XmlDocument. Už tu nic nepřidávat, vše do CsprojHelper.
/// </summary>
public partial class CsprojInstance : CsprojConsts
{
    /// <summary>
    /// EN: File system path to csproj file.
    /// CZ: Cesta k csproj souboru na disku.
    /// </summary>
    public string? PathFs;

    /// <summary>
    /// EN: Constructor taking XmlDocument directly.
    /// CZ: Konstruktor přijímající XmlDocument přímo.
    /// </summary>
    /// <param name="xmlDocument">EN: XML document. CZ: XML dokument.</param>
    public CsprojInstance(XmlDocument xmlDocument)
    {
        this.XmlDocument = xmlDocument;
    }

    /// <summary>
    /// EN: Constructor taking path or content.
    /// CZ: Konstruktor přijímající cestu nebo obsah.
    /// </summary>
    /// <param name="path">EN: Path to csproj file. CZ: Cesta k csproj souboru.</param>
    /// <param name="content">EN: Content or null to load from path. CZ: Obsah nebo null pro načtení z cesty.</param>
    public CsprojInstance(string path, string? content = null)
    {
        XmlDocument = new XmlDocument();
        PathFs = path;
        try
        {
            if (content != null)
                XmlDocument.LoadXml(content);
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
    /// EN: Is this constructor useful? I need path to create XmlDocument etc. Yes, it is useful when inserting only XmlDocument - for that I create separate ctor.
    /// CZ: Je tento konstruktor k něčemu? Potřebuji path abych mohl vytvořit XmlDocument atd. Ano, je k něčemu když vkládám jen XmlDocument - na to vytvořím samostatný ctor.
    /// </summary>
    private CsprojInstance()
    {
        // EN: XmlDocument will be initialized by caller / CZ: XmlDocument bude inicializován volajícím
        XmlDocument = null!;
    }

    /// <summary>
    /// EN: XML document representing csproj file.
    /// CZ: XML dokument reprezentující csproj soubor.
    /// </summary>
    public XmlDocument XmlDocument { get; set; }

    /// <summary>
    /// EN: Creates or replaces Microsoft.Extensions.Logging.Abstractions package reference.
    /// CZ: Vytvoří nebo nahradí referenci na balíček Microsoft.Extensions.Logging.Abstractions.
    /// </summary>
    public void CreateOrReplaceMicrosoft_Extensions_Logging_Abstractions()
    {
        var newElement = CreateNewItemGroupElement(ItemGroupTagName.PackageReference, "Microsoft.Extensions.Logging.Abstractions", "*", true, null);

        var existingNode = GetItemGroup(ItemGroupTagName.PackageReference, "Include", "Microsoft.Extensions.Logging.Abstractions");

        if (existingNode == null)
        {
            AddXmlElementToItemGroupOrCreate(newElement);
        }
    }

    /// <summary>
    /// EN: Gets item group node by tag name and attribute.
    /// CZ: Získá uzel item group podle názvu tagu a atributu.
    /// </summary>
    /// <param name="tagName">EN: Item group tag name. CZ: Název tagu item group.</param>
    /// <param name="attributeName">EN: Attribute name. CZ: Název atributu.</param>
    /// <param name="attributeValue">EN: Attribute value. CZ: Hodnota atributu.</param>
    /// <returns>EN: XML node or null. CZ: XML uzel nebo null.</returns>
    private XmlNode GetItemGroup(ItemGroupTagName tagName, string attributeName, string attributeValue)
    {
        var node = XmlDocument.SelectSingleNode($"/Project/ItemGroup/{tagName}[@{attributeName}='{attributeValue}']");

        return node;
    }

    /// <summary>
    /// EN: Creates or replaces item group for readme.md file.
    /// CZ: Vytvoří nebo nahradí item group pro readme.md soubor.
    /// </summary>
    public void CreateOrReplaceItemGroupForReadmeMd()
    {
        RemoveAllItemsInItemGroupWhichContainsInInclude(ItemGroupTagName.None, "Include", "readme.md");
        var newElement = CreateNewItemGroupElement(ItemGroupTagName.None, null, null, true, ".");

        AddXmlElementToItemGroupOrCreate(newElement);
    }

    /// <summary>
    /// EN: Removes property group item by tag name.
    /// CZ: Odstraní položku property group podle názvu tagu.
    /// </summary>
    /// <param name="tagName">EN: Tag name to remove. CZ: Název tagu k odstranění.</param>
    public void RemovePropertyGroupItem(string tagName)
    {
        var node = XmlDocument.SelectSingleNode("/Project/PropertyGroup/" + tagName);
        node?.ParentNode?.RemoveChild(node);
    }

    public string AddPropertyGroupItemIfNotExists(string key)
    {
        var content = PropertyGroupItemContent(key);

        if (content == null)
        {
            Console.WriteLine($"Enter new {key} for " + Path.GetFileNameWithoutExtension(PathFs));
            content = Console.ReadLine();

            AddOrEditPropertyGroupItem(key, content, new());
        }

        return content;
    }



    public string? PropertyGroupItemContent(string tagName)
    {
        var node = XmlDocument.SelectSingleNode("/Project/PropertyGroup/" + tagName);
        if (node == null) return null;
        return node.InnerText;
    }

    public void RemoveSingleItemGroup(string attributeValue, ItemGroupTagName tagName)
    {
        var node = XmlDocument.SelectSingleNode($"/Project/ItemGroup/{tagName}[@{Include} = '{attributeValue}']");
        node?.ParentNode?.RemoveChild(node);
    }

    public XmlElement CreateNewPackageReference(string include, string version)
    {
        return CreateNewItemGroupElement(ItemGroupTagName.PackageReference, include, version, null, null);
    }

    /// <summary>
    /// EN: Returns value xml. If parameter not needed, insert null. Only creates new element and returns it, whether I insert it via ReplaceChild or AppendChild is up to me.
    /// CZ: Vrací value xml. Pokud některý parametr není potřeba, vloží se null. Pouze vytvoří nový element a vrátí jej, zda ho potom vložím přes ReplaceChild či AppendChild je na mně.
    /// </summary>
    /// <param name="tagName">EN: Tag name. CZ: Název tagu.</param>
    /// <param name="include">EN: Include attribute value or null. CZ: Hodnota atributu Include nebo null.</param>
    /// <param name="version">EN: Version attribute value or null. CZ: Hodnota atributu Version nebo null.</param>
    /// <param name="pack">EN: Pack attribute value or null. CZ: Hodnota atributu Pack nebo null.</param>
    /// <param name="packagePath">EN: PackagePath attribute value or null. CZ: Hodnota atributu PackagePath nebo null.</param>
    /// <returns>EN: Created XML element. CZ: Vytvořený XML element.</returns>
    public XmlElement CreateNewItemGroupElement(ItemGroupTagName tagName, string include, string version, bool? pack,
        string packagePath)
    {
        var newElement = XmlDocument.CreateElement(tagName.ToString());
        if (include != null)
        {
            var includeAttribute = CreateAttribute(newElement, Include, include);
            newElement.Attributes.Append(includeAttribute);
        }

        if (version != null)
        {
            var versionAttribute = CreateAttribute(newElement, Version, "*");
            newElement.Attributes.Append(versionAttribute);
        }

        if (pack.HasValue)
        {
            var packAttribute = CreateAttribute(newElement, "Pack", pack.Value.ToString());
            newElement.Attributes.Append(packAttribute);
        }

        if (packagePath != null)
        {
            var packagePathAttribute = CreateAttribute(newElement, "PackagePath", packagePath);
            newElement.Attributes.Append(packagePathAttribute);
        }

        return newElement;
    }

    public void AddXmlElementToItemGroupOrCreate(XmlElement element)
    {
        var itemGroup = XmlDocument.SelectSingleNode("/Project/ItemGroup");
        if (itemGroup == null)
        {
            var project = XmlDocument.SelectSingleNode("/Project");
            var newItemGroup = XmlDocument.CreateElement("ItemGroup");
            itemGroup = project.AppendChild(newItemGroup);
        }

        itemGroup.AppendChild(element);
    }

    private XmlAttribute CreateAttribute(XmlElement element, string attributeName, string attributeValue)
    {
        var attribute = XmlDocument.CreateAttribute(attributeName);
        attribute.Value = attributeValue;

        var appendedAttribute = element.Attributes.Append(attribute);
        return appendedAttribute;
    }

    public string TurnOnOffAsyncConditionalCompilationSymbol(bool add)
    {
        return AddRemoveDefineConstant(add, ASYNC);
    }

    public void AddOrEditPropertyGroupItem(string tagName, string content, ForceValueForKey forceValueForKey)
    {
        var existingElement = XmlDocument.SelectSingleNode("/Project/PropertyGroup/" + tagName);

        content = SetValueByDict(content, tagName, forceValueForKey);

        if (existingElement != null)
        {
            existingElement.InnerText = content;
        }
        else
        {
            var propertyGroupNode = XmlDocument.SelectSingleNode("/Project/PropertyGroup");
            var newElement = propertyGroupNode.AddElement(tagName);
            newElement.InnerText = content;
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

    public string AddRemovePropertyGroupItem(bool add, string tagName, string partValue)
    {
        var nodes = XmlDocument.SelectNodes("/Project/PropertyGroup");

        var isReleaseGlobal = false;
        var isDebugGlobal = false;

        foreach (XmlElement propertyGroup in nodes)
        {
            var isRelease = false;
            var isDebug = false;

            var condition = XmlHelper.GetAttributeWithNameValue(propertyGroup, "Condition");

            if (condition == null)
            {
                continue;
            }

            var conditionWithoutSpaces = condition.Replace(" ", "");
            if (conditionWithoutSpaces == Release)
                isRelease = true;
            else if (conditionWithoutSpaces == Debug) isDebug = true;

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
                var singleNode = propertyGroup.SelectSingleNode(tagName);

                if (singleNode != null)
                {
                    singleNode.InnerXml = OnOff(singleNode.InnerXml, add, partValue);
                }
                else
                {
                    var project = XmlDocument.SelectSingleNode("/Project");

                    AddPropertyGroupItemElement(isRelease ? Release : Debug, add, partValue, tagName, project, propertyGroup, []);
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
        if (!isDebugGlobal) AddPropertyGroupItemToProject(Debug, add, partValue, tagName, []);

        if (!isReleaseGlobal) AddPropertyGroupItemToProject(Release, add, partValue, tagName, []);

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
    /// EN: Adds only PropertyGroup text attr like '$(Configuration)|$(Platform)'=='Debug|AnyCPU'.
    /// CZ: Přidává pouze PropertyGroup textový atribut jako '$(Configuration)|$(Platform)'=='Debug|AnyCPU'.
    /// </summary>
    /// <param name="innerAttrValueCondition">EN: Inner attribute value condition. CZ: Vnitřní hodnota podmínky atributu.</param>
    /// <param name="add">EN: Whether to add. CZ: Zda přidat.</param>
    /// <param name="defineConstantValue">EN: Define constant value. CZ: Hodnota definované konstanty.</param>
    /// <param name="tagName">EN: Tag name. CZ: Název tagu.</param>
    /// <param name="forceValueForKey">EN: Force value for key. CZ: Vynucená hodnota pro klíč.</param>
    private void AddPropertyGroupItemToProject(string innerAttrValueCondition, bool add,
        string defineConstantValue, string tagName, ForceValueForKey forceValueForKey)
    {
        var project = XmlDocument.SelectSingleNode("/Project");

        var propertyGroup = XmlDocument.CreateNode(XmlNodeType.Element, PropertyGroup, null);
        AddPropertyGroupItemElement(innerAttrValueCondition, add, defineConstantValue, tagName, project, propertyGroup, forceValueForKey);
    }

    private void AddPropertyGroupItemElement(string innerAttrValueCondition, bool add, string defineConstantValue, string tagName, XmlNode? project, XmlNode propertyGroup, ForceValueForKey forceValueForKey)
    {
        var defineConstant = XmlDocument.CreateNode(XmlNodeType.Element, tagName, null);
        defineConstantValue = SetValueByDict(defineConstantValue, tagName, forceValueForKey);

        defineConstant.InnerXml = (tagName == DefineConstants ? DefineConstantsInner + ";" : "") + (add ? defineConstantValue : "");
        propertyGroup.AppendChild(defineConstant);

        var propertyGroupConditionAttribute = XmlDocument.CreateAttribute(Condition);
        propertyGroupConditionAttribute.Value = innerAttrValueCondition;

        propertyGroup.Attributes.Append(propertyGroupConditionAttribute);


        project.AppendChild(propertyGroup);
    }

    private string SetValueByDict(string defineConstantValue, string tagName, ForceValueForKey forceValueForKey)
    {
        if (forceValueForKey.TryGetValue(Path.GetFileNameWithoutExtension(PathFs), out var forceValueForKeyDict))
        {
            if (forceValueForKeyDict.TryGetValue(tagName, out var forceValue))
            {
                defineConstantValue = forceValue;
            }
        }

        return defineConstantValue;
    }

    /// <summary>
    /// EN: Removes items from item group with attribute. Must check in detail what will be deleted - no easy reverse path exists.
    /// CZ: Odstraní položky z item group s atributem. Nutno zkontrolovat detailně co se bude mazat - snadná reverzní cesta neexistuje.
    /// </summary>
    /// <param name="tagName">EN: Tag name. CZ: Název tagu.</param>
    /// <param name="attributeName">EN: Attribute name. CZ: Název atributu.</param>
    public void RemoveItemsFromItemGroupWithAttr(ItemGroupTagName tagName, string attributeName)
    {
        var nodes = XmlDocument.SelectNodes($"/Project/ItemGroup/{tagName}[@{attributeName}]");
        // EN: Must check in detail what will be deleted - no easy reverse path exists
        // CZ: Nutno zkontrolovat detailně co se bude mazat - snadná reverzní cesta neexistuje
        foreach (XmlNode item in nodes) item.ParentNode.RemoveChild(item);
    }


    /// <summary>
    /// EN: Gets all items in item group which contains in Include. Because I often have null values where pure where fails, this method is here.
    /// CZ: Získá všechny položky v item group které obsahují v Include. Protože mám často null hodnoty kde mi čisté where selže, je tu tato metoda.
    /// </summary>
    /// <param name="tagName">EN: Tag name. CZ: Název tagu.</param>
    /// <param name="attributeName">EN: Attribute name. CZ: Název atributu.</param>
    /// <param name="mustContain">EN: String that must be contained. CZ: Řetězec který musí být obsažen.</param>
    /// <returns>EN: List of item group elements. CZ: Seznam item group elementů.</returns>
    public List<ItemGroupElement> GetAllItemsInItemGroupWhichContainsInInclude(ItemGroupTagName tagName, string attributeName,
        string mustContain)
    {
        var items = ItemsInItemGroup(tagName);
        items = FilterByAttrAndContains(items, attributeName, mustContain);
        return items;
    }


    public List<ItemGroupElement> FilterByAttrAndContains(List<ItemGroupElement> list, string attributeName,
        string mustContain)
    {
        return list.Where(element =>
            (attributeName == "Link" ? element.Link :
                attributeName == "Include" ? element.Include :
                throw new Exception($"{nameof(attributeName)} is {attributeName}, must be Link or Include"))
            .ContainsNullAllow(mustContain)).ToList();
    }

    public void RemoveAllItemsInItemGroupWhichContainsInInclude(ItemGroupTagName tagName, string attributeName,
        string mustContain)
    {
        var items = ItemsInItemGroup(tagName);
        items = FilterByAttrAndContains(items, attributeName, mustContain);

        if (items.Count != 0)
            foreach (var item in items)
                item.XmlNode.ParentNode.RemoveChild(item.XmlNode);
    }

    /// <summary>
    /// EN: Don't need to return XmlDocument, it's in each returned element.OwnerDocument.
    /// CZ: Nepotřebuji tu vracet XmlDocument, je v každém vráceném prvku.OwnerDocument.
    /// </summary>
    /// <param name="tagName">EN: Tag name. CZ: Název tagu.</param>
    /// <returns>EN: List of item group elements. CZ: Seznam item group elementů.</returns>
    public List<ItemGroupElement> ItemsInItemGroup(ItemGroupTagName tagName)
    {
        var itemsInItemGroup = XmlDocument.SelectNodes("/Project/ItemGroup/" + tagName);

        var result = new List<ItemGroupElement>();

        foreach (XmlNode item in itemsInItemGroup)
        {
            var element = ItemGroupElement.Parse(item);

            result.Add(element);
        }

        return result;
    }


    public async Task ReplacePackageReferenceForProjectReference(string pathCsproj)
    {
        var csprojInstance = new CsprojInstance(pathCsproj);

        var packageReferences = ItemsInItemGroup(ItemGroupTagName.PackageReference);

        foreach (var item in packageReferences)
        {
            csprojInstance.RemoveSingleItemGroup(item.Include, ItemGroupTagName.PackageReference);
            csprojInstance.CreateNewItemGroupElement(ItemGroupTagName.ProjectReference,
                "..\\" + item.Include + "\\" + item.Include + ".csproj", null, null, null);
        }

        csprojInstance.Save();
    }


    public async Task<DuplicatesInItemGroup> DetectDuplicatedProjectAndPackageReferences()
    {
        var packages = ItemsInItemGroup(ItemGroupTagName.PackageReference);
        var projects = ItemsInItemGroup(ItemGroupTagName.ProjectReference);

        var packagesNames = packages.Select(package => package.Include).ToList();
        var projectsNames = projects.Select(project => Path.GetFileNameWithoutExtension(project.Include)).ToList();

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
    /// EN: Returns always XML content of the csproj after removing duplicates.
    /// CZ: Vrací vždy XML obsah csproj po odstranění duplikátů.
    /// </summary>
    /// <returns>EN: XML content. CZ: XML obsah.</returns>
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
    /// EN: Universal method for modifying attribute of existing element in ItemGroup.
    /// CZ: Univerzální metoda pro úpravu atributu existujícího elementu v ItemGroup.
    /// </summary>
    /// <param name="tagName">EN: Element type (e.g. PackageReference). CZ: Typ elementu (např. PackageReference).</param>
    /// <param name="includeValue">EN: Include attribute value. CZ: Hodnota atributu Include.</param>
    /// <param name="attributeName">EN: Name of attribute to modify. CZ: Název upravovaného atributu.</param>
    /// <param name="newValue">EN: New attribute value. CZ: Nová hodnota atributu.</param>
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