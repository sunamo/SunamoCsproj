namespace SunamoCsproj._sunamo;

/// <summary>
/// Našel jsem ještě třídu DotXml ale ta umožňuje vytvářet jen dokumenty ke bude root ThisApp.Name
/// A nebo moje vlastní XML třídy, ale ty umí vytvářet jen třídy bez rozsáhlejšího xml vnoření.
/// Element - prvek kterému se zapisují ihned i innerObsah. Může být i prázdný.
/// Tag - prvek kterému to mohu zapsat později nebo vůbec.
/// </summary>
internal class XmlGenerator //: IXmlGenerator
{
    static Type type = typeof(XmlGenerator);
    internal StringBuilder sb = new StringBuilder();
    private bool _useStack = false;
    private Stack<string> _stack = null;
    internal XmlGenerator() : this(false)
    {
    }
    internal XmlGenerator(bool useStack)
    {
        _useStack = useStack;
        if (useStack)
        {
            _stack = new Stack<string>();
        }
    }
    internal void WriteNonPairTagWithAttrs(string tag, params string[] args)
    {
        sb.AppendFormat("<{0} ", tag);
        for (int i = 0; i < args.Length; i++)
        {
            string text = args[i];
            object hodnota = args[++i];
            sb.AppendFormat("{0}=\"{1}\" ", text, hodnota);
        }
        sb.Append(" />");
    }
    internal void WriteRaw(string p)
    {
        sb.Append(p);
    }
    public override string ToString()
    {
        return sb.ToString();
    }
    internal void WriteTagWithAttrs(string p, List<string> p_2)
    {
        WriteTagWithAttrs(p, p_2.ToArray());
    }
    /// <summary>
    /// if will be sth null, wont be writing
    /// </summary>
    /// <param name="p"></param>
    /// <param name="p_2"></param>
    internal void WriteTagWithAttrs(string p, params string[] p_2)
    {
        WriteTagWithAttrs(true, p, p_2);
    }
    /// <summary>
    /// Add also null
    /// </summary>
    /// <param name="nameTag"></param>
    /// <param name="p"></param>
    private void WriteTagWithAttrs(string nameTag, Dictionary<string, string> p)
    {
        WriteTagWithAttrs(true, nameTag, DictionaryHelper.GetListStringFromDictionary(p).ToArray());
    }
        bool IsNulledOrEmpty(string s)
    {
        if (string.IsNullOrEmpty(s) || s == "(null)")
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// if will be sth null, wont be writing
    /// </summary>
    /// <param name="p"></param>
    /// <param name="p_2"></param>
    private void WriteTagWithAttrs(bool appendNull, string p, params string[] p_2)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("<{0} ", p);
        for (int i = 0; i < p_2.Length; i++)
        {
            var attr = p_2[i];
            var val = p_2[++i];
            if (string.IsNullOrEmpty(val) && appendNull || !string.IsNullOrEmpty(val))
            {
                if (!IsNulledOrEmpty(attr) && appendNull || !IsNulledOrEmpty(val))
                {
                    sb.AppendFormat("{0}=\"{1}\" ", attr, val);
                }
            }
        }
        sb.Append("<");
        string r = sb.ToString();
        if (_useStack)
        {
            _stack.Push(r);
        }
        this.sb.Append(r);
    }
}
