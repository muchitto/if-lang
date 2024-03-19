using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class BreakStatementNode(NodeContext nodeContext) : BaseNode(nodeContext)
{
    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitBreakStatementNode(this);
    }
}