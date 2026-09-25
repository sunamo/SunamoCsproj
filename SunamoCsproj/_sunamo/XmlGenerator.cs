namespace SunamoCsproj._sunamo;

// Element - element to which inner content is written immediately. Can be empty.
// Tag - element to which content can be written later or not at all.
internal class XmlGenerator
{
    internal StringBuilder StringBuilder { get; set; } = new StringBuilder();

    private bool _useStack = false;
    private Stack<string>? _stack = null;

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

    public override string ToString() => StringBuilder.ToString();

    internal void WriteTagWithAttrs(string tagName, List<string> attributes)
    {
        WriteTagWithAttrs(tagName, attributes.ToArray());
    }

    internal void WriteTagWithAttrs(string tagName, params string[] attributes)
    {
        WriteTagWithAttrs(true, tagName, attributes);
    }

    bool IsNulledOrEmpty(string text)
    {
        if (string.IsNullOrEmpty(text) || text == "(null)")
        {
            return true;
        }
        return false;
    }

    private void WriteTagWithAttrs(bool isAppendingNull, string tagName, params string[] attributes)
    {
        var tagBuilder = new StringBuilder();
        tagBuilder.AppendFormat("<{0} ", tagName);
        for (int i = 0; i < attributes.Length; i++)
        {
            var attributeName = attributes[i];
            var attributeValue = attributes[++i];
            if (string.IsNullOrEmpty(attributeValue) && isAppendingNull || !string.IsNullOrEmpty(attributeValue))
            {
                if (!IsNulledOrEmpty(attributeName) && isAppendingNull || !IsNulledOrEmpty(attributeValue))
                {
                    tagBuilder.AppendFormat("{0}=\"{1}\" ", attributeName, attributeValue);
                }
            }
        }
        tagBuilder.Append("<");
        var tagString = tagBuilder.ToString();
        if (_useStack)
        {
            _stack!.Push(tagString); // EN: Safe because _stack is initialized when _useStack is true / CZ: Bezpečné protože _stack je inicializován když _useStack je true
        }
        this.StringBuilder.Append(tagString);
    }
}
