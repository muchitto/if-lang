namespace Compiler;

public class SemanticContext
{
    public List<Scope> ScopeStack { get; } = [];
    public List<Scope> AllScopes { get; } = [];
}