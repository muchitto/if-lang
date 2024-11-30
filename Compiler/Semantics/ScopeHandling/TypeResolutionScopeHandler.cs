using Compiler.Syntax.Nodes;

namespace Compiler.Semantics.ScopeHandling;

public class TypeResolutionScopeHandler(SemanticHandler semanticHandler) : BaseScopeHandler(semanticHandler)
{
    public override Scope EnterScope(ScopeType scopeType, BaseNode baseNode)
    {
        if (SemanticHandler.InScopeType(ScopeType.Function) || baseNode is BodyBlockNode)
        {
            return SemanticHandler.NewScope(scopeType, baseNode);
        }

        return SemanticHandler.RecallNodeScope(baseNode);
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