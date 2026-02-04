namespace SunamoCsproj._sunamo;

/// <summary>
/// Helper class for throwing exceptions.
/// </summary>
internal class ThrowEx
{
    /// <summary>
    /// Throws a custom exception with specified message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    internal static void Custom(string message)
    {
        throw new Exception(message);
    }
}