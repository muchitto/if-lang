namespace Compiler.Semantics.TypeInformation.Types;

public class EnumItemTypeInfo : TypeInfo
{
    public EnumItemTypeInfo(string name, Dictionary<string, TypeRef> parameters)
    {
        Name = name;
        Parameters = parameters;
    }

    public string Name { get; }

    public Dictionary<string, TypeRef> Parameters { get; }

    public override bool HasDeferredTypes()
    {
        foreach (var parameter in Parameters.Values)
        {
            if (parameter.TypeInfo.HasDeferredTypes())
            {
                return true;
            }
        }

        return false;
    }
}