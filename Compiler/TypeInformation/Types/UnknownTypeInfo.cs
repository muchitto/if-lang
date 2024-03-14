namespace Compiler.TypeInformation.Types;

public class UnknownTypeInfo : TypeInfo
{
    public override string ToString()
    {
        return "unknown";
    }

    public override bool Compare(TypeInfo other)
    {
        return other is UnknownTypeInfo;
    }

    public override bool HasDeferredTypes()
    {
        return false;
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitUnknownTypeInfo(this);
    }
}