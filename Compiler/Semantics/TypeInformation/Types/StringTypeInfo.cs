namespace Compiler.Semantics.TypeInformation.Types;

public class StringTypeInfo : FoundationalTypeInfo
{
    public override string ToString()
    {
        return "string";
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitStringTypeInfo(this);
    }

    public override bool Compare(ITypeComparer comparer, TypeInfo other)
    {
        return comparer.CompareStringTypeInfo(this, other);
    }
}