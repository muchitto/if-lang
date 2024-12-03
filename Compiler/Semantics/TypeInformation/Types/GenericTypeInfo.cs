namespace Compiler.Semantics.TypeInformation.Types;

public class GenericTypeInfo(string name, List<TypeRef> genericParams) : TypeInfo
{
    public override string? TypeName => null;

    public string Name { get; } = name;
    public List<TypeRef> GenericParams { get; } = genericParams;

    public override string ToString()
    {
        return $"{Name}<{string.Join(", ", GenericParams.Select(x => x.ToString()))}>";
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitGenericTypeInfo(this);
    }

    public override bool Compare(ITypeComparer comparer, TypeInfo other)
    {
        return comparer.CompareGenericTypeInfo(this, other);
    }
}