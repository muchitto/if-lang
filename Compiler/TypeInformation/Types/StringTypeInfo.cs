namespace Compiler.TypeInformation.Types;

public class StringTypeInfo : TypeInfo
{
    public override bool Compare(TypeInfo other)
    {
        return other is StringTypeInfo;
    }

    public override string ToString()
    {
        return "string";
    }

    public override bool HasDeferredTypes()
    {
        return false;
    }
}