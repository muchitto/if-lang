using Compiler.Semantics.ScopeHandling;

namespace Compiler.Semantics.TypeInformation.Types;

public class EnumTypeInfo(Scope scope, string name, List<AbstractStructuralFieldTypeInfo> fields)
    : BaseEnumTypeInfo(scope, fields)
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