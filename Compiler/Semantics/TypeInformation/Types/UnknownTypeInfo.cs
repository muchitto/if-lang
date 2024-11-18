namespace Compiler.Semantics.TypeInformation.Types;

public class UnknownTypeInfo : FoundationalTypeInfo
{
    public override string? TypeName => "unknown";

    public override string ToString()
    {
        return "unknown";
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitUnknownTypeInfo(this);
    }

    public override bool Compare(ITypeComparer comparer, TypeInfo other)
    {
        return comparer.CompareUnknownTypeInfo(this, other);
    }
}