namespace Compiler.Semantics.TypeInformation.Types;

public abstract class NameTypeRefData(string name, TypeRef typeRef)
{
    public string Name { get; set; } = name;

    public TypeRef TypeRef { get; set; } = typeRef;
}