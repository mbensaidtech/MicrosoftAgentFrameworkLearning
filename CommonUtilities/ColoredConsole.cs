namespace CommonUtilities;

/// <summary>
/// Provides static methods for Console to write messages with different colors based on message type.
/// Similar to Console.Write/WriteLine but with automatic color coding.
/// </summary>
public static class ColoredConsole
{
    /// <summary>
    /// Writes a user message in green color.
    /// </summary>
    public static void WriteUser(string? value)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(value);
        Console.ResetColor();
    }

    /// <summary>
    /// Writes a user message followed by a line terminator in green color.
    /// </summary>
    public static void WriteUserLine(string? value)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(value);
        Console.ResetColor();
    }

    /// <summary>
    /// Writes an assistant message in cyan color.
    /// </summary>
    public static void WriteAssistant(string? value)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(value);
        Console.ResetColor();
    }

    /// <summary>
    /// Writes an assistant message followed by a line terminator in cyan color.
    /// </summary>
    public static void WriteAssistantLine(string? value)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(value);
        Console.ResetColor();
    }

    /// <summary>
    /// Writes a system message in yellow color.
    /// </summary>
    public static void WriteSystem(string? value)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(value);
        Console.ResetColor();
    }

    /// <summary>
    /// Writes a system message followed by a line terminator in yellow color.
    /// </summary>
    public static void WriteSystemLine(string? value)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(value);
        Console.ResetColor();
    }
}

