using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class BreakStatementNode : BaseNode
{
    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitBreakStatementNode(this);
    }
}