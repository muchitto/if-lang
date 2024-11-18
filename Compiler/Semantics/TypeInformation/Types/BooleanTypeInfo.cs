namespace Compiler.Semantics.TypeInformation.Types;

public class BooleanTypeInfo : FoundationalTypeInfo
{
    public override string? TypeName => "boolean";

    public override string ToString()
    {
        return TypeName!;
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitBooleanTypeInfo(this);
    }

    public override bool Compare(ITypeComparer comparer, TypeInfo other)
    {
        return comparer.CompareBooleanTypeInfo(this, other);
    }
}