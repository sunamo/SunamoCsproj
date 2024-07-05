namespace SunamoCsproj;

/// <summary>
/// Zde jsou ty co používají xd
/// Už tu nic nepřidávat, vše už jen do 
/// </summary>
public class CsprojInstance : CsprojConsts
{
    public XmlDocument xd { get; set; }
    public string Path = null;

    public CsprojInstance(XmlDocument xd)
    {
        this.xd = xd;
    }

    public CsprojInstance(string path, string content = null)
    {
        this.xd = new XmlDocument();
        this.Path = path;
        try
        {
            if (content != null)
            {
                xd.LoadXml(path);
            }
            else if (path != null)
            {
                xd.Load(path);
            }
            else
            {
                ThrowEx.Custom("Was not entered path neither content");
            }
        }
        catch (Exception ex)
        {
            ThrowEx.Custom(ex.Message + " Path: " + path);

        }
    }

    public void Save()
    {
        xd.Save(Path);
    }

    /// <summary>
    /// Je tento ctor k nečemu?
    /// 
    /// potřebuji path abych mohl vytvořit xd atd.
    /// Ano, je k nečemu, když vkládám jen xd - na to vytvořím samostatný ctor.
    /// </summary>
    private CsprojInstance()
    {

    }



    public void RemoveSingleItemGroup(string attrValue, ItemGroupTagName tagName)
    {
        var t = xd.SelectSingleNode($"/Project/ItemGroup/{tagName}[@{Include} = '{attrValue}']");
        if (t != null)
        {
            t.ParentNode?.RemoveChild(t);
        }

    }

    public void CreateElementInPropertyGroupWhichDoesNotExists()
    {
        // Používat na to MSBuild jako mám v MsBuildTarget

    }

    public XmlElement CreateNewPackageReference(string include, string version)
    {
        return CreateNewItemGroupElement(ItemGroupTagName.PackageReference, include, version);
    }

    /// <summary>
    /// Vrací ale jinak bude i v xml
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="include"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public XmlElement CreateNewItemGroupElement(ItemGroupTagName tagName, string include, string version)
    {
        var newEl = xd.CreateElement(ItemGroupTagName.PackageReference.ToString());
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

        return newEl;
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

    public string AddRemoveDefineConstant(bool add, string defineConstant)
    {
        //XmlDocument xd = new XmlDocument();
        //try
        //{
        //    xd.LoadXml(content);
        //}
        //catch (Exception)
        //{
        //    Console.WriteLine("Error! Probably not valid xml! " + content);
        //    return null;
        //}

        var nodes = xd.SelectNodes("/Project/PropertyGroup/DefineConstants");

        bool isRelease = false;
        bool isDebug = false;

        foreach (XmlElement item2 in nodes)
        {
            var d = XmlHelper.GetAttributeWithNameValue(item2.ParentNode, "Condition");
            if (d == Release)
            {
                isRelease = true;
            }
            else if (d == Debug)
            {
                isDebug = true;
            }

            item2.InnerXml = OnOff(item2.InnerXml, add, defineConstant);
        }

        // First must be debug due to unit tests
        if (!isDebug)
        {
            AddPropertyGroupItemToProject(xd, Debug, add, defineConstant);
        }

        if (!isRelease)
        {
            AddPropertyGroupItemToProject(xd, Release, add, defineConstant);
        }

        return XHelper.FormatXmlInMemory(xd.OuterXml);
    }

    private static string OnOff(string innerXml, bool add, string defineConstant)
    {
        var parts = innerXml.Split(';').ToList();

        if (add)
        {
            if (!parts.Contains(defineConstant))
            {
                parts.Add(defineConstant);
            }
        }
        else
        {
            parts.Remove(defineConstant);
        }

        return string.Join(';', parts);
    }

    /// <summary>
    /// Přidává pouze PropertyGroup s attr
    /// '$(Configuration)|$(Platform)'=='Debug|AnyCPU'
    /// 
    /// </summary>
    /// <param name="xd"></param>
    /// <param name="innerAttrValue"></param>
    /// <param name="add"></param>
    /// <param name="defineConstantValue"></param>
    private static void AddPropertyGroupItemToProject(XmlDocument xd, string innerAttrValue, bool add, string defineConstantValue)
    {
        var project = xd.SelectSingleNode("/Project");

        var propertyGroup = xd.CreateNode(XmlNodeType.Element, PropertyGroup, null);
        var defineConstant = xd.CreateNode(XmlNodeType.Element, DefineConstants, null);

        defineConstant.InnerXml = DefineConstantsInner + (add ? ";" + defineConstantValue : "");
        propertyGroup.AppendChild(defineConstant);

        var propertyGroupConditionAttr = xd.CreateAttribute(Condition);
        propertyGroupConditionAttr.Value = innerAttrValue;

        propertyGroup.Attributes.Append(propertyGroupConditionAttr);


        project.AppendChild(propertyGroup);
    }

    public void RemoveItemsFromItemGroupWithAttr(ItemGroupTagName tagName, string v)
    {
        var t = xd.SelectNodes($"/Project/ItemGroup/{tagName}[@{v}]");
        // nutno zkontrolovat detailně co se bude mazat. 
        // snadná reverzní cesta neexistuje
        foreach (XmlNode item in t)
        {
            item.ParentNode.RemoveChild(item);
        }
    }
}
