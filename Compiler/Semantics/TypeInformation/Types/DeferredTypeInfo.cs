namespace Compiler.Semantics.TypeInformation.Types;

public class DeferredTypeInfo : TypeInfo
{
    public override string ToString()
    {
        return "deferred";
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitDeferredTypeInfo(this);
    }
}