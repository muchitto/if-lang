using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class BreakStatementNode(NodeContext nodeContext) : BaseNode(nodeContext)
{
    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitBreakStatementNode(this);
    }
}