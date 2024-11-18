using Compiler.Semantics.ScopeHandling;

namespace Compiler.Semantics.TypeInformation.Types;

public class EnumItemTypeInfo(Scope scope, string name, Dictionary<string, TypeRef> parameters)
    : ScopedTypeInfo(scope)
{
    public override string? TypeName => Name;

    public string Name { get; } = name;

    public Dictionary<string, TypeRef> Parameters { get; } = parameters;

    public override void Accept(ITypeInfoVisitor typeInfoVisitor)
    {
        typeInfoVisitor.VisitEnumItemTypeInfo(this);
    }

    public override bool Compare(ITypeComparer comparer, TypeInfo other)
    {
        return comparer.CompareEnumItemTypeInfo(this, other);
    }
}