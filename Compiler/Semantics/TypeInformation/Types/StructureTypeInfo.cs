using Compiler.Semantics.ScopeHandling;

namespace Compiler.Semantics.TypeInformation.Types;

public class StructureTypeInfo(Scope scope, List<AbstractStructuralFieldTypeInfo> fields)
    : AbstractStructuralTypeInfo(scope, fields)
{
    public override string? TypeName => null;

    public override string ToString()
    {
        return $"Structure<{string.Join(", ", Fields.Select(x => $"{x.Name}: {x.TypeRef}"))}>";
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