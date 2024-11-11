namespace Compiler.Semantics.TypeInformation.Types;

public class StructureTypeInfo(Scope scope, Dictionary<string, TypeRef> fields)
    : AbstractStructuralTypeInfo(scope, fields)
{
    public override string ToString()
    {
        return $"Structure<{string.Join(", ", Fields.Select(x => $"{x.Key}: {x.Value}"))}>";
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitStructureTypeInfo(this);
    }

    public override bool Compare(ITypeComparer comparer, TypeInfo other)
    {
        return comparer.CompareStructureTypeInfo(this, other);
    }
}