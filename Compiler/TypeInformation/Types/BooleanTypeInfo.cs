namespace Compiler.TypeInformation.Types;

public class BooleanTypeInfo : TypeInfo
{
    public override string ToString()
    {
        return "boolean";
    }

    public override bool Compare(TypeInfo other)
    {
        return other is BooleanTypeInfo;
    }

    public override bool HasDeferredTypes()
    {
        return false;
    }
}