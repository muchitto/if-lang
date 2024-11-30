namespace Compiler.Semantics.TypeInformation.Types;

public class InlineEnumTypeInfo(List<InlineEnumItemTypeInfo> items) : TypeInfo
{
    public List<InlineEnumItemTypeInfo> Items { get; } = items;

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