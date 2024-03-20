namespace Compiler.Semantics.TypeInformation.Types;

public class ObjectTypeInfo(TypeRef? baseClass, string name, Dictionary<string, TypeRef> fields, Scope scope) : TypeInfo
{
    public TypeRef? BaseClass { get; } = baseClass;
    public string Name { get; } = name;
    public Dictionary<string, TypeRef> Fields { get; } = fields;

    public Scope? Scope { get; } = scope;

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
}