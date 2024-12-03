using Compiler.Semantics.ScopeHandling;
using Compiler.Syntax.Nodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Semantics.SemanticPasses;

public abstract class SemanticHelperBaseNodeVisitor(SemanticHandler semanticHandler, BaseScopeHandler scopeHandler)
    : BaseNodeVisitor(semanticHandler, scopeHandler)
{
    public Scope? CurrentScope => SemanticContext.CurrentScope;

    public bool InGlobalScope => CurrentScope?.Parent == null || CurrentScope.AttachedNode is ProgramNode;

    public bool CanSetAlreadyDeclaredSymbol => InGlobalScope || SemanticHandler.InScopeType(ScopeType.Object);

    public static List<BaseNodeVisitor> DefaultPasses(SemanticContext context)
    {
        var semanticHandler = new SemanticHandler(context);

        return
        [
            new ReorganizeNodesNodeVisitor(semanticHandler),
            new MainFunctionCheckNodeVisitor(semanticHandler),
            new DeclarationCollectionNodeVisitor(semanticHandler),
            new TypeResolutionNodeVisitor(semanticHandler),
            new UnknownCheckerVisitor(semanticHandler),
            new TypeCheckNodeVisitor(semanticHandler),
            new NullCheckerNodeVisitor(semanticHandler),
            new ControlFlowConstructionNodeVisitor(semanticHandler)
        ];
    }

    public static void RunDefaultPasses(SemanticContext handler, BaseNode node)
    {
        foreach (var pass in DefaultPasses(handler))
        {
            node.Accept(pass);
        }
    }

    public static void RunDefaultPasses(BaseNode node, SemanticContext semanticContext)
    {
        RunDefaultPasses(semanticContext, node);
    }
}