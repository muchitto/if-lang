using Compiler.Syntax.Nodes;

namespace Compiler.Semantics.ScopeHandling;

public class DeclarationCollectionScopeHandler(SemanticHandler semanticHandler) : BaseScopeHandler(semanticHandler)
{
    public override Scope EnterScope(ScopeType scopeType, BaseNode baseNode)
    {
        return SemanticHandler.NewScope(scopeType, baseNode);
    }

    public override Scope ExitScope()
    {
        return SemanticHandler.PopScope();
    }

    public override Scope MustRecallScope(BaseNode baseNode)
    {
        return SemanticHandler.RecallNodeScope(baseNode);
    }
}