namespace Compiler.Syntax;

public enum Operator
{
    Add,
    Subtract,
    Multiply,
    Divide,
    Modulo,
    Power,
    Equal,
    NotEqual,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
    And,
    Or,
    Not,
    BitwiseAnd,
    BitwiseOr,
    BitwiseNot,
    BitwiseXor,
    BitwiseLeftShift,
    BitwiseRightShift,
    Assign,
    AddAssign,
    SubtractAssign,
    MultiplyAssign,
    DivideAssign,
    ModuloAssign,
    PowerAssign,
    BitwiseAndAssign,
    BitwiseOrAssign,
    BitwiseXorAssign,
    BitwiseLeftShiftAssign,
    BitwiseRightShiftAssign
}

public static class OperatorExtensions
{
    public static int Precedence(this Operator op)
    {
        return op switch
        {
            Operator.Power => 7,
            Operator.Multiply or Operator.Divide or Operator.Modulo => 6,
            Operator.Add or Operator.Subtract => 5,
            Operator.BitwiseLeftShift or Operator.BitwiseRightShift => 4,
            Operator.GreaterThan or Operator.LessThan or Operator.GreaterThanOrEqual or Operator.LessThanOrEqual => 3,
            Operator.Equal or Operator.NotEqual => 2,
            Operator.And => 1,
            Operator.Or => 0,
            Operator.Not or Operator.BitwiseNot => 8,
            _ => throw new Exception($"Unknown operator: {op}")
        };
    }

    public static Associativity Associativity(this Operator op)
    {
        return op switch
        {
            Operator.Power => Syntax.Associativity.Right,
            Operator.Multiply or Operator.Divide or Operator.Modulo or Operator.Add or Operator.Subtract
                or Operator.BitwiseLeftShift or Operator.BitwiseRightShift or Operator.GreaterThan or Operator.LessThan
                or Operator.GreaterThanOrEqual or Operator.LessThanOrEqual or Operator.Equal or Operator.NotEqual
                or Operator.And or Operator.Or => Syntax.Associativity.Left,
            Operator.Not or Operator.BitwiseNot => Syntax.Associativity.Unary,
            _ => throw new Exception($"Unknown operator: {op}")
        };
    }
}

public enum Associativity
{
    Left,
    Right,
    Unary
}