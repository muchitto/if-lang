namespace Compiler.Semantics.TypeInformation.Types;

public abstract class NameTypeRefData(string name, TypeRef typeRef) : TypeInfo
{
    public string Name { get; set; } = name;

    public TypeRef TypeRef { get; set; } = typeRef;

    public override string? TypeName => Name;
}