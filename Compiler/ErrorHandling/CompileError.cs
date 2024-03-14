using Compiler.Lexing;
using Compiler.Syntax.Nodes;

namespace Compiler.ErrorHandling;

public class CompileError(string message) : Exception(message)
{
    public class PositionalError(string message, PositionData positionData) : CompileError(message)
    {
        public PositionData PositionData { get; } = positionData;
    }

    public class GeneralError(string message) : CompileError(message)
    {
    }

    public class SyntaxError(string message, PositionData positionData) : PositionalError(message, positionData)
    {
    }

    public class LexingError(string message, PositionData positionData) : PositionalError(message, positionData)
    {
    }


    public class TypeError(string message, PositionData positionData) : PositionalError(message, positionData)
    {
    }

    public class SemanticError(string message) : Exception(message)
    {
        public class SymbolAlreadyDeclaredError(string name, BaseNode node)
            : SemanticError($"symbol {name} already declared in this scope")
        {
        }

        public class SymbolNotDeclaredError(string name, BaseNode node)
            : SemanticError($"symbol {name} not declared in this scope")
        {
        }

        public class ReturnStatementNotInFunctionError(BaseNode node)
            : SemanticError("return statement not in function")
        {
        }

        public class BreakStatementNotInLoopError(BaseNode node)
            : SemanticError("break statement not in loop")
        {
        }

        public class ContinueStatementNotInLoopError(BaseNode node)
            : SemanticError("continue statement not in loop")
        {
        }

        public class VoidAssignmentError(BaseNode node)
            : SemanticError("invalid type assignment")
        {
        }

        public class InvalidTypeAssignmentError(BaseNode node)
            : SemanticError("invalid type assignment")
        {
        }
    }
}