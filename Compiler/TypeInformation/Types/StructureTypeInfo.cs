namespace Compiler.TypeInformation.Types;

public class StructureTypeInfo(Dictionary<string, TypeRef> fields) : TypeInfo
{
    public Dictionary<string, TypeRef> Fields { get; } = fields;

    public override string ToString()
    {
        return $"Structure<{string.Join(", ", Fields.Select(x => $"{x.Key}: {x.Value}"))}>";
    }

    public override bool Compare(TypeInfo other)
    {
        if (other is not StructureTypeInfo structureTypeInfo)
        {
            return false;
        }

        // In structures, the order of fields is not important, we only care about that the fields found in
        // this is one is also found in the other, it doesn't matter if the other has more fields
        foreach (var (key, value) in Fields)
        {
            if (!structureTypeInfo.Fields.TryGetValue(key, out var otherValue) || !value.Compare(otherValue))
            {
                return false;
            }
        }

        return true;
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