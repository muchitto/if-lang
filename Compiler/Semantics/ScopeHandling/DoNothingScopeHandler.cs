using Compiler.Syntax.Nodes;

namespace Compiler.Semantics.ScopeHandling;

public class DoNothingScopeHandler(SemanticHandler semanticHandler) : BaseScopeHandler(semanticHandler)
{
    public override Scope EnterScope(ScopeType scopeType, BaseNode baseNode)
    {
        return null;
    }

    public override Scope ExitScope()
    {
        return null;
    }

    public override Scope MustRecallScope(BaseNode baseNode)
    {
        return null;
    }
}