using Compiler.Semantics.ScopeHandling;

namespace Compiler.Semantics.TypeInformation.Types;

public abstract class ScopedTypeInfo(Scope scope) : TypeInfo
{
    public Scope Scope { get; } = scope;
}