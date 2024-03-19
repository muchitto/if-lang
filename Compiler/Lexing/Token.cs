using Compiler.ErrorHandling;
using Compiler.Syntax;

namespace Compiler.Lexing;

public enum TokenType
{
    Identifier,
    Keyword,
    Number,
    String,
    NewLine,
    EndOfFile,
    Symbol,
    Operator
}

public readonly record struct Token
{
    public readonly PositionData PositionData;
    public readonly TokenType Type;
    public readonly string Value;


    public Token(TokenType type, string value, PositionData positionData)
    {
        Type = type;
        Value = value;
        PositionData = positionData;
    }

    public Token(TokenType type, PositionData positionData)
    {
        Type = type;
        Value = "";
        PositionData = positionData;
    }

    public bool Is(TokenType type)
    {
        return Type == type;
    }

    public bool Is(string value)
    {
        return Value == value;
    }

    public bool Is(TokenType type, string value)
    {
        return Is(value) && Is(type);
    }

    public Operator ToOperator()
    {
        return Value switch
        {
            "+" => Operator.Add,
            "-" => Operator.Subtract,
            "*" => Operator.Multiply,
            "/" => Operator.Divide,
            "%" => Operator.Modulo,
            "==" => Operator.Equal,
            "!=" => Operator.NotEqual,
            "<" => Operator.LessThan,
            "<=" => Operator.LessThanOrEqual,
            ">" => Operator.GreaterThan,
            ">=" => Operator.GreaterThanOrEqual,
            "&&" => Operator.And,
            "||" => Operator.Or,
            "!" => Operator.Not,

            // Bitwise
            "&" => Operator.BitwiseAnd,
            "|" => Operator.BitwiseOr,
            "~" => Operator.BitwiseNot,
            "^" => Operator.BitwiseXor,
            "<<" => Operator.BitwiseLeftShift,
            ">>" => Operator.BitwiseRightShift,

            // Assign
            "=" => Operator.Assign,
            "+=" => Operator.Add,
            "-=" => Operator.Subtract,
            "*=" => Operator.Multiply,
            "/=" => Operator.Divide,
            "%=" => Operator.Modulo,
            "&=" => Operator.BitwiseAnd,
            "|=" => Operator.BitwiseOr,
            "^=" => Operator.BitwiseXor,
            "<<=" => Operator.BitwiseLeftShift,
            ">>=" => Operator.BitwiseRightShift,

            // Other

            "is" => Operator.Is,

            _ => throw new Exception($"Unknown operator: {Value}")
        };
    }

    public bool IsOperator()
    {
        return Is(TokenType.Operator) || Is(TokenType.Keyword, "is");
    }

    public bool IsAssignmentOperator()
    {
        return Lexer.AssignmentOperators.Contains(Value);
    }
}