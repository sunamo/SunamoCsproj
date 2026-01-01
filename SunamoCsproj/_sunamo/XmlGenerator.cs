namespace SunamoCsproj._sunamo;

/// <summary>
/// EN: XML generator for creating XML strings with tags and attributes.
/// CZ: Generátor XML pro vytváření XML řetězců s tagy a atributy.
/// Element - element to which inner content is written immediately. Can be empty.
/// Tag - element to which content can be written later or not at all.
/// </summary>
internal class XmlGenerator
{
    internal StringBuilder StringBuilder = new StringBuilder();
    private bool _useStack = false;
    private Stack<string> _stack = null;

    /// <summary>
    /// EN: Initializes a new instance of XmlGenerator without stack support.
    /// CZ: Inicializuje novou instanci XmlGenerator bez podpory zásobníku.
    /// </summary>
    internal XmlGenerator() : this(false)
    {
    }

    /// <summary>
    /// EN: Initializes a new instance of XmlGenerator with optional stack support.
    /// CZ: Inicializuje novou instanci XmlGenerator s volitelnou podporou zásobníku.
    /// </summary>
    /// <param name="useStack">EN: Whether to use stack for tracking tags. CZ: Zda použít zásobník pro sledování tagů.</param>
    internal XmlGenerator(bool useStack)
    {
        _useStack = useStack;
        if (useStack)
        {
            _stack = new Stack<string>();
        }
    }

    /// <summary>
    /// EN: Returns the generated XML as a string.
    /// CZ: Vrátí vygenerovaný XML jako řetězec.
    /// </summary>
    /// <returns>EN: The generated XML string. CZ: Vygenerovaný XML řetězec.</returns>
    public override string ToString()
    {
        return StringBuilder.ToString();
    }

    /// <summary>
    /// EN: Writes a tag with attributes from a list.
    /// CZ: Zapíše tag s atributy ze seznamu.
    /// </summary>
    /// <param name="tagName">EN: The name of the XML tag. CZ: Název XML tagu.</param>
    /// <param name="attributes">EN: List of attribute name/value pairs. CZ: Seznam dvojic název/hodnota atributu.</param>
    internal void WriteTagWithAttrs(string tagName, List<string> attributes)
    {
        WriteTagWithAttrs(tagName, attributes.ToArray());
    }

    /// <summary>
    /// EN: Writes a tag with attributes. If any value is null, it won't be written.
    /// CZ: Zapíše tag s atributy. Pokud je nějaká hodnota null, nebude zapsána.
    /// </summary>
    /// <param name="tagName">EN: The name of the XML tag. CZ: Název XML tagu.</param>
    /// <param name="attributes">EN: Array of attribute name/value pairs (alternating). CZ: Pole dvojic název/hodnota atributu (střídavě).</param>
    internal void WriteTagWithAttrs(string tagName, params string[] attributes)
    {
        WriteTagWithAttrs(true, tagName, attributes);
    }

    /// <summary>
    /// EN: Checks if a string is null, empty, or "(null)".
    /// CZ: Zkontroluje zda je řetězec null, prázdný, nebo "(null)".
    /// </summary>
    /// <param name="text">EN: The text to check. CZ: Text ke kontrole.</param>
    /// <returns>EN: True if the text is null or empty. CZ: True pokud je text null nebo prázdný.</returns>
    bool IsNulledOrEmpty(string text)
    {
        if (string.IsNullOrEmpty(text) || text == "(null)")
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// EN: Writes a tag with attributes. If any value is null, it won't be written unless appendNull is true.
    /// CZ: Zapíše tag s atributy. Pokud je nějaká hodnota null, nebude zapsána pokud není appendNull true.
    /// </summary>
    /// <param name="appendNull">EN: Whether to append null values. CZ: Zda připojit null hodnoty.</param>
    /// <param name="tagName">EN: The name of the XML tag. CZ: Název XML tagu.</param>
    /// <param name="attributes">EN: Array of attribute name/value pairs (alternating). CZ: Pole dvojic název/hodnota atributu (střídavě).</param>
    private void WriteTagWithAttrs(bool appendNull, string tagName, params string[] attributes)
    {
        StringBuilder tagBuilder = new StringBuilder();
        tagBuilder.AppendFormat("<{0} ", tagName);
        for (int i = 0; i < attributes.Length; i++)
        {
            var attributeName = attributes[i];
            var attributeValue = attributes[++i];
            if (string.IsNullOrEmpty(attributeValue) && appendNull || !string.IsNullOrEmpty(attributeValue))
            {
                if (!IsNulledOrEmpty(attributeName) && appendNull || !IsNulledOrEmpty(attributeValue))
                {
                    tagBuilder.AppendFormat("{0}=\"{1}\" ", attributeName, attributeValue);
                }
            }
        }
        tagBuilder.Append("<");
        string tagString = tagBuilder.ToString();
        if (_useStack)
        {
            _stack.Push(tagString);
        }
        this.StringBuilder.Append(tagString);
    }
}
