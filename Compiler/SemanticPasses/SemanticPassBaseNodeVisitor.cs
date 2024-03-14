using Compiler.ScopeHandler;
using Compiler.Syntax.Visitor;

namespace Compiler.SemanticPasses;

public class SemanticPassBaseNodeVisitor(SemanticContext semanticContext, SemanticHandler semanticHandler)
    : BaseNodeVisitor
{
    protected SemanticContext SemanticContext { get; init; } = semanticContext;
    protected SemanticHandler SemanticHandler { get; init; } = semanticHandler;
}