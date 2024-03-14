namespace Compiler.TypeInformation.Types;

public class DeferredTypeInfo : TypeInfo
{
    public override string ToString()
    {
        return "deferred";
    }

    public override bool Compare(TypeInfo other)
    {
        return other is DeferredTypeInfo;
    }

    public override bool HasDeferredTypes()
    {
        return true;
    }
}