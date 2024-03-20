namespace Compiler.Semantics.TypeInformation.Types;

public class InlineEnumTypeInfo(Dictionary<string, TypeRef> items) : TypeInfo
{
    public Dictionary<string, TypeRef> Items { get; } = items;

    public override void Accept(ITypeInfoVisitor typeInfoVisitor)
    {
        typeInfoVisitor.VisitInlineEnumTypeInfo(this);
    }
}