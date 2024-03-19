namespace Compiler.Semantics.TypeInformation.Types;

public class BooleanTypeInfo : TypeInfo
{
    public override string ToString()
    {
        return "boolean";
    }

    public override bool HasDeferredTypes()
    {
        return false;
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitBooleanTypeInfo(this);
    }
}