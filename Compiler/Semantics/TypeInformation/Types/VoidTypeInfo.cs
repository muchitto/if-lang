namespace Compiler.Semantics.TypeInformation.Types;

public class VoidTypeInfo : TypeInfo
{
    public override string ToString()
    {
        return "void";
    }


    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitVoidTypeInfo(this);
    }
}