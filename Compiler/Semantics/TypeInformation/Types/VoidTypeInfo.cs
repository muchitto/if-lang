namespace Compiler.Semantics.TypeInformation.Types;

public class VoidTypeInfo : FoundationalTypeInfo
{
    public override string? TypeName => "void";

    public override string ToString()
    {
        return "void";
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitVoidTypeInfo(this);
    }

    public override bool Compare(ITypeComparer comparer, TypeInfo other)
    {
        return comparer.CompareVoidTypeInfo(this, other);
    }
}