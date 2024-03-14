namespace Compiler.TypeInformation.Types;

public class GenericTypeInfo(string name, List<TypeRef> genericParams) : TypeInfo
{
    public string Name { get; } = name;
    public List<TypeRef> GenericParams { get; } = genericParams;

    public override string ToString()
    {
        return $"{Name}[{string.Join(", ", GenericParams.Select(x => x.ToString()))}]";
    }

    public override bool Compare(TypeInfo other)
    {
        if (other is not GenericTypeInfo otherGenericTypeInfo)
        {
            return false;
        }

        if (otherGenericTypeInfo.Name != Name || otherGenericTypeInfo.GenericParams.Count != GenericParams.Count)
        {
            return false;
        }

        for (var c = 0; c < GenericParams.Count; c++)
        {
            if (!GenericParams[c].Compare(otherGenericTypeInfo.GenericParams[c]))
            {
                return false;
            }
        }

        return true;
    }

    public override bool HasDeferredTypes()
    {
        return GenericParams.Any(x => x.TypeInfo.HasDeferredTypes());
    }
}