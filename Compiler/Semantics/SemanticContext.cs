using Compiler.Semantics.ScopeHandling;

namespace Compiler.Semantics;

public class SemanticContext
{
    public List<Scope> ScopeStack { get; } = [];
    public List<Scope> AllScopes { get; } = [];

    public Scope? CurrentScope => ScopeStack.Count > 0 ? ScopeStack.Last() : null;
}