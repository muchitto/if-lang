namespace Compiler.Semantics.TypeInformation.Types;

public class StringTypeInfo : TypeInfo
{
    public override string ToString()
    {
        return "string";
    }

    public override bool HasDeferredTypes()
    {
        return false;
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitStringTypeInfo(this);
    }
}