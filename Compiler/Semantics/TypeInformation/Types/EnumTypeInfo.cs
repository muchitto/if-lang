namespace Compiler.Semantics.TypeInformation.Types;

public class EnumTypeInfo : TypeInfo
{
    public EnumTypeInfo(string name, Dictionary<string, TypeRef> items)
    {
        Name = name;
        Items = items;
    }

    public string Name { get; }
    public Dictionary<string, TypeRef> Items { get; }

    public override bool HasDeferredTypes()
    {
        foreach (var item in Items.Values)
        {
            if (item.TypeInfo.HasDeferredTypes())
            {
                return true;
            }
        }

        return false;
    }
}