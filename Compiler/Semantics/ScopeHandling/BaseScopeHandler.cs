using Compiler.Syntax.Nodes;

namespace Compiler.Semantics.ScopeHandling;

public abstract class BaseScopeHandler(SemanticHandler semanticHandler)
{
    public delegate T InScopeHandler<out T>(Scope scope);

    protected SemanticHandler SemanticHandler = semanticHandler;

    public abstract Scope EnterScope(ScopeType scopeType, BaseNode baseNode);

    public abstract Scope ExitScope();

    public abstract Scope MustRecallScope(BaseNode baseNode);

    public Scope MustRecallScope(Scope scope)
    {
        return SemanticHandler.RecallScope(scope);
    }
}