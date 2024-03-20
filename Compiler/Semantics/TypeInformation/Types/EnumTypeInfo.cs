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

    public override void Accept(ITypeInfoVisitor typeInfoVisitor)
    {
        typeInfoVisitor.VisitEnumTypeInfo(this);
    }
}