using Compiler.Semantics.ScopeHandling;

namespace Compiler.Semantics.TypeInformation.Types;

public class ObjectTypeInfo(
    Scope scope,
    TypeRef? baseClass,
    string name,
    Dictionary<string, TypeRef> fields
)
    : AbstractStructuralTypeInfo(scope, fields)
{
    public TypeRef? BaseClass { get; } = baseClass;
    public string Name { get; } = name;

    public override string TypeName => Name;

    public override string ToString()
    {
        return Name;
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitObjectTypeInfo(this);
    }

    public bool IsChildOf(ObjectTypeInfo otherTypeInfo)
    {
        if (BaseClass?.TypeInfo is not ObjectTypeInfo baseClassTypeInfo)
        {
            return false;
        }

        return baseClassTypeInfo.Name == otherTypeInfo.Name || baseClassTypeInfo.IsChildOf(otherTypeInfo);
    }

    public bool IsParentOf(ObjectTypeInfo typeInfo)
    {
        return typeInfo.IsChildOf(this);
    }

    public override bool Compare(ITypeComparer comparer, TypeInfo other)
    {
        return comparer.CompareObjectTypeInfo(this, other);
    }
}