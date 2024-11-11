using Compiler.Syntax.Nodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Semantics.Unification;

public class TypePromotion(SemanticContext semanticContext) : BaseNodeVisitor(semanticContext)
{
    public override ProgramNode VisitProgramNode(ProgramNode programNode)
    {
        return base.VisitProgramNode(programNode);
    }
}