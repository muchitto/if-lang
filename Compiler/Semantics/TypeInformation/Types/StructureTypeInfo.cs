namespace Compiler.Semantics.TypeInformation.Types;

public class StructureTypeInfo(Dictionary<string, TypeRef> fields) : TypeInfo
{
    public Dictionary<string, TypeRef> Fields { get; } = fields;

    public override string ToString()
    {
        return $"Structure<{string.Join(", ", Fields.Select(x => $"{x.Key}: {x.Value}"))}>";
    }

    public override bool HasDeferredTypes()
    {
        return Fields.Values.Any(x => x.TypeInfo.HasDeferredTypes());
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitStructureTypeInfo(this);
    }
}