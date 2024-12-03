using Compiler.Semantics.ScopeHandling;

namespace Compiler.Semantics.TypeInformation.Types;

public class InlineEnumTypeInfo(Scope scope, List<AbstractStructuralFieldTypeInfo> items)
    : BaseEnumTypeInfo(scope, items)
{
    public override string? TypeName => null;

    public override void Accept(ITypeInfoVisitor typeInfoVisitor)
    {
        typeInfoVisitor.VisitInlineEnumTypeInfo(this);
    }

    public override bool Compare(ITypeComparer comparer, TypeInfo other)
    {
        return comparer.CompareInlineEnumTypeInfo(this, other);
    }
}