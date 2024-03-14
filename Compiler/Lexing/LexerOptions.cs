namespace Compiler.Lexing;

public class LexerOptions
{
    public Stack<bool> RecordNewLines { get; set; } = new();

    public bool ShouldRecordNewLines => RecordNewLines.Count > 0 && RecordNewLines.Peek();
}