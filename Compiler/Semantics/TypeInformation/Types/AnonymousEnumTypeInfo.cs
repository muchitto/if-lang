namespace Compiler.Semantics.TypeInformation.Types;

public class AnonymousEnumTypeInfo(Scope scope, Dictionary<string, TypeRef> fields)
    : AbstractStructuralTypeInfo(scope, fields)
{
    public override void Accept(ITypeInfoVisitor typeInfoVisitor)
    {
        typeInfoVisitor.VisitAnonymousEnumTypeInfo(this);
    }

    public override bool Compare(ITypeComparer comparer, TypeInfo other)
    {
        return comparer.CompareAnonymousEnumTypeInfo(this, other);
    }
}