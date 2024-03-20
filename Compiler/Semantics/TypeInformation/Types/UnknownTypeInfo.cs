namespace Compiler.Semantics.TypeInformation.Types;

public class UnknownTypeInfo : TypeInfo
{
    public override string ToString()
    {
        return "unknown";
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitUnknownTypeInfo(this);
    }
}