using Compiler.Syntax;
using Compiler.Syntax.Nodes;

namespace Compiler.ErrorHandling;

public class CompileError(string message) : Exception(message)
{
    public class PositionalError(string message, PositionData positionData) : CompileError(message)
    {
        public PositionData PositionData { get; } = positionData;

        public virtual string FormatError()
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

    public class SemanticError : PositionalError
    {
        public SemanticError(
            string message,
            PositionData positionData,
            string? inObject = null,
            string? inFunction = null
        ) : base(message, positionData)
        {
            InObject = inObject;
            InFunction = inFunction;
        }

        public SemanticError(
            string message, NodeContext nodeContext, string? inObject = null, string? inFunction = null
        ) : this(message, nodeContext.PositionData, inObject, inFunction)
        {
        }

        public SemanticError(string message, BaseNode baseNode, string? inObject = null, string? inFunction = null)
            : this(message, baseNode.NodeContext, inObject, inFunction)
        {
        }

        public string? InObject { get; }
        public string? InFunction { get; }

        public override string FormatError()
        {
            var useMessage = Message;

            if (InObject != null)
            {
                useMessage += $" in object {InObject}";
            }

            if (InFunction != null)
            {
                useMessage += $" in function {InFunction}";
            }

            return ErrorFormatter.FormatError(useMessage, PositionData);
        }
    }
}