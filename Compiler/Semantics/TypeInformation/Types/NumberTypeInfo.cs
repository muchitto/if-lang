namespace Compiler.Semantics.TypeInformation.Types;

public enum NumberType
{
    Int16,
    Int32,
    Int64,
    UInt16,
    UInt32,
    UInt64,
    Float32,
    Float64
}

public class NumberTypeInfo(NumberType type) : TypeInfo
{
    public NumberType NumberType { get; } = type;

    public override string ToString()
    {
        return NumberType.ToString().ToLower();
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitNumberTypeInfo(this);
    }
}