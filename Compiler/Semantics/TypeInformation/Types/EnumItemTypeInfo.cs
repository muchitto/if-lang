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

    public override void Accept(ITypeInfoVisitor typeInfoVisitor)
    {
        typeInfoVisitor.VisitEnumItemTypeInfo(this);
    }
}