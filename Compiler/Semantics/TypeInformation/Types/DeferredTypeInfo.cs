namespace Compiler.Semantics.TypeInformation.Types;

public class DeferredTypeInfo : FoundationalTypeInfo
{
    public override string? TypeName => null;

    public override string ToString()
    {
        return "deferred";
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitDeferredTypeInfo(this);
    }

    public override bool Compare(ITypeComparer comparer, TypeInfo other)
    {
        return comparer.CompareDeferredTypeInfo(this, other);
    }
}