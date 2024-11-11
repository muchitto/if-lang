namespace Compiler.Semantics.TypeInformation.Types;

public abstract class AbstractStructuralTypeInfo(Scope scope, Dictionary<string, TypeRef> fields)
    : ScopedTypeInfo(scope)
{
    public Dictionary<string, TypeRef> Fields { get; } = fields;
}