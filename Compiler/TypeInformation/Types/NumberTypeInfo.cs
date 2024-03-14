namespace Compiler.TypeInformation.Types;

public class NumberTypeInfo : TypeInfo
{
    public override string ToString()
    {
        return "number";
    }

    public override bool Compare(TypeInfo other)
    {
        return other is NumberTypeInfo;
    }

    public override bool HasDeferredTypes()
    {
        return false;
    }
}