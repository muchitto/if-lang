using Compiler.Syntax.Nodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Semantics.Unification;

public class TypePromotion : BaseNodeVisitor
{
    public override ProgramNode VisitProgramNode(ProgramNode programNode)
    {
        return base.VisitProgramNode(programNode);
    }
}