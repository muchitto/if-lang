namespace Compiler.ErrorHandling;

public class CompileError(string message) : Exception(message)
{
    public class PositionalError(string message, PositionData positionData) : CompileError(message)
    {
        public PositionData PositionData { get; } = positionData;

        public string FormatError()
        {
            return ErrorFormatter.FormatError(Message, PositionData);
        }
    }

    public class GeneralError(string message) : CompileError(message)
    {
    }

    public class ParseError(string message, PositionData positionData) : PositionalError(message, positionData)
    {
    }

    public class LexingError(string message, PositionData positionData) : PositionalError(message, positionData)
    {
    }


    public class TypeError(string message, PositionData positionData) : PositionalError(message, positionData)
    {
    }

    public class SemanticError(string message, PositionData positionData) : PositionalError(message, positionData)
    {
    }
}