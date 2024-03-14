namespace Compiler.TypeInformation.Types;

public class VoidTypeInfo : TypeInfo
{
    public override string ToString()
    {
        return "void";
    }

    public override bool Compare(TypeInfo other)
    {
        return other is VoidTypeInfo;
    }

    public override bool HasDeferredTypes()
    {
        return false;
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitVoidTypeInfo(this);
    }
}