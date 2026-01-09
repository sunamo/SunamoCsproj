// variables names: ok
namespace SunamoCsproj._sunamo;

/// <summary>
/// XML generator for creating XML strings with tags and attributes.
/// Element - element to which inner content is written immediately. Can be empty.
/// Tag - element to which content can be written later or not at all.
/// </summary>
internal class XmlGenerator
{
    /// <summary>
    /// The internal StringBuilder for building XML.
    /// </summary>
    internal StringBuilder StringBuilder { get; set; } = new StringBuilder();

    private bool _useStack = false;
    private Stack<string>? _stack = null;

    /// <summary>
    /// Initializes a new instance of XmlGenerator without stack support.
    /// </summary>
    internal XmlGenerator() : this(false)
    {
    }

    /// <summary>
    /// Initializes a new instance of XmlGenerator with optional stack support.
    /// </summary>
    /// <param name="useStack">Whether to use stack for tracking tags.</param>
    internal XmlGenerator(bool useStack)
    {
        _useStack = useStack;
        if (useStack)
        {
            _stack = new Stack<string>();
        }
    }

    /// <summary>
    /// Returns the generated XML as a string.
    /// </summary>
    /// <returns>The generated XML string.</returns>
    public override string ToString()
    {
        return StringBuilder.ToString();
    }

    /// <summary>
    /// Writes a tag with attributes from a list.
    /// </summary>
    /// <param name="tagName">The name of the XML tag.</param>
    /// <param name="attributes">List of attribute name/value pairs (alternating).</param>
    internal void WriteTagWithAttrs(string tagName, List<string> attributes)
    {
        WriteTagWithAttrs(tagName, attributes.ToArray());
    }

    /// <summary>
    /// Writes a tag with attributes. If any value is null, it won't be written.
    /// </summary>
    /// <param name="tagName">The name of the XML tag.</param>
    /// <param name="attributes">Array of attribute name/value pairs (alternating).</param>
    internal void WriteTagWithAttrs(string tagName, params string[] attributes)
    {
        WriteTagWithAttrs(true, tagName, attributes);
    }

    /// <summary>
    /// Checks if a string is null, empty, or "(null)".
    /// </summary>
    /// <param name="text">The text to check.</param>
    /// <returns>True if the text is null or empty.</returns>
    bool IsNulledOrEmpty(string text)
    {
        if (string.IsNullOrEmpty(text) || text == "(null)")
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Writes a tag with attributes. If any value is null, it won't be written unless isAppendingNull is true.
    /// </summary>
    /// <param name="isAppendingNull">Whether to append null values.</param>
    /// <param name="tagName">The name of the XML tag.</param>
    /// <param name="attributes">Array of attribute name/value pairs (alternating).</param>
    private void WriteTagWithAttrs(bool isAppendingNull, string tagName, params string[] attributes)
    {
        StringBuilder tagBuilder = new StringBuilder();
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
        string tagString = tagBuilder.ToString();
        if (_useStack)
        {
            _stack!.Push(tagString); // EN: Safe because _stack is initialized when _useStack is true / CZ: Bezpečné protože _stack je inicializován když _useStack je true
        }
        this.StringBuilder.Append(tagString);
    }
}