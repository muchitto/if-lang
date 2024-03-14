namespace Compiler.TypeInformation.Types;

public class ObjectTypeInfo(string name, Dictionary<string, TypeRef> fields) : TypeInfo
{
    public string Name { get; } = name;
    public Dictionary<string, TypeRef> Fields { get; } = fields;

    public override string ToString()
    {
        return Name;
    }

    public override bool Compare(TypeInfo other)
    {
        return other is ObjectTypeInfo objectTypeInfo && Name == objectTypeInfo.Name;
    }

    public override bool HasDeferredTypes()
    {
        return Fields.Values.Any(x =>
        {
            if (x.TypeInfo == this)
            {
                return false;
            }

            return x.TypeInfo.HasDeferredTypes();
        });
    }
}