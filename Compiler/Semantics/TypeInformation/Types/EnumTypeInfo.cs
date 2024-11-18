using Compiler.Semantics.ScopeHandling;

namespace Compiler.Semantics.TypeInformation.Types;

public class EnumTypeInfo(Scope scope, string name, Dictionary<string, TypeRef> fields)
    : AbstractStructuralTypeInfo(scope, fields)
{
    public override string? TypeName => Name;

    public string Name { get; } = name;

    public override void Accept(ITypeInfoVisitor typeInfoVisitor)
    {
        typeInfoVisitor.VisitEnumTypeInfo(this);
    }

    public override bool Compare(ITypeComparer comparer, TypeInfo other)
    {
        return comparer.CompareEnumTypeInfo(this, other);
    }
}